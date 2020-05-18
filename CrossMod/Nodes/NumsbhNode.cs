using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SSBHLib;
using SSBHLib.Formats.Meshes;
using SSBHLib.Tools;
using System.Collections.Generic;
using System.IO;

namespace CrossMod.Nodes
{
    // extra node not specified in the SSBH
    // seems to be triangle adjacency for certain meshes
    public class Adjb
    {
        public Dictionary<int, uint[]> MeshToIndexBuffer = new Dictionary<int, uint[]>();

        public void Read(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                int count = reader.ReadInt32();

                int prevId = -1;
                int prevOffset = -1;
                for (int i = 0; i < count; i++)
                {
                    int id = reader.ReadInt32();
                    int offset = reader.ReadInt32();

                    if (prevOffset != -1)
                    {
                        long temp = reader.BaseStream.Position;
                        reader.BaseStream.Position = 4 + 8 * count + prevOffset;
                        uint[] buffer = new uint[(offset - prevOffset) / 2];
                        for (int j = 0; j < buffer.Length; j++)
                            buffer[j] = (uint)reader.ReadInt16();
                        reader.BaseStream.Position = temp;
                        MeshToIndexBuffer.Add(prevId, buffer);
                    }

                    prevOffset = offset;
                    prevId = id;
                }

                if (prevOffset != -1)
                {
                    long temp = reader.BaseStream.Position;
                    reader.BaseStream.Position = 4 + 8 * count + prevOffset;
                    uint[] buffer = new uint[((int)(reader.BaseStream.Length - 4 - count * 8) - prevOffset) / 2];
                    for (int j = 0; j < buffer.Length; j++)
                        buffer[j] = (uint)reader.ReadInt16();
                    reader.BaseStream.Position = temp;
                    MeshToIndexBuffer.Add(prevId, buffer);
                }
            }
        }
    }

    [FileTypeAttribute(".numshb")]
    public class NumsbhNode : FileNode
    {
        public Mesh mesh;
        public Adjb ExtendedMesh;

        public NumsbhNode(string path) : base(path)
        {
            ImageKey = "mesh";
            SelectedImageKey = "mesh";
        }

        public override void Open()
        {
            string adjb = System.IO.Path.GetDirectoryName(AbsolutePath) + "/model.adjb";
            System.Console.WriteLine(adjb);
            if (File.Exists(adjb))
            {
                ExtendedMesh = new Adjb();
                ExtendedMesh.Read(adjb);
            }

            if (Ssbh.TryParseSsbhFile(AbsolutePath, out SsbhFile ssbhFile))
            {
                if (ssbhFile is Mesh)
                {
                    mesh = (Mesh)ssbhFile;
                }
            }
        }

        public RModel GetRenderModel(RSkeleton skeleton = null)
        {
            var model = new RModel
            {
                BoundingSphere = new Vector4(mesh.BoundingSphereX, mesh.BoundingSphereY, mesh.BoundingSphereZ, mesh.BoundingSphereRadius)
            };

            foreach (MeshObject meshObject in mesh.Objects)
            {
                PrintAttributeInformation(meshObject);

                var rMesh = new RMesh
                {
                    Name = meshObject.Name,
                    SingleBindName = meshObject.ParentBoneName,
                    BoundingSphere = new Vector4(meshObject.BoundingSphereX, meshObject.BoundingSphereY,
                        meshObject.BoundingSphereZ, meshObject.BoundingSphereRadius),
                };

                // Get bounding sphere.

                // Get vertex data.
                rMesh.RenderMesh = GetRenderMesh(skeleton, meshObject, rMesh);

                model.subMeshes.Add(rMesh);
            }

            return model;
        }

        private RenderMesh GetRenderMesh(RSkeleton skeleton, MeshObject meshObject, RMesh rMesh)
        {
            var vertexAccessor = new SsbhVertexAccessor(mesh);
            var vertexIndices = vertexAccessor.ReadIndices(0, meshObject.IndexCount, meshObject);

            System.Diagnostics.Debug.WriteLine($"Vertex Count: {vertexIndices.Length}");

            List<CustomVertex> vertices = CreateVertices(skeleton, meshObject, vertexAccessor, vertexIndices);
            /*if(obs.IndexOf(meshObject) != 0x2B && ExtendedMesh != null && ExtendedMesh.MeshToIndexBuffer.ContainsKey(obs.IndexOf(meshObject)))
            {
                rMesh.RenderMesh = new RenderMesh(vertices, new List<uint>(ExtendedMesh.MeshToIndexBuffer[obs.IndexOf(meshObject)]), PrimitiveType.TriangleFan);
            }
            else*/
            return new RenderMesh(vertices, new List<uint>(vertexIndices));
        }

        private List<CustomVertex> CreateVertices(RSkeleton skeleton, MeshObject meshObject, SsbhVertexAccessor vertexAccessor, uint[] vertexIndices)
        {
            // Read attribute values.
            var positions = vertexAccessor.ReadAttribute("Position0", 0, meshObject.VertexCount, meshObject);
            var normals = vertexAccessor.ReadAttribute("Normal0", 0, meshObject.VertexCount, meshObject);
            var tangents = vertexAccessor.ReadAttribute("Tangent0", 0, meshObject.VertexCount, meshObject);
            var map1Values = vertexAccessor.ReadAttribute("map1", 0, meshObject.VertexCount, meshObject);
            var uvSetValues = vertexAccessor.ReadAttribute("uvSet", 0, meshObject.VertexCount, meshObject);
            var uvSet1Values = vertexAccessor.ReadAttribute("uvSet1", 0, meshObject.VertexCount, meshObject);
            var bake1Values = vertexAccessor.ReadAttribute("bake1", 0, meshObject.VertexCount, meshObject);
            var colorSet1Values = vertexAccessor.ReadAttribute("colorSet1", 0, meshObject.VertexCount, meshObject);
            var colorSet5Values = vertexAccessor.ReadAttribute("colorSet5", 0, meshObject.VertexCount, meshObject);

            // Convert to the appropriate OpenTK types.
            // TODO: There may be a way to skip this conversion.
            var intIndices = (int[])(object)vertexIndices;
            var positionVectors = GetVectors3d(positions);
            var normalVectors = GetVectors3d(normals);
            var map1Vectors = GetVectors2d(map1Values);
            var tangentVectors = GetVectors3d(tangents);
            var bake1Vectors = GetVectors2d(bake1Values);
            var colorSet1Vectors = GetVectors4d(colorSet1Values);
            var colorSet5Vectors = GetVectors4d(colorSet5Values);

            SFGraphics.Utils.TriangleListUtils.CalculateTangentsBitangents(positionVectors, normalVectors, map1Vectors, intIndices, out _, out Vector3[] bitangents);

            var riggingAccessor = new SsbhRiggingAccessor(mesh);
            var influences = riggingAccessor.ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubMeshIndex);
            var indexByBoneName = GetIndexByBoneName(skeleton);

            GetRiggingData(positions, influences, indexByBoneName, out IVec4[] boneIndices, out Vector4[] boneWeights);

            var vertices = new List<CustomVertex>(positions.Length);
            for (int i = 0; i < positions.Length; i++)
            {
                var uvSet = map1Vectors[i];
                if (uvSetValues.Length != 0)
                    uvSet = GetVector4(uvSetValues[i]).Xy;
                var uvSet1 = Vector2.Zero;
                if (uvSet1Values.Length != 0)
                    uvSet1 = GetVector4(uvSet1Values[i]).Xy;

                var bones = boneIndices[i];
                var weights = boneWeights[i];

                // Accessors return length 0 when the attribute isn't present.
                var bake1 = Vector2.Zero;
                if (bake1Values.Length != 0)
                    bake1 = bake1Vectors[i];

                // The values are read as float, so we can't use OpenGL to convert.
                // Convert the range [0, 128] to [0, 255]. 
                var colorSet1 = Vector4.One;
                if (colorSet1Values.Length != 0)
                    colorSet1 = colorSet1Vectors[i] / 128.0f;

                var colorSet5 = Vector4.One;
                if (colorSet5Values.Length != 0)
                    colorSet5 = colorSet5Vectors[i] / 128.0f;

                vertices.Add(new CustomVertex(positionVectors[i], normalVectors[i], tangentVectors[i], bitangents[i], map1Vectors[i], uvSet, uvSet1, bones, weights, bake1, colorSet1, colorSet5));
            }

            return vertices;
        }

        private static Dictionary<string, int> GetIndexByBoneName(RSkeleton skeleton)
        {
            var indexByBoneName = new Dictionary<string, int>();

            if (skeleton != null)
            {
                for (int i = 0; i < skeleton.Bones.Count; i++)
                {
                    indexByBoneName.Add(skeleton.Bones[i].Name, i);
                }
            }

            return indexByBoneName;
        }

        private static Vector4[] GetVectors4d(SsbhVertexAttribute[] values)
        {
            var vectors = new Vector4[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                vectors[i] = new Vector4(value.X, value.Y, value.Z, value.W);
            }

            return vectors;
        }

        private static Vector3[] GetVectors3d(SsbhVertexAttribute[] values)
        {
            var vectors = new Vector3[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                vectors[i] = new Vector3(value.X, value.Y, value.Z);
            }

            return vectors;
        }

        private static Vector2[] GetVectors2d(SsbhVertexAttribute[] values)
        {
            var vectors = new Vector2[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                vectors[i] = new Vector2(value.X, value.Y);
            }

            return vectors;
        }

        private static void GetRiggingData(SsbhVertexAttribute[] positions, SsbhVertexInfluence[] influences, Dictionary<string, int> indexByBoneName, out IVec4[] boneIndices, out Vector4[] boneWeights)
        {
            boneIndices = new IVec4[positions.Length];
            boneWeights = new Vector4[positions.Length];
            foreach (SsbhVertexInfluence influence in influences)
            {
                // Some influences refer to bones that don't exist in the skeleton.
                // _eff bones?
                if (!indexByBoneName.ContainsKey(influence.BoneName))
                    continue;

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
        }

        private static void PrintAttributeInformation(MeshObject meshObject)
        {
            System.Diagnostics.Debug.WriteLine(meshObject.Name);
            foreach (var attribute in meshObject.Attributes)
            {
                System.Diagnostics.Debug.WriteLine($"{attribute.Name} {attribute.AttributeStrings[0].Name} {GetAttributeType(attribute)} Unk4: {attribute.Unk4} Unk5: {attribute.Unk5}");
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        private static Vector4 GetVector4(SsbhVertexAttribute values)
        {
            return new Vector4(values.X, values.Y, values.Z, values.W);
        }

        private static VertexAttribPointerType GetAttributeType(MeshAttribute meshAttribute)
        {
            // TODO: Move this enum into SSBHLib.
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
    }
}
