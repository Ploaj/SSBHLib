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
        public MESH mesh;

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
                    mesh = (MESH)ssbhFile;
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            return GetRenderableNode(null);
        }

        public IRenderable GetRenderableNode(RSkeleton Skeleton = null)
        {
            System.Diagnostics.Debug.WriteLine("Create render meshes");
            RModel model = new RModel();

            List<int> bufferOffsets = new List<int>(mesh.VertexBuffers.Length);
            int bufferOffset = 0;

            // TODO: If there are enough elements, estimating capacity may improve performance.
            List<byte> vertexBuffer = new List<byte>();

            // Merge buffers into one because OpenGL supports a single array buffer.
            foreach (MESH_Buffer meshBuffer in mesh.VertexBuffers)
            {
                vertexBuffer.AddRange(meshBuffer.Buffer);

                bufferOffsets.Add(bufferOffset);
                bufferOffset += meshBuffer.Buffer.Length;
            }

            // Read the mesh information into the Rendering Mesh.
            foreach (MESH_Object meshObject in mesh.Objects)
            {
                RMesh rMesh = new RMesh
                {
                    Name = meshObject.Name,
                    SingleBindName = meshObject.ParentBoneName,
                    IndexCount = meshObject.IndexCount,
                    IndexOffset = (int)meshObject.ElementOffset
                };

                var vertices = new List<CustomVertex>();
                var accessor = new SSBHVertexAccessor(mesh);
                var values = accessor.ReadAttribute("Position0", 0, meshObject.VertexCount, meshObject);
                foreach (var value in values)
                {
                    vertices.Add(new CustomVertex(new Vector3(value.X, value.Y, value.Z), Vector3.Zero));
                }

                var indices = accessor.ReadIndices(0, meshObject.IndexCount, meshObject);
                // TODO: SFGraphics doesn't support the other index types yet.
                var intIndices = new List<int>();
                foreach (var index in indices)
                {
                    intIndices.Add((int)index);
                }

                rMesh.RenderMesh = new RenderMesh(vertices, intIndices);

                model.subMeshes.Add(rMesh);

                if (meshObject.DrawElementType == 1)
                    rMesh.DrawElementType = DrawElementsType.UnsignedInt;

                AddVertexAttributes(rMesh, bufferOffsets, meshObject);

                // Add rigging if the skeleton exists.
                if (Skeleton != null)
                {
                    // TODO: This step is slow.
                    AddRiggingBufferData(vertexBuffer, Skeleton, meshObject, rMesh);
                }
            }

            SetVertexAndIndexBuffers(model, vertexBuffer);

            return model;
        }

        private void SetVertexAndIndexBuffers(RModel model, List<byte> vertexBuffer)
        {
            // Create and prepare the buffers for rendering
            model.indexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ElementArrayBuffer);
            model.indexBuffer.SetData(mesh.PolygonBuffer, BufferUsageHint.StaticDraw);

            model.vertexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ArrayBuffer);
            model.vertexBuffer.SetData(vertexBuffer.ToArray(), BufferUsageHint.StaticDraw);
        }

        private void AddRiggingBufferData(List<byte> vertexBuffer, RSkeleton Skeleton, MESH_Object meshObject, RMesh mesh)
        {
            // This is such a messy way of prepping it...
            Dictionary<string, int> indexByBoneName = new Dictionary<string, int>();
            if (Skeleton != null)
            {
                for (int i = 0; i < Skeleton.Bones.Count; i++)
                {
                    indexByBoneName.Add(Skeleton.Bones[i].Name, i);
                }
            }

            // Get the influences.
            SSBHRiggingAccessor riggingAccessor = new SSBHRiggingAccessor(this.mesh);
            SSBHVertexInfluence[] influences = riggingAccessor.ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubMeshIndex);

            // Create a bank for writing.
            Vector4[] bones = new Vector4[meshObject.VertexCount];
            Vector4[] boneWeights = new Vector4[meshObject.VertexCount];
            foreach (SSBHVertexInfluence vertexInfluence in influences)
            {
                AddWeight(ref bones[vertexInfluence.VertexIndex], ref boneWeights[vertexInfluence.VertexIndex], (ushort)indexByBoneName[vertexInfluence.BoneName], vertexInfluence.Weight);
            }

            byte[] riggingData = GetRiggingData(meshObject, bones, boneWeights);

            // Add attributes for the new data
            mesh.VertexAttributes.Add(new CustomVertexAttribute()
            {
                Name = "boneIndices",
                Size = 4,
                IType = VertexAttribIntegerType.UnsignedShort,
                Offset = vertexBuffer.Count,
                Stride = 4 * 6,
                Integer = true
            });

            mesh.VertexAttributes.Add(new CustomVertexAttribute()
            {
                Name = "boneWeights",
                Size = 4,
                Type = VertexAttribPointerType.Float,
                Offset = vertexBuffer.Count + 8,
                Stride = 4 * 6
            });

            // Add rigging buffer onto the end of vertex buffer
            vertexBuffer.AddRange(riggingData);
        }

        private static byte[] GetRiggingData(MESH_Object meshObject, Vector4[] bones, Vector4[] boneWeights)
        {
            byte[] riggingData;

            // Build a byte buffer for the data.
            using (MemoryStream riggingBuffer = new MemoryStream())
            {
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
                riggingData = riggingBuffer.GetBuffer();
            }

            return riggingData;
        }

        private static void AddVertexAttributes(RMesh mesh, List<int> bufferOffsets, MESH_Object meshObject)
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

                // TODO: There may be another way to determine size.
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

        private void AddWeight(ref Vector4 bones, ref Vector4 boneWeights, ushort bone, float weight)
        {
            if (boneWeights.X == 0)
            {
                bones.X = bone;
                boneWeights.X = weight;
            }
            else if (boneWeights.Y == 0)
            {
                bones.Y = bone;
                boneWeights.Y = weight;
            }
            else if (boneWeights.Z == 0)
            {
                bones.Z = bone;
                boneWeights.Z = weight;
            }
            else if (boneWeights.W == 0)
            {
                bones.W = bone;
                boneWeights.W = weight;
            }
        }
    }
}
