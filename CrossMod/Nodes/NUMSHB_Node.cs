using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SSBHLib;
using SSBHLib.Formats;
using SSBHLib.Tools;
using System;
using System.Collections.Generic;
using System.IO;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numshb")]
    public class NUMSHB_Node : FileNode, IRenderableNode
    {
        public MESH _mesh;

        public NUMSHB_Node()
        {
            ImageKey = "model";
            SelectedImageKey = "model";
        }

        public override void Open(string Path)
        {
            if (SSBH.TryParseSSBHFile(Path, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is MESH)
                {
                    _mesh = (MESH)ssbhFile;
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            return GetRenderableNode(null);
        }

        public IRenderable GetRenderableNode(RSkeleton Skeleton = null)
        {
            RModel model = new RModel();

            // Merge buffers into one because OpenGL supports a single array buffer.
            int[] bufferOffsets = new int[_mesh.VertexBuffers.Length];
            byte[] vertexBuffer = new byte[0];
            int bufferOffset = 0;
            int bufferIndex = 0;

            foreach (MESH_Buffer meshBuffer in _mesh.VertexBuffers)
            {
                List<byte> combinedBuffer = new List<byte>();
                combinedBuffer.AddRange(vertexBuffer);
                combinedBuffer.AddRange(meshBuffer.Buffer);

                vertexBuffer = combinedBuffer.ToArray();
                bufferOffsets[bufferIndex++] = bufferOffset;
                bufferOffset += meshBuffer.Buffer.Length;
            }

            // Read the mesh information into the Rendering Mesh.
            foreach (MESH_Object meshObject in _mesh.Objects)
            {
                RMesh mesh = new RMesh
                {
                    Name = meshObject.Name,
                    SingleBindName = meshObject.ParentBoneName,
                    IndexCount = meshObject.IndexCount,
                    IndexOffset = (int)meshObject.ElementOffset
                };

                model.subMeshes.Add(mesh);

                if (meshObject.DrawElementType == 1)
                    mesh.DrawElementType = DrawElementsType.UnsignedInt;

                AddVertexAttributes(mesh, bufferOffsets, meshObject);

                // Rigging if skeleton exists
                // this is such a messy way of prepping it...
                if (Skeleton != null)
                {
                    Dictionary<string, int> indexByBoneName = new Dictionary<string, int>();
                    if (Skeleton != null)
                    {
                        for (int i = 0; i < Skeleton.Bones.Count; i++)
                        {
                            indexByBoneName.Add(Skeleton.Bones[i].Name, i);
                        }
                    }

                    // get the influences
                    SSBHRiggingAccessor riggingAccessor = new SSBHRiggingAccessor(_mesh);
                    SSBHVertexInfluence[] influences = riggingAccessor.ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubMeshIndex);

                    // create a bank to write
                    Vector4[] bones = new Vector4[meshObject.VertexCount];
                    Vector4[] boneWeights = new Vector4[meshObject.VertexCount];
                    foreach (SSBHVertexInfluence vertexInfluence in influences)
                    {
                        AddWeight(ref bones[vertexInfluence.VertexIndex], ref boneWeights[vertexInfluence.VertexIndex], (ushort)indexByBoneName[vertexInfluence.BoneName], vertexInfluence.Weight);
                    }

                    // build a byte buffer for the data
                    MemoryStream riggingBuffer = new MemoryStream();
                    using (BinaryWriter writer = new BinaryWriter(riggingBuffer))
                    {
                        for (int i = 0; i < meshObject.VertexCount; i++)
                        {
                            for (int j = 0; j < 4; j++)
                                writer.Write((ushort)bones[i][j]);
                            for (int j = 0; j < 4; j++)
                                writer.Write(boneWeights[i][j]);
                        }
                    }
                    byte[] riggingData = riggingBuffer.GetBuffer();
                    riggingBuffer.Dispose();

                    // add attributes for the new data
                    mesh.VertexAttributes.Add(new CustomVertexAttribute()
                    {
                        Name = "boneIndices",
                        Size = 4,
                        IType = VertexAttribIntegerType.UnsignedShort,
                        Offset = vertexBuffer.Length,
                        Stride = 4 * 6,
                        Integer = true
                    });
                    mesh.VertexAttributes.Add(new CustomVertexAttribute()
                    {
                        Name = "boneWeights",
                        Size = 4,
                        Type = VertexAttribPointerType.Float,
                        Offset = vertexBuffer.Length + 8,
                        Stride = 4 * 6
                    });

                    // Add rigging buffer onto the end of vertex buffer
                    List<byte> combinedBuffer = new List<byte>();
                    combinedBuffer.AddRange(vertexBuffer);
                    combinedBuffer.AddRange(riggingData);

                    vertexBuffer = combinedBuffer.ToArray();
                }
            }

            // Create and prepare the buffers for rendering
            model.indexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ElementArrayBuffer);
            model.indexBuffer.SetData(_mesh.PolygonBuffer, BufferUsageHint.StaticDraw);

            model.vertexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ArrayBuffer);
            model.vertexBuffer.SetData(vertexBuffer, BufferUsageHint.StaticDraw);
            
            return model;
        }

        private static void AddVertexAttributes(RMesh mesh, int[] bufferOffsets, MESH_Object meshObject)
        {
            // Vertex Attributes
            foreach (MESH_Attribute meshAttribute in meshObject.Attributes)
            {
                CustomVertexAttribute customAttribute = new CustomVertexAttribute
                {
                    Name = meshAttribute.AttributeStrings[0].Name,
                    Normalized = false,
                    Stride = meshAttribute.BufferIndex == 1 ? meshObject.Stride2 : meshObject.Stride,
                    Offset = bufferOffsets[meshAttribute.BufferIndex] + (meshAttribute.BufferIndex == 0 ? meshObject.VertexOffset : meshObject.VertexOffset2) + meshAttribute.BufferOffset,
                    Size = 3
                };

                // there may be another way to determine size, but this works for now
                if (customAttribute.Name.Equals("map1") || customAttribute.Name.Contains("uvSet"))
                {
                    customAttribute.Size = 2;
                }
                if (customAttribute.Name.Contains("colorSet"))
                {
                    customAttribute.Size = 4;
                }

                customAttribute.Type = GetAttributeType(meshAttribute);
                mesh.VertexAttributes.Add(customAttribute);

                System.Diagnostics.Debug.WriteLine($"{customAttribute.Name} {customAttribute.Size} Unk4: {meshAttribute.Unk4_0} Unk5: {meshAttribute.Unk5_0}");
            }
        }

        private static VertexAttribPointerType GetAttributeType(MESH_Attribute meshAttribute)
        {
            // convert the data type
            switch (meshAttribute.DataType)
            {
                case 0:
                    return VertexAttribPointerType.Float;
                case 2:
                    return VertexAttribPointerType.Byte; // color
                case 5:
                    return VertexAttribPointerType.HalfFloat;
                case 8:
                    return VertexAttribPointerType.HalfFloat;
                default:
                    return VertexAttribPointerType.Float;
            }
        }

        private void AddWeight(ref Vector4 b, ref Vector4 w, ushort bone, float Weight)
        {
            if(w.X == 0)
            {
                b.X = bone;
                w.X = Weight;
            } else if (w.Y == 0)
            {
                b.Y = bone;
                w.Y = Weight;
            } else if (w.Z == 0)
            {
                b.Z = bone;
                w.Z = Weight;
            } else if (w.W == 0)
            {
                b.W = bone;
                w.W = Weight;
            }
        }
    }
}
