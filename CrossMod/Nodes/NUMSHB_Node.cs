using System;
using CrossMod.Rendering;
using SSBHLib.Formats;
using SSBHLib;
using OpenTK;
using System.Collections.Generic;
using System.IO;
using SFGenericModel.VertexAttributes;
using OpenTK.Graphics.OpenGL;
using SSBHLib.Tools;

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
            RModel Model = new RModel();

            // Merge buffers into one because Opengl can't do multiple array buffers.
            int[] bufferOffsets = new int[_mesh.VertexBuffers.Length];
            byte[] vertexBuffer = new byte[0];
            int bufferOffset = 0;
            int bufferIndex = 0;

            foreach (MESH_Buffer meshBuffer in _mesh.VertexBuffers)
            {
                byte[] nfinalBuffer = new byte[meshBuffer.Buffer.Length + vertexBuffer.Length];
                Array.Copy(vertexBuffer, 0, nfinalBuffer, 0, vertexBuffer.Length);
                Array.Copy(meshBuffer.Buffer, 0, nfinalBuffer, vertexBuffer.Length, meshBuffer.Buffer.Length);
                vertexBuffer = nfinalBuffer;
                bufferOffsets[bufferIndex++] = bufferOffset;
                bufferOffset += meshBuffer.Buffer.Length;
            }

            // gonna have to tack the rigging data onto this buffer
            // so this hold the current position
            int RiggingOffset = vertexBuffer.Length; 

            // Read the mesh information into the Rendering Mesh (RMesh) class
            foreach (MESH_Object meshObject in _mesh.Objects)
            {
                RMesh Mesh = new RMesh();
                Model.subMeshes.Add(Mesh);
                Mesh.Name = meshObject.Name;
                Mesh.SingleBindName = meshObject.ParentBoneName;
                Mesh.IndexCount = meshObject.IndexCount;
                Mesh.IndexOffset = (int)meshObject.ElementOffset;

                if (meshObject.DrawElementType == 1)
                    Mesh.DrawElementType = DrawElementsType.UnsignedInt;

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

                    // convert the data type
                    switch (meshAttribute.DataType)
                    {
                        case 0:
                            customAttribute.Type = VertexAttribPointerType.Float;
                            break;
                        case 2:
                            customAttribute.Type = VertexAttribPointerType.Byte;
                            break; // color
                        case 5:
                            customAttribute.Type = VertexAttribPointerType.HalfFloat;
                            break;
                        case 8:
                            customAttribute.Type = VertexAttribPointerType.HalfFloat;
                            break;
                        default:
                            customAttribute.Type = VertexAttribPointerType.Float;
                            break;
                    }
                    Mesh.VertexAttributes.Add(customAttribute);

                    System.Diagnostics.Debug.WriteLine($"{customAttribute.Name} {customAttribute.Size}");
                }

                // Rigging if skeleton exists
                // this is such a messy way of prepping it...
                if (Skeleton != null)
                {
                    Dictionary<string, int> boneNameToIndex = new Dictionary<string, int>();
                    if (Skeleton != null)
                    {
                        for (int i = 0; i < Skeleton.Bones.Count; i++)
                        {
                            boneNameToIndex.Add(Skeleton.Bones[i].Name, i);
                        }
                    }

                    // get the influences
                    SSBHRiggingAccessor riggingAccessor = new SSBHRiggingAccessor(_mesh);
                    SSBHVertexInfluence[] Influences = riggingAccessor.ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubMeshIndex);

                    // create a bank to write
                    Vector4[] Bones = new Vector4[meshObject.VertexCount];
                    Vector4[] Weight = new Vector4[meshObject.VertexCount];
                    foreach(SSBHVertexInfluence vertexInfluence in Influences)
                    {
                        AddWeight(ref Bones[vertexInfluence.VertexIndex], ref Weight[vertexInfluence.VertexIndex], (ushort)boneNameToIndex[vertexInfluence.BoneName], vertexInfluence.Weight);
                    }

                    // build a byte buffer for the data
                    MemoryStream riggingBuffer = new MemoryStream();
                    using (BinaryWriter writer = new BinaryWriter(riggingBuffer))
                    {
                        for(int i = 0; i < meshObject.VertexCount; i++)
                        {
                            for(int j = 0; j < 4; j++)
                                writer.Write((ushort)Bones[i][j]);
                            for (int j = 0; j < 4; j++)
                                writer.Write(Weight[i][j]);
                        }
                    }
                    byte[] riggingData = riggingBuffer.GetBuffer();
                    riggingBuffer.Dispose();

                    // add attributes for the new data
                    Mesh.VertexAttributes.Add(new CustomVertexAttribute()
                    {
                        Name = "Bone",
                        Size = 4,
                        IType = VertexAttribIntegerType.UnsignedShort,
                        Offset = vertexBuffer.Length,
                        Stride = 4 * 6,
                        Integer = true
                    });
                    Mesh.VertexAttributes.Add(new CustomVertexAttribute()
                    {
                        Name = "Weight",
                        Size = 4,
                        Type = VertexAttribPointerType.Float,
                        Offset = vertexBuffer.Length + 8,
                        Stride = 4 * 6
                    });

                    // add rigging buffer onto the end of vertex buffer
                    byte[] nfinalBuffer = new byte[riggingData.Length + vertexBuffer.Length];
                    Array.Copy(vertexBuffer, 0, nfinalBuffer, 0, vertexBuffer.Length);
                    Array.Copy(riggingData, 0, nfinalBuffer, vertexBuffer.Length, riggingData.Length);
                    vertexBuffer = nfinalBuffer;
                }
            }
            
            // Create and prepare the buffers for rendering
            // they should not have their data changed after being created

            Model.indexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ElementArrayBuffer);
            Model.indexBuffer.SetData(_mesh.PolygonBuffer, BufferUsageHint.StaticDraw);

            Model.vertexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ArrayBuffer);
            Model.vertexBuffer.SetData(vertexBuffer, BufferUsageHint.StaticDraw);
            
            return Model;
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
