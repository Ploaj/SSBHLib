using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using CrossMod.Tools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.VertexAttributes;
using SFGraphics.GLObjects.BufferObjects;
using SSBHLib;
using SSBHLib.Formats.Meshes;
using SSBHLib.Tools;
using System.Collections.Generic;
using System.IO;

namespace CrossMod.Nodes
{
    public class NumsbhNode : FileNode
    {
        public Mesh mesh;
        public Adjb ExtendedMesh;

        public NumsbhNode(string path) : base(path, "mesh", false)
        {
            Open();
        }

        private void Open()
        {
            string adjb = Path.GetDirectoryName(AbsolutePath) + "/model.adjb";
            if (File.Exists(adjb))
            {
                ExtendedMesh = new Adjb();
                ExtendedMesh.Read(adjb);
            }

            Ssbh.TryParseSsbhFile(AbsolutePath, out mesh);
        }

        public RModel GetRenderModel(RSkeleton skeleton)
        {
            var model = new RModel
            {
                BoundingSphere = new Vector4(mesh.BoundingSphereCenter.ToOpenTK(), mesh.BoundingSphereRadius)
            };

            // Use a shared buffer to improve performance.
            // The render meshes will keep references to these objects.
            var vertexBuffer0 = new BufferObject(BufferTarget.ArrayBuffer);
            vertexBuffer0.SetData(mesh.VertexBuffers[0].Buffer, BufferUsageHint.StaticDraw);

            var vertexBuffer1 = new BufferObject(BufferTarget.ArrayBuffer);
            vertexBuffer1.SetData(mesh.VertexBuffers[1].Buffer, BufferUsageHint.StaticDraw);

            foreach (MeshObject meshObject in mesh.Objects)
            {
                var singleBindIndex = skeleton.GetBoneIndex(meshObject.ParentBoneName);

                var rMesh = new RMesh(meshObject.Name, 
                    meshObject.SubIndex, 
                    meshObject.ParentBoneName,
                    singleBindIndex,
                    new Vector4(meshObject.BoundingSphereCenter.ToOpenTK(), 
                    meshObject.BoundingSphereRadius),
                    CreateRenderMesh(skeleton, meshObject, vertexBuffer0, vertexBuffer1)
                );

                model.SubMeshes.Add(rMesh);
            }

            return model;
        }

        private UltimateMesh CreateRenderMesh(RSkeleton skeleton, MeshObject meshObject, BufferObject vertexBuffer0, BufferObject vertexBuffer1)
        {
            var vertexAccessor = new SsbhVertexAccessor(mesh);
            var vertexIndices = vertexAccessor.ReadIndices(meshObject);

            var renderMesh = new UltimateMesh(vertexIndices, meshObject.VertexCount);

            ConfigureVertexAttributes(renderMesh, skeleton, meshObject, vertexBuffer0, vertexBuffer1);
            return renderMesh;
        }

        private void ConfigureVertexAttributes(UltimateMesh renderMesh, RSkeleton skeleton, MeshObject meshObject, BufferObject vertexBuffer0, BufferObject vertexBuffer1)
        {
            var riggingAccessor = new SsbhRiggingAccessor(mesh);
            var influences = riggingAccessor.ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubIndex);
            var indexByBoneName = GetIndexByBoneName(skeleton);

            // TODO: Optimize reading/configuring rigging buffer?
            GetRiggingData(meshObject.VertexCount, influences, indexByBoneName, out IVec4[] boneIndices, out Vector4[] boneWeights);

            renderMesh.AddBuffer("boneIndexBuffer", boneIndices);
            renderMesh.ConfigureAttribute(new VertexIntAttribute("boneIndices", ValueCount.Four, VertexAttribIntegerType.Int), "boneIndexBuffer", 0, sizeof(int) * 4);

            renderMesh.AddBuffer("boneWeightBuffer", boneWeights);
            renderMesh.ConfigureAttribute(new VertexFloatAttribute("boneWeights", ValueCount.Four, VertexAttribPointerType.Float, false), "boneWeightBuffer", 0, sizeof(float) * 4);

            // Shared vertex buffers for all mesh objects.
            renderMesh.AddBuffer($"vertexBuffer0", vertexBuffer0);
            renderMesh.AddBuffer($"vertexBuffer1", vertexBuffer1);

            var usedAttributes = new HashSet<string>();
            foreach (var attribute in meshObject.Attributes)
            {
                var name = ConfigureAttribute(renderMesh, meshObject, attribute);
                usedAttributes.Add(name);
            }

            // Configure unused attributes to just use black.
            Vector4[] defaultColorSet1 = CreateBuffer(meshObject, new Vector4(0.5f));

            // x^2 * 7 should equal 1.0 to have no effect when multiplying.
            Vector4[] defaultColorSet2 = CreateBuffer(meshObject, new Vector4((float)System.Math.Sqrt(1.0 / 7.0)));

