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
                    RenderMesh = GetRenderMesh(skeleton, meshObject),
                };

                model.subMeshes.Add(rMesh);
            }

            return model;
        }

        private RenderMesh GetRenderMesh(RSkeleton skeleton, MeshObject meshObject)
        {
            var vertexAccessor = new SsbhVertexAccessor(mesh);
            var vertexIndices = vertexAccessor.ReadIndices(0, meshObject.IndexCount, meshObject);

            System.Diagnostics.Debug.WriteLine($"Vertex Count: {vertexIndices.Length}");

            var vertices = CreateVertices(skeleton, meshObject, vertexAccessor, vertexIndices);
            return new RenderMesh(vertices, vertexIndices);
        }

        private CustomVertex[] CreateVertices(RSkeleton skeleton, MeshObject meshObject, SsbhVertexAccessor vertexAccessor, uint[] vertexIndices)
        {
            SsbhVertexAttribute[] positionValues = ReadAttributeOrSetZero("Position0", meshObject, vertexAccessor);
            var normalValues = ReadAttributeOrSetZero("Normal0", meshObject, vertexAccessor);
            var tangentValues = ReadAttributeOrSetZero("Tangent0", meshObject, vertexAccessor);
            var map1Values = ReadAttributeOrSetZero("map1", meshObject, vertexAccessor);
            var uvSetValues = ReadAttributeOrSetZero("uvSet", meshObject, vertexAccessor);
            var uvSet1Values = ReadAttributeOrSetZero("uvSet1", meshObject, vertexAccessor);
            var uvSet2Values = ReadAttributeOrSetZero("uvSet2", meshObject, vertexAccessor);
            var bake1Values = ReadAttributeOrSetZero("bake1", meshObject, vertexAccessor);
            var colorSet1Values = ReadAttributeOrSetDefault("colorSet1", meshObject, vertexAccessor, 128f);
            var colorSet2Values = ReadAttributeOrSetZero("colorSet2", meshObject, vertexAccessor);
            var colorSet21Values = ReadAttributeOrSetZero("colorSet2_1", meshObject, vertexAccessor);
            var colorSet22Values = ReadAttributeOrSetZero("colorSet2_2", meshObject, vertexAccessor);
            var colorSet23Values = ReadAttributeOrSetZero("colorSet2_3", meshObject, vertexAccessor);
            var colorSet3Values = ReadAttributeOrSetZero("colorSet3", meshObject, vertexAccessor);
            var colorSet4Values = ReadAttributeOrSetZero("colorSet4", meshObject, vertexAccessor);
            var colorSet5Values = ReadAttributeOrSetZero("colorSet5", meshObject, vertexAccessor);
            var colorSet6Values = ReadAttributeOrSetZero("colorSet6", meshObject, vertexAccessor);
            var colorSet7Values = ReadAttributeOrSetZero("colorSet7", meshObject, vertexAccessor);

            var riggingAccessor = new SsbhRiggingAccessor(mesh);
            var influences = riggingAccessor.ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubMeshIndex);
            var indexByBoneName = GetIndexByBoneName(skeleton);

            GetRiggingData(positionValues, influences, indexByBoneName, out IVec4[] boneIndices, out Vector4[] boneWeights);

            var vertices = new CustomVertex[positionValues.Length];
            for (int i = 0; i < positionValues.Length; i++)
            {
                vertices[i] = new CustomVertex(positionValues[i], normalValues[i], tangentValues[i],
                    map1Values[i], uvSetValues[i], uvSet1Values[i], uvSet2Values[i], boneIndices[i], boneWeights[i], bake1Values[i],
                    colorSet1Values[i], colorSet2Values[i], colorSet21Values[i], colorSet22Values[i], colorSet23Values[i],
                    colorSet3Values[i], colorSet4Values[i], colorSet5Values[i], colorSet6Values[i], colorSet7Values[i]);
            }

            return vertices;
        }

        private static SsbhVertexAttribute[] ReadAttributeOrSetZero(string name, MeshObject meshObject,
            SsbhVertexAccessor vertexAccessor)
        {
            // Accessors return length 0 when the attribute isn't present.
            var result = vertexAccessor.ReadAttribute(name, 0, meshObject.VertexCount, meshObject);
            if (result.Length == 0)
                result = new SsbhVertexAttribute[meshObject.VertexCount];
            return result;
        }

        private static SsbhVertexAttribute[] ReadAttributeOrSetDefault(string name, MeshObject meshObject,
            SsbhVertexAccessor vertexAccessor, float defaultValue)
        {
            // Accessors return length 0 when the attribute isn't present.
            var result = vertexAccessor.ReadAttribute(name, 0, meshObject.VertexCount, meshObject);
            if (result.Length == 0)
            {
                result = new SsbhVertexAttribute[meshObject.VertexCount];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = new SsbhVertexAttribute(defaultValue, defaultValue, defaultValue, defaultValue);
                }
            }
            return result;
        }

        private static Dictionary<string, int> GetIndexByBoneName(RSkeleton skeleton)
        {
            if (skeleton == null)
                return new Dictionary<string, int>();

            var indexByBoneName = new Dictionary<string, int>(skeleton.Bones.Count);
            for (int i = 0; i < skeleton.Bones.Count; i++)
            {
                indexByBoneName.Add(skeleton.Bones[i].Name, i);
            }

            return indexByBoneName;
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

                if (boneWeights[influence.VertexIndex].X == 0.0)
                {
                    boneIndices[influence.VertexIndex].X = indexByBoneName[influence.BoneName];
                    boneWeights[influence.VertexIndex].X = influence.Weight;
                }
                else if (boneWeights[influence.VertexIndex].Y == 0.0)
                {
                    boneIndices[influence.VertexIndex].Y = indexByBoneName[influence.BoneName];
                    boneWeights[influence.VertexIndex].Y = influence.Weight;
                }
                else if (boneWeights[influence.VertexIndex].Z == 0.0)
                {
                    boneIndices[influence.VertexIndex].Z = indexByBoneName[influence.BoneName];
                    boneWeights[influence.VertexIndex].Z = influence.Weight;
                }
                else if (boneWeights[influence.VertexIndex].W == 0.0)
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
