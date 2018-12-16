using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;
using OpenTK;
using SFGenericModel.Materials;
using SFGraphics.GLObjects.Textures;

namespace CrossMod.Rendering.Models
{
    public class RMesh : IRenderable
    {
        public static Resources.DefaultTextures defaultTextures = null;

        public RenderMesh RenderMesh { get; set; } = null;

        public string Name { get; set; }

        public Vector4 BoundingSphere { get; set; }

        public DrawElementsType DrawElementType = DrawElementsType.UnsignedShort;

        public string SingleBindName { get; set; } = "";
        public int SingleBindIndex { get; set; } = -1;

        public Material Material { get; set; } = null;

        public bool Visible { get; set; } = true;

        public void Draw(Shader shader, Camera camera, RSkeleton skeleton)
        {
            if (!Visible)
                return;

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

            RenderMesh?.Draw(shader, camera);
        }

        private void SetTextureUniforms(Shader shader)
        {
            if (defaultTextures == null)
                defaultTextures = new Resources.DefaultTextures();

            var genericMaterial = CreateGenericMaterial(Material);

            genericMaterial.SetShaderUniforms(shader);
        }

        private GenericMaterial CreateGenericMaterial(Material material)
        {
            // Don't use the default texture unit.
            var genericMaterial = new GenericMaterial(1);

            AddTextures(material, genericMaterial);
            AddMaterialParams(material, genericMaterial);

            return genericMaterial;
        }

        private static void AddTextures(Material material, GenericMaterial genericMaterial)
        {
            AddMaterialTextures(material, genericMaterial);

            AddImageBasedLightingTextures(genericMaterial);

            AddRenderModeTextures(genericMaterial);
        }

        private static void AddMaterialParams(Material material, GenericMaterial genericMaterial)
        {
            // Set specific parameters and use a default value if not present.
            AddVec4("vec4Param", genericMaterial, material, RenderSettings.Instance.ParamId, new Vector4(0));

            // Assume no edge lighting if not present.
            AddVec4("paramA6", genericMaterial, material, 0xA6, new Vector4(0));

            // Some sort of skin subsurface color?
            AddVec4("paramA3", genericMaterial, material, 0xA3, new Vector4(1));

            // Sprite sheet UV parameters.
            AddVec4("paramAA", genericMaterial, material, 0xAA, new Vector4(1));

            // Enables/disables specular occlusion.
            AddBool("paramE9", genericMaterial, material, 0xE9, true);

            // Controls anisotropic specular.
            AddFloat("paramCA", genericMaterial, material, 0xCA, 0.0f);

            // Alpha offset.
            AddVec4("param98", genericMaterial, material, 0x98, new Vector4(1, 0, 0, 0));
        }

        private static void AddMaterialTextures(Material material, GenericMaterial genericMaterial)
        {
            // Use black for the default value.
            // Some materials seem to use emission as the main diffuse.
            AddTexture("colMap", material.col, genericMaterial);

            // Use the first texture for both layers if the second layer isn't present.
            if (material.HasCol2)
                genericMaterial.AddTexture("col2Map", material.col2);
            else
                genericMaterial.AddTexture("col2Map", material.col);

            AddTexture("prmMap", material.prm, genericMaterial);
            AddTexture("norMap", material.nor, genericMaterial);
            AddTexture("emiMap", material.emi, genericMaterial);
            AddTexture("bakeLitMap", material.bakeLit, genericMaterial);
            AddTexture("gaoMap", material.gao, genericMaterial);
        }

        private static void AddRenderModeTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("uvPattern", defaultTextures.uvPattern);
        }

        private static void AddImageBasedLightingTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("diffusePbrCube", defaultTextures.diffusePbr);
            genericMaterial.AddTexture("specularPbrCube", defaultTextures.specularPbr);
            genericMaterial.AddTexture("iblLut", defaultTextures.iblLut);
        }

        private static void AddTexture(string name, Texture texture, GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture(name, texture);
        }

        private static void AddBool(string name, GenericMaterial genericMaterial, Material source, long paramId, bool defaultValue)
        {
            if (source.boolByParamId.ContainsKey(paramId))
            {
                var value = source.boolByParamId[paramId];
                genericMaterial.AddBoolToInt(name, value);
            }
            else
            {
                genericMaterial.AddBoolToInt(name, defaultValue);
            }
        }

        private static void AddFloat(string name, GenericMaterial genericMaterial, Material source, long paramId, float defaultValue)
        {
            if (source.floatByParamId.ContainsKey(paramId))
            {
                var value = source.floatByParamId[paramId];
                genericMaterial.AddFloat(name, value);
            }
            else
            {
                genericMaterial.AddFloat(name, defaultValue);
            }
        }

        private static void AddVec4(string name, GenericMaterial genericMaterial, Material source, long paramId, Vector4 defaultValue)
        {
            if (source.vec4ByParamId.ContainsKey(paramId))
            {
                var value = source.vec4ByParamId[paramId];
                genericMaterial.AddVector4(name, value);
            }
            else if (source.boolByParamId.ContainsKey(paramId))
            {
                var value = source.boolByParamId[paramId];
                if (value)
                    genericMaterial.AddVector4(name, new Vector4(1, 0, 1, 0));
                else
                    genericMaterial.AddVector4(name, new Vector4(0, 0, 1, 0));
            }
            else if (source.floatByParamId.ContainsKey(paramId))
            {
                var value = source.floatByParamId[paramId];
                genericMaterial.AddVector4(name, new Vector4(value, value, value, 0));
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
