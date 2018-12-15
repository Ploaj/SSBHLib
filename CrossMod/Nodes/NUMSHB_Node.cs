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
    public class NUMSHB_Node : FileNode
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

        public RModel GetRenderModel(RSkeleton Skeleton = null)
        {
            System.Diagnostics.Debug.WriteLine("Create render meshes");
            RModel model = new RModel();

            // Read the mesh information into the Rendering Mesh.
            foreach (MESH_Object meshObject in mesh.Objects)
            {
                RMesh rMesh = new RMesh
                {
                    Name = meshObject.Name,
                    SingleBindName = meshObject.ParentBoneName,
                };

                var vertexAccessor = new SSBHVertexAccessor(mesh);

                var indices = vertexAccessor.ReadIndices(0, meshObject.IndexCount, meshObject);
                // TODO: SFGraphics doesn't support the other index types yet.
                var intIndices = new List<int>();
                foreach (var index in indices)
                {
                    intIndices.Add((int)index);
                }

                // Read attribute values.
                var positions = vertexAccessor.ReadAttribute("Position0", 0, meshObject.VertexCount, meshObject);
                var normals = vertexAccessor.ReadAttribute("Normal0", 0, meshObject.VertexCount, meshObject);
                var tangents = vertexAccessor.ReadAttribute("Tangent0", 0, meshObject.VertexCount, meshObject);
                var map1Values = vertexAccessor.ReadAttribute("map1", 0, meshObject.VertexCount, meshObject);
                var bake1Values = vertexAccessor.ReadAttribute("bake1", 0, meshObject.VertexCount, meshObject);

                Vector3[] generatedBitangents = GenerateBitangents(intIndices, positions, map1Values);

                var boneIndices = new IVec4[positions.Length];
                var boneWeights = new Vector4[positions.Length];

                var riggingAccessor = new SSBHRiggingAccessor(mesh);
                SSBHVertexInfluence[] influences = riggingAccessor.ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubMeshIndex);
                Dictionary<string, int> indexByBoneName = new Dictionary<string, int>();
                if (Skeleton != null)
                {
                    for (int i = 0; i < Skeleton.Bones.Count; i++)
                    {
                        indexByBoneName.Add(Skeleton.Bones[i].Name, i);
                    }
                }

                foreach (SSBHVertexInfluence influence in influences)
                {
                    if (boneWeights[influence.VertexIndex].X == 0)
                    {
                        boneIndices[influence.VertexIndex].X = indexByBoneName[influence.BoneName];
                        boneWeights[influence.VertexIndex].X = influence.Weight;
                    }
                    else if (boneWeights[influence.VertexIndex].Y == 0)
                    {
                        boneIndices[influence.VertexIndex].Y = indexByBoneName[influence.BoneName];
                        boneWeights[influence.VertexIndex].Y = influence.Weight;
                    }
                    else if (boneWeights[influence.VertexIndex].Z == 0)
                    {
                        boneIndices[influence.VertexIndex].Z = indexByBoneName[influence.BoneName];
                        boneWeights[influence.VertexIndex].Z = influence.Weight;
                    }
                    else if (boneWeights[influence.VertexIndex].W == 0)
                    {
                        boneIndices[influence.VertexIndex].W = indexByBoneName[influence.BoneName];
                        boneWeights[influence.VertexIndex].W = influence.Weight;
                    }
                }

                var vertices = new List<CustomVertex>();
                for (int i = 0; i < positions.Length; i++)
                {
                    var position = GetVector4(positions[i]).Xyz;
                    var normal = GetVector4(normals[i]).Xyz;
                    var tangent = GetVector4(tangents[i]).Xyz;
                    var map1 = GetVector4(map1Values[i]).Xy;
                    var bones = boneIndices[i];
                    var weights = boneWeights[i];

                    // Accessors return length 0 when the attribute isn't present.
                    var bake1 = new Vector2(0);
                    if (bake1Values.Length != 0)
                        bake1 = GetVector4(bake1Values[i]).Xy;

                    Vector3 bitangent = GetBitangent(generatedBitangents, i, normal);

                    vertices.Add(new CustomVertex(position, normal, tangent, bitangent, map1, bones, weights, bake1));
                }

                rMesh.RenderMesh = new RenderMesh(vertices, intIndices);

                model.subMeshes.Add(rMesh);

                if (meshObject.DrawElementType == 1)
                    rMesh.DrawElementType = DrawElementsType.UnsignedInt;

                AddVertexAttributes(rMesh, meshObject);
            }

            return model;
        }

        private static Vector3 GetBitangent(Vector3[] generatedBitangents, int i, Vector3 normal)
        {
            // Account for mirrored normal maps.
            var bitangent = SFGraphics.Utils.VectorUtils.Orthogonalize(generatedBitangents[i], normal);
            bitangent *= -1;
            return bitangent;
        }

        private static Vector3[] GenerateBitangents(List<int> intIndices, SSBHVertexAttribute[] positions, SSBHVertexAttribute[] uvs)
        {
            var generatedBitangents = new Vector3[positions.Length];
            for (int i = 0; i < intIndices.Count; i += 3)
            {
                SFGraphics.Utils.VectorUtils.GenerateTangentBitangent(GetVector4(positions[intIndices[i]]).Xyz, GetVector4(positions[intIndices[i + 1]]).Xyz, GetVector4(positions[intIndices[i + 2]]).Xyz,
                    GetVector4(uvs[intIndices[i]]).Xy, GetVector4(uvs[intIndices[i + 1]]).Xy, GetVector4(uvs[intIndices[i + 2]]).Xy, out Vector3 tangent, out Vector3 bitangent);

                generatedBitangents[intIndices[i]] += bitangent;
                generatedBitangents[intIndices[i + 1]] += bitangent;
                generatedBitangents[intIndices[i + 2]] += bitangent;
            }

            return generatedBitangents;
        }

        private static Vector4 GetVector4(SSBHVertexAttribute values)
        {
            return new Vector4(values.X, values.Y, values.Z, values.W);
        }

        private static void AddVertexAttributes(RMesh mesh, MESH_Object meshObject)
        {
            // Vertex Attributes
            foreach (MESH_Attribute meshAttribute in meshObject.Attributes)
            {
                CustomVertexAttribute customAttribute = new CustomVertexAttribute
                {
                    Name = meshAttribute.AttributeStrings[0].Name,
                    Normalized = false,
                    Stride = meshAttribute.BufferIndex == 1 ? meshObject.Stride2 : meshObject.Stride,
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

                System.Diagnostics.Debug.WriteLine($"{customAttribute.Name} {customAttribute.Size} {customAttribute.Type}");
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