            // x * 3 should equal 1.0 to have no effect when multiplying.
            Vector4[] defaultColorSet5 = CreateBuffer(meshObject, new Vector4(1.0f / 3.0f));

            renderMesh.AddBuffer("defaultBlack", new Vector4[meshObject.VertexCount]);
            renderMesh.AddBuffer("defaultColorSet1", defaultColorSet1);
            renderMesh.AddBuffer("defaultColorSet2", defaultColorSet2);
            renderMesh.AddBuffer("defaultColorSet5", defaultColorSet5);

            ConfigureDefaultBufferVec4(renderMesh, "Normal0", usedAttributes, "defaultBlack");
            ConfigureDefaultBufferVec4(renderMesh, "Tangent0", usedAttributes, "defaultBlack");

            ConfigureDefaultBufferVec2(renderMesh, "map1", usedAttributes, "defaultBlack");
            ConfigureDefaultBufferVec2(renderMesh, "uvSet", usedAttributes, "defaultBlack");
            ConfigureDefaultBufferVec2(renderMesh, "uvSet1", usedAttributes, "defaultBlack");
            ConfigureDefaultBufferVec2(renderMesh, "uvSet2", usedAttributes, "defaultBlack");
            ConfigureDefaultBufferVec2(renderMesh, "bake1", usedAttributes, "defaultBlack");

            ConfigureDefaultBufferVec4(renderMesh, "colorSet1", usedAttributes, "defaultColorSet1");
            ConfigureDefaultBufferVec4(renderMesh, "colorSet3", usedAttributes, "defaultColorSet1");

            ConfigureDefaultBufferVec4(renderMesh, "colorSet2", usedAttributes, "defaultColorSet2");

            ConfigureDefaultBufferVec4(renderMesh, "colorSet5", usedAttributes, "defaultColorSet5");
        }

        private static void ConfigureDefaultBufferVec2(UltimateMesh renderMesh, string attributeName, HashSet<string> usedAttributes, string bufferName)
        {
            if (!usedAttributes.Contains(attributeName))
                renderMesh.ConfigureAttribute(new VertexFloatAttribute(attributeName, ValueCount.Two, VertexAttribPointerType.Float, false), bufferName, 0, sizeof(float) * 4);
        }

        private static void ConfigureDefaultBufferVec4(UltimateMesh renderMesh, string attributeName, HashSet<string> usedAttributes, string bufferName)
        {
            if (!usedAttributes.Contains(attributeName))
                renderMesh.ConfigureAttribute(new VertexFloatAttribute(attributeName, ValueCount.Four, VertexAttribPointerType.Float, false), bufferName, 0, sizeof(float) * 4);
        }

        private static Vector4[] CreateBuffer(MeshObject meshObject, Vector4 value)
        {
            var defaultColorSet = new Vector4[meshObject.VertexCount];
            for (int i = 0; i < defaultColorSet.Length; i++)
            {
                defaultColorSet[i] = value;
            }

            return defaultColorSet;
        }

        private static string ConfigureAttribute(UltimateMesh renderMesh, MeshObject meshObject, MeshAttribute attribute)
        {
            var name = attribute.AttributeStrings[0].Text;
            var valueCount = (ValueCount)UltimateVertexAttribute.GetAttributeFromName(name).ComponentCount;
            var type = GetGlAttributeType(attribute);
            var bufferName = $"vertexBuffer{attribute.BufferIndex}";
            var offset = (attribute.BufferIndex == 0 ? meshObject.VertexOffset : meshObject.VertexOffset2) + attribute.BufferOffset;
            var stride = attribute.BufferIndex == 0 ? meshObject.Stride : meshObject.Stride2;

            // Convert colors to floating point.
            var normalized = attribute.DataType == MeshAttribute.AttributeDataType.Byte;

            renderMesh.ConfigureAttribute(new VertexFloatAttribute(name, valueCount, type, normalized), bufferName, offset, stride);
            return name;
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
                // TODO: Some influences refer to bones that don't exist in the skeleton.
                // _eff bones?
                if (!indexByBoneName.ContainsKey(influence.BoneName))
                    continue;


                // HACK: Weird workaround to only select the first 4 influences per vertex.
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

        private static VertexAttribPointerType GetGlAttributeType(MeshAttribute meshAttribute)
        {
            // Render bytes as unsigned because of attribute normalization.
            return meshAttribute.DataType switch
            {
                MeshAttribute.AttributeDataType.Float => VertexAttribPointerType.Float,
                MeshAttribute.AttributeDataType.Byte => VertexAttribPointerType.UnsignedByte,
                MeshAttribute.AttributeDataType.HalfFloat => VertexAttribPointerType.HalfFloat,
                MeshAttribute.AttributeDataType.HalfFloat2 => VertexAttribPointerType.HalfFloat,
                _ => VertexAttribPointerType.Float,
            };
        }
    }
}
