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

        public NUMSHB_Node()
        {
            ImageKey = "model";
            SelectedImageKey = "model";
        }

        public override void Open(string Path)
        {
            string ADJB = System.IO.Path.GetDirectoryName(Path) + "/model.adjb";
            System.Console.WriteLine(ADJB);
            if (System.IO.File.Exists(ADJB))
            {
                ExtendedMesh = new CrossMod.Nodes.ADJB();
                ExtendedMesh.Read(ADJB);
            }

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
            RModel model = new RModel();

            // The bounding sphere containing all meshes.
            var modelSphere = mesh.GetBoundingSphere();
            model.BoundingSphere = new Vector4(modelSphere.Item1, modelSphere.Item2, modelSphere.Item3, modelSphere.Item4);

            foreach (MeshObject meshObject in mesh.Objects)
            {
                List<MeshObject> obs = new List<MeshObject>();
                obs.AddRange(mesh.Objects);
                System.Diagnostics.Debug.WriteLine(obs.IndexOf(meshObject).ToString("X") + " " + meshObject.Name + " " + (meshObject.IndexCount).ToString("X"));

                PrintAttributeInformation(meshObject);

                RMesh rMesh = new RMesh
                {
                    Name = meshObject.Name,
                    SingleBindName = meshObject.ParentBoneName,
                };

                // Get bounding sphere.
                var sphere = meshObject.GetBoundingSphere();
                rMesh.BoundingSphere = new Vector4(sphere.Item1, sphere.Item2, sphere.Item3, sphere.Item4);

                // Get vertex data.
                var vertexAccessor = new SSBHVertexAccessor(mesh);
                var vertexIndices = vertexAccessor.ReadIndices(0, meshObject.IndexCount, meshObject);

                System.Diagnostics.Debug.WriteLine(vertexIndices.Length.ToString("X") + " " + vertexIndices[0] + " " + vertexIndices[1] + " " + vertexIndices[2]);

                List<CustomVertex> vertices = CreateVertices(Skeleton, meshObject, vertexAccessor, vertexIndices);

                /*if(obs.IndexOf(meshObject) != 0x2B && ExtendedMesh != null && ExtendedMesh.MeshToIndexBuffer.ContainsKey(obs.IndexOf(meshObject)))
                {
                    rMesh.RenderMesh = new RenderMesh(vertices, new List<uint>(ExtendedMesh.MeshToIndexBuffer[obs.IndexOf(meshObject)]), PrimitiveType.TriangleFan);
                }
                else*/
                rMesh.RenderMesh = new RenderMesh(vertices, new List<uint>(vertexIndices));


                model.subMeshes.Add(rMesh);
            }

            return model;
        }

        private List<CustomVertex> CreateVertices(RSkeleton Skeleton, MeshObject meshObject, SSBHVertexAccessor vertexAccessor, uint[] vertexIndices)
        {

            // Read attribute values.
            var positions = vertexAccessor.ReadAttribute("Position0", 0, meshObject.VertexCount, meshObject);
            var normals = vertexAccessor.ReadAttribute("Normal0", 0, meshObject.VertexCount, meshObject);
            var tangents = vertexAccessor.ReadAttribute("Tangent0", 0, meshObject.VertexCount, meshObject);
            var map1Values = vertexAccessor.ReadAttribute("map1", 0, meshObject.VertexCount, meshObject);
            var bake1Values = vertexAccessor.ReadAttribute("bake1", 0, meshObject.VertexCount, meshObject);
            var colorSet1Values = vertexAccessor.ReadAttribute("colorSet1", 0, meshObject.VertexCount, meshObject);
            var colorSet5Values = vertexAccessor.ReadAttribute("colorSet5", 0, meshObject.VertexCount, meshObject);


            Vector3[] generatedBitangents = GenerateBitangents(vertexIndices, positions, map1Values);

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

            var vertices = new List<CustomVertex>();
            for (int i = 0; i < positions.Length; i++)
            {
                var position = GetVector4(positions[i]).Xyz;

                var normal = GetVector4(normals[i]).Xyz;
                var tangent = GetVector4(tangents[i]).Xyz;
                var bitangent = GetBitangent(generatedBitangents, i, normal);

                var map1 = GetVector4(map1Values[i]).Xy;

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

                vertices.Add(new CustomVertex(position, normal, tangent, bitangent, map1, bones, weights, bake1, colorSet1, colorSet5));
            }

            return vertices;
        }

        private static void PrintAttributeInformation(MeshObject meshObject)
        {
            System.Diagnostics.Debug.WriteLine(meshObject.Name);
            foreach (var attribute in meshObject.Attributes)
            {
                System.Diagnostics.Debug.WriteLine($"{attribute.Name} {GetAttributeType(attribute)} Unk4: {attribute.Unk4_0} Unk5: {attribute.Unk5_0}");
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        private static Vector3 GetBitangent(Vector3[] generatedBitangents, int i, Vector3 normal)
        {
            // Account for mirrored normal maps.
            var bitangent = SFGraphics.Utils.VectorUtils.Orthogonalize(generatedBitangents[i], normal);
            bitangent *= -1;
            return bitangent;
        }

        private static Vector3[] GenerateBitangents(uint[] indices, SSBHVertexAttribute[] positions, SSBHVertexAttribute[] uvs)
        {
            var generatedBitangents = new Vector3[positions.Length];
            for (int i = 0; i < indices.Length; i += 3)
            {
                SFGraphics.Utils.VectorUtils.GenerateTangentBitangent(GetVector4(positions[indices[i]]).Xyz, GetVector4(positions[indices[i + 1]]).Xyz, GetVector4(positions[indices[i + 2]]).Xyz,
                    GetVector4(uvs[indices[i]]).Xy, GetVector4(uvs[indices[i + 1]]).Xy, GetVector4(uvs[indices[i + 2]]).Xy, out Vector3 tangent, out Vector3 bitangent);

                generatedBitangents[indices[i]] += bitangent;
                generatedBitangents[indices[i + 1]] += bitangent;
                generatedBitangents[indices[i + 2]] += bitangent;
            }

            return generatedBitangents;
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
