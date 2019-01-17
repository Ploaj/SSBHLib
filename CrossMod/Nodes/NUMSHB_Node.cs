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
    public class ADJB
    {
        public Dictionary<int, uint[]> MeshToIndexBuffer = new Dictionary<int, uint[]>();

        public void Read(string FilePath)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                int Count = reader.ReadInt32();

                int PrevID = -1;
                int PrevOffset = -1;
                for(int i = 0; i < Count; i++)
                {
                    int ID = reader.ReadInt32();
                    int Offset = reader.ReadInt32();

                    if(PrevOffset != -1)
                    {
                        long temp = reader.BaseStream.Position;
                        reader.BaseStream.Position = 4 + 8 * Count + PrevOffset;
                        uint[] buffer = new uint[(Offset - PrevOffset) / 2];
                        for (int j = 0; j < buffer.Length; j++)
                            buffer[j] = (uint)reader.ReadInt16();
                        reader.BaseStream.Position = temp;
                        MeshToIndexBuffer.Add(PrevID, buffer);
                    }

                    PrevOffset = Offset;
                    PrevID = ID;
                }

                if (PrevOffset != -1)
                {
                    long temp = reader.BaseStream.Position;
                    reader.BaseStream.Position = 4 + 8 * Count + PrevOffset;
                    uint[] buffer = new uint[((int)(reader.BaseStream.Length - 4 - Count * 8) - PrevOffset) / 2];
                    for (int j = 0; j < buffer.Length; j++)
                        buffer[j] = (uint)reader.ReadInt16();
                    reader.BaseStream.Position = temp;
                    MeshToIndexBuffer.Add(PrevID, buffer);
                }
            }
        }
    }

    [FileTypeAttribute(".numshb")]
    public class NUMSHB_Node : FileNode
    {
        public MESH mesh;
        public ADJB ExtendedMesh;

        public NUMSHB_Node(string path): base(path)
        {
            ImageKey = "mesh";
            SelectedImageKey = "mesh";
        }

        public override void Open()
        {
            string ADJB = System.IO.Path.GetDirectoryName(AbsolutePath) + "/model.adjb";
            System.Console.WriteLine(ADJB);
            if (File.Exists(ADJB))
            {
                ExtendedMesh = new ADJB();
                ExtendedMesh.Read(ADJB);
            }

            if (SSBH.TryParseSSBHFile(AbsolutePath, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is MESH)
                {
                    mesh = (MESH)ssbhFile;
                }
            }
        }

        public RModel GetRenderModel(RSkeleton Skeleton = null)
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
                };

                // Get bounding sphere.
                rMesh.BoundingSphere = new Vector4(meshObject.BoundingSphereX, meshObject.BoundingSphereY, meshObject.BoundingSphereZ, meshObject.BoundingSphereRadius);

                // Get vertex data.
                rMesh.RenderMesh = GetRenderMesh(Skeleton, meshObject, rMesh);

                model.subMeshes.Add(rMesh);
            }

            return model;
        }

        private RenderMesh GetRenderMesh(RSkeleton Skeleton, MeshObject meshObject, RMesh rMesh)
        {
            using (var vertexAccessor = new SSBHVertexAccessor(mesh))
            {
                var vertexIndices = vertexAccessor.ReadIndices(0, meshObject.IndexCount, meshObject);

                System.Diagnostics.Debug.WriteLine($"Vertex Count: {vertexIndices.Length}");

                List<CustomVertex> vertices = CreateVertices(Skeleton, meshObject, vertexAccessor, vertexIndices);
                /*if(obs.IndexOf(meshObject) != 0x2B && ExtendedMesh != null && ExtendedMesh.MeshToIndexBuffer.ContainsKey(obs.IndexOf(meshObject)))
                {
                    rMesh.RenderMesh = new RenderMesh(vertices, new List<uint>(ExtendedMesh.MeshToIndexBuffer[obs.IndexOf(meshObject)]), PrimitiveType.TriangleFan);
                }
                else*/
                return new RenderMesh(vertices, new List<uint>(vertexIndices));
            }
        }

        private List<CustomVertex> CreateVertices(RSkeleton Skeleton, MeshObject meshObject, SSBHVertexAccessor vertexAccessor, uint[] vertexIndices)
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

            // TODO: Convert vectors.
            var intIndices = new List<int>();
            foreach (var value in vertexIndices)
            {
                intIndices.Add((int)value);
            }
            SFGraphics.Utils.TriangleListUtils.CalculateTangentsBitangents(GetVectors3d(positions), GetVectors3d(normals), GetVectors2d(map1Values), intIndices, out Vector3[] newTangents, out Vector3[] bitangents);

            var riggingAccessor = new SSBHRiggingAccessor(mesh);
            var influences = riggingAccessor.ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubMeshIndex);
            var indexByBoneName = new Dictionary<string, int>();

            if (Skeleton != null)
            {
                for (int i = 0; i < Skeleton.Bones.Count; i++)
                {
                    indexByBoneName.Add(Skeleton.Bones[i].Name, i);
                }
            }

            GetRiggingData(positions, influences, indexByBoneName, out IVec4[] boneIndices, out Vector4[] boneWeights);

            var vertices = new List<CustomVertex>(positions.Length);
            for (int i = 0; i < positions.Length; i++)
            {
                var position = GetVector4(positions[i]).Xyz;

                var normal = GetVector4(normals[i]).Xyz;
                var tangent = GetVector4(tangents[i]).Xyz;
                var bitangent = bitangents[i];

                var map1 = GetVector4(map1Values[i]).Xy;

                var uvSet = map1;
                if (uvSetValues.Length != 0)
                    uvSet = GetVector4(uvSetValues[i]).Xy;
                var uvSet1 = new Vector2(0);
                if (uvSet1Values.Length != 0)
                    uvSet1 = GetVector4(uvSet1Values[i]).Xy;

                var bones = boneIndices[i];
                var weights = boneWeights[i];

                // Accessors return length 0 when the attribute isn't present.
                var bake1 = new Vector2(0);
                if (bake1Values.Length != 0)
                    bake1 = GetVector4(bake1Values[i]).Xy;

                // The values are read as float, so we can't use OpenGL to convert.
                // Convert the range [0, 128] to [0, 255]. 
                var colorSet1 = new Vector4(1);
                if (colorSet1Values.Length != 0)
                    colorSet1 = GetVector4(colorSet1Values[i]) / 128.0f;

                var colorSet5 = new Vector4(1);
                if (colorSet5Values.Length != 0)
                    colorSet5 = GetVector4(colorSet5Values[i]) / 128.0f;

                vertices.Add(new CustomVertex(position, normal, tangent, bitangent, map1, uvSet, uvSet1, bones, weights, bake1, colorSet1, colorSet5));
            }

            return vertices;
        }

        private static List<Vector3> GetVectors3d(SSBHVertexAttribute[] values)
        {
            var vectors = new List<Vector3>();
            foreach (var value in values)
            {
                vectors.Add(GetVector4(value).Xyz);
            }
            return vectors;
        }

        private static List<Vector2> GetVectors2d(SSBHVertexAttribute[] values)
        {
            var vectors = new List<Vector2>();
            foreach (var value in values)
            {
                vectors.Add(GetVector4(value).Xy);
            }
            return vectors;
        }

        private static void GetRiggingData(SSBHVertexAttribute[] positions, SSBHVertexInfluence[] influences, Dictionary<string, int> indexByBoneName, out IVec4[] boneIndices, out Vector4[] boneWeights)
        {
            boneIndices = new IVec4[positions.Length];
            boneWeights = new Vector4[positions.Length];
            foreach (SSBHVertexInfluence influence in influences)
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
                System.Diagnostics.Debug.WriteLine($"{attribute.Name} {attribute.AttributeStrings[0].Name} {GetAttributeType(attribute)} Unk4: {attribute.Unk4_0} Unk5: {attribute.Unk5_0}");
            }
            System.Diagnostics.Debug.WriteLine("");
        }
       
        private static Vector4 GetVector4(SSBHVertexAttribute values)
        {
            return new Vector4(values.X, values.Y, values.Z, values.W);
        }

        private static VertexAttribPointerType GetAttributeType(MeshAttribute meshAttribute)
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
    }
}
