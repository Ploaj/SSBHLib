using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.VertexAttributes;
using SSBHLib;
using SSBHLib.Formats.Meshes;
using SSBHLib.Tools;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
            string adjb = Path.GetDirectoryName(AbsolutePath) + "/model.adjb";
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
                    RenderMesh = CreateRenderMesh(skeleton, meshObject),
                };

                model.SubMeshes.Add(rMesh);
            }

            return model;
        }

        private UltimateMesh CreateRenderMesh(RSkeleton skeleton, MeshObject meshObject)
        {
            var vertexAccessor = new SsbhVertexAccessor(mesh);
            var vertexIndices = vertexAccessor.ReadIndices(meshObject);

            System.Diagnostics.Debug.WriteLine($"Vertex Count: {vertexIndices.Length}");

            var renderMesh = new UltimateMesh(vertexIndices, meshObject.VertexCount);

            ConfigureVertexAttributes(renderMesh, skeleton, meshObject, vertexAccessor);
            return renderMesh;
        }

        private void ConfigureVertexAttributes(UltimateMesh renderMesh, RSkeleton skeleton, MeshObject meshObject, SsbhVertexAccessor vertexAccessor)
        {
            // TODO: Just use the mesh buffer.
            var positionValues = ReadAttributeOrSetZero("Position0", meshObject, vertexAccessor);
            var normalValues = ReadAttributeOrSetZero("Normal0", meshObject, vertexAccessor);
            var tangentValues = ReadAttributeOrSetZero("Tangent0", meshObject, vertexAccessor);
            var map1Values = ReadAttributeOrSetZero("map1", meshObject, vertexAccessor);
            var uvSetValues = ReadAttributeOrSetZero("uvSet", meshObject, vertexAccessor);
            var uvSet1Values = ReadAttributeOrSetZero("uvSet1", meshObject, vertexAccessor);
            var uvSet2Values = ReadAttributeOrSetZero("uvSet2", meshObject, vertexAccessor);
            var bake1Values = ReadAttributeOrSetZero("bake1", meshObject, vertexAccessor);
            var colorSet1Values = ReadAttributeOrSetDefault("colorSet1", meshObject, vertexAccessor, 128f);

            // TODO: How to reduce the number of attributes if attributes are read directly from a shared mesh buffer?
            var colorSet2Values = ReadAttributeOrSetZero("colorSet2", meshObject, vertexAccessor);
            var colorSet21Values = ReadAttributeOrSetZero("colorSet2_1", meshObject, vertexAccessor);
            var colorSet22Values = ReadAttributeOrSetZero("colorSet2_2", meshObject, vertexAccessor);
            var colorSet23Values = ReadAttributeOrSetZero("colorSet2_3", meshObject, vertexAccessor);
            var colorSet3Values = ReadAttributeOrSetZero("colorSet3", meshObject, vertexAccessor);
            var colorSet4Values = ReadAttributeOrSetZero("colorSet4", meshObject, vertexAccessor);
            var colorSet5Values = ReadAttributeOrSetZero("colorSet5", meshObject, vertexAccessor);
            var colorSet6Values = ReadAttributeOrSetZero("colorSet6", meshObject, vertexAccessor);
            var colorSet7Values = ReadAttributeOrSetZero("colorSet7", meshObject, vertexAccessor);

            AddAttribute("Position0", renderMesh, positionValues);
            AddAttribute("Normal0", renderMesh, normalValues);
            AddAttribute("Tangent0", renderMesh, tangentValues);
            AddAttribute("map1", renderMesh, map1Values);
            AddAttribute("uvSet", renderMesh, uvSetValues);
            AddAttribute("uvSet1", renderMesh, uvSet1Values);
            AddAttribute("uvSet2", renderMesh, uvSet2Values);
            AddAttribute("bake1", renderMesh, bake1Values);

            AddAttribute("colorSet1", renderMesh, colorSet1Values);

            var riggingAccessor = new SsbhRiggingAccessor(mesh);
            var influences = riggingAccessor.ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubMeshIndex);
            var indexByBoneName = GetIndexByBoneName(skeleton);

            GetRiggingData(meshObject.VertexCount, influences, indexByBoneName, out IVec4[] boneIndices, out Vector4[] boneWeights);

            // TODO: Add option to SFGraphics to combine these calls.
            // TODO: Add option to skip offset and stride if the whole buffer is used.
            renderMesh.AddBuffer("boneIndexBuffer", boneIndices);
            renderMesh.ConfigureAttribute(new VertexIntAttribute("boneIndices", ValueCount.Four, VertexAttribIntegerType.Int), "boneIndexBuffer", 0, sizeof(int) * 4);

            renderMesh.AddBuffer("boneWeightBuffer", boneWeights);
            renderMesh.ConfigureAttribute(new VertexFloatAttribute("boneWeights", ValueCount.Four, VertexAttribPointerType.Float, false), "boneWeightBuffer", 0, sizeof(float) * 4);
        }

        private static void AddAttribute(string name, UltimateMesh renderMesh, SsbhVertexAttribute[] values)
        {
            renderMesh.AddBuffer(name, values);
            renderMesh.ConfigureAttribute(new VertexFloatAttribute(name, ValueCount.Four, VertexAttribPointerType.Float, false), name, 0, Marshal.SizeOf(typeof(SsbhVertexAttribute)));
        }

        private static SsbhVertexAttribute[] ReadAttributeOrSetZero(string name, MeshObject meshObject,
            SsbhVertexAccessor vertexAccessor)
        {
            // Accessors return length 0 when the attribute isn't present.
            var result = vertexAccessor.ReadAttribute(name, meshObject);
            if (result.Length == 0)
                result = new SsbhVertexAttribute[meshObject.VertexCount];
            return result;
        }

        private static SsbhVertexAttribute[] ReadAttributeOrSetDefault(string name, MeshObject meshObject,
            SsbhVertexAccessor vertexAccessor, float defaultValue)
        {
            // Accessors return length 0 when the attribute isn't present.
            var result = vertexAccessor.ReadAttribute(name, meshObject);
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

        private static void GetRiggingData(int vertexCount, SsbhVertexInfluence[] influences, Dictionary<string, int> indexByBoneName, out IVec4[] boneIndices, out Vector4[] boneWeights)
        {
            boneIndices = new IVec4[vertexCount];
            boneWeights = new Vector4[vertexCount];
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
                System.Diagnostics.Debug.WriteLine($"{attribute.Name} {attribute.AttributeStrings[0].Name} {GetGlAttributeType(attribute)} Unk4: {attribute.Unk4} Unk5: {attribute.Unk5}");
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        private static VertexAttribPointerType GetGlAttributeType(MeshAttribute meshAttribute)
        {
            switch (meshAttribute.DataType)
            {
                case MeshAttribute.AttributeDataType.Float:
                    return VertexAttribPointerType.Float;
                case MeshAttribute.AttributeDataType.Byte:
                    return VertexAttribPointerType.Byte; 
                case MeshAttribute.AttributeDataType.HalfFloat:
                    return VertexAttribPointerType.HalfFloat;
                case MeshAttribute.AttributeDataType.HalfFloat2:
                    return VertexAttribPointerType.HalfFloat;
                default:
                    return VertexAttribPointerType.Float;
            }
        }
    }
}
