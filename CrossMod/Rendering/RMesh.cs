using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;
using OpenTK;

namespace CrossMod.Rendering
{
    public class RMesh : IRenderable
    {
        private static Resources.DefaultTextures defaultTextures = null;

        public string Name { get; set; }

        public DrawElementsType DrawElementType = DrawElementsType.UnsignedShort;

        public List<CustomVertexAttribute> VertexAttributes = new List<CustomVertexAttribute>();

        public int IndexOffset { get; set; }
        public int IndexCount { get; set; }

        public string SingleBindName { get; set; } = "";
        public int SingleBindIndex { get; set; } = -1;

        public Material Material { get; set; } = null;

        public bool Visible { get; set; } = true;

        public void Draw(Shader shader, Camera camera, RSkeleton skeleton)
        {
            if (!Visible) return;
            if (skeleton != null)
            {
                var matrix = Matrix4.Identity;
                if (SingleBindIndex >= 0)
                    matrix = skeleton.GetAnimationSingleBindsTransform(SingleBindIndex);
                shader.SetMatrix4x4("transform", ref matrix);
            }

            if (Material != null)
            {
                SetTextureUniforms(shader);
            }
            foreach (CustomVertexAttribute a in VertexAttributes)
            {
                a.Bind(shader);
            }

            GL.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementType, IndexOffset);
        }

        private void SetTextureUniforms(Shader shader)
        {
            if (defaultTextures == null)
                defaultTextures = new Resources.DefaultTextures();

            var genericMaterial = CreateGenericMaterial(Material);

            genericMaterial.SetShaderUniforms(shader);
        }

        private SFGenericModel.Materials.GenericMaterial CreateGenericMaterial(Material material)
        {
            // Don't use the default texture unit.
            var genericMaterial = new SFGenericModel.Materials.GenericMaterial(1);

            // Use black for the default value.
            // Some materials seemt to use emission as the main diffuse.
            if (material.col != null)
                genericMaterial.AddTexture("colMap", material.col);
            else
                genericMaterial.AddTexture("colMap", defaultTextures.defaultBlack);

            // Use the first texture for both layers if the second layer isn't present.
            if (material.col2 != null)
                genericMaterial.AddTexture("col2Map", material.col2);
            else if (material.col != null)
                genericMaterial.AddTexture("col2Map", material.col);
            else
                genericMaterial.AddTexture("col2Map", defaultTextures.defaultBlack);

            if (material.prm != null)
                genericMaterial.AddTexture("prmMap", material.prm);
            else
                genericMaterial.AddTexture("prmMap", defaultTextures.defaultPrm);

            if (material.nor != null)
                genericMaterial.AddTexture("norMap", material.nor);
            else
                genericMaterial.AddTexture("norMap", defaultTextures.defaultNormal);

            if (material.emi != null)
                genericMaterial.AddTexture("emiMap", material.emi);
            else
                genericMaterial.AddTexture("emiMap", defaultTextures.defaultBlack);

            if (material.bakeLit != null)
                genericMaterial.AddTexture("bakeLitMap", material.bakeLit);
            else
                genericMaterial.AddTexture("bakeLitMap", defaultTextures.defaultWhite);

            genericMaterial.AddTexture("diffusePbrCube", defaultTextures.diffusePbr);
            genericMaterial.AddTexture("specularPbrCube", defaultTextures.specularPbr);
            genericMaterial.AddTexture("iblLut", defaultTextures.iblLut);
            genericMaterial.AddTexture("uvPattern", defaultTextures.uvPattern);

            // Set specific parameters and use a default value if not present.
            AddMtalVec4("vec4Param", genericMaterial, material, RenderSettings.Instance.paramId, new Vector4(0));
            AddMtalVec4("paramA6", genericMaterial, material, 0xA6, new Vector4(1));
            AddMtalVec4("param98", genericMaterial, material, 0x98, new Vector4(1, 0, 0, 0));

            return genericMaterial;
        }

        private static void AddMtalVec4(string name, SFGenericModel.Materials.GenericMaterial genericMaterial, Material source, long paramId, Vector4 defaultValue)
        {
            if (source.vec4ByParamId.ContainsKey(paramId))
            {
                var value = source.vec4ByParamId[paramId];
                genericMaterial.AddVector4(name, new Vector4(value.X, value.Y, value.Z, value.W));
            }
            else
            {
                genericMaterial.AddVector4(name, defaultValue);
            }
        }

        public void Render(Camera Camera)
        {

        }
    }
}
