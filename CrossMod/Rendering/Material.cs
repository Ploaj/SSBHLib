using System.Collections.Generic;
using SFGraphics.GLObjects.Textures;
using SFGenericModel.Materials;
using OpenTK;

namespace CrossMod.Rendering
{
    public class Material
    {
        public string Name;

        public Resources.DefaultTextures defaultTextures;
        public Texture col = null;

        public Texture col2 = null;
        public bool HasCol2 { get; set; } = false;

        public Texture dif = null;
        public bool HasDiffuse { get; set; } = false;

        public Texture nor = null;

        public Texture prm = null;

        public Texture emi = null;

        public Texture emi2 = null;
        public bool HasEmi2 { get; set; } = false;

        public Texture bakeLit = null;

        public Texture proj = null;

        public Texture gao = null;

        public Texture difCube = null;

        public Texture inkNor = null;

        public TextureCubeMap specularIbl = null;

        public Dictionary<long, Vector4> vec4ByParamId = new Dictionary<long, Vector4>();
        public Dictionary<long, bool> boolByParamId = new Dictionary<long, bool>();
        public Dictionary<long, float> floatByParamId = new Dictionary<long, float>();

        public Material(Resources.DefaultTextures defaultTextures)
        {
            // TODO: Don't store another reference.
            this.defaultTextures = defaultTextures;

            // Ensure the textures are never null, so we can modify their state later.
            col = defaultTextures.defaultBlack;
            col2 = defaultTextures.defaultBlack;
            proj = defaultTextures.defaultBlack;
            nor = defaultTextures.defaultNormal;
            inkNor = defaultTextures.defaultWhite;
            prm = defaultTextures.defaultPrm;
            emi = defaultTextures.defaultBlack;
            emi2 = defaultTextures.defaultBlack;
            bakeLit = defaultTextures.defaultBlack;
            gao = defaultTextures.defaultWhite;
            specularIbl = defaultTextures.blackCube;
            difCube = defaultTextures.defaultBlack;
            dif = defaultTextures.defaultBlack;
        }

        public GenericMaterial CreateGenericMaterial(Material material)
        {
            // Don't use the default texture unit.
            var genericMaterial = new GenericMaterial(1);

            AddTextures(genericMaterial);
            AddMaterialParams(genericMaterial);

            return genericMaterial;
        }

        private void AddTextures(GenericMaterial genericMaterial)
        {
            AddMaterialTextures(genericMaterial);

            AddImageBasedLightingTextures(genericMaterial);

            AddRenderModeTextures(genericMaterial);
        }

        private void AddMaterialParams(GenericMaterial genericMaterial)
        {
            // Set specific parameters and use a default value if not present.
            AddVec4("vec4Param", genericMaterial, RenderSettings.Instance.ParamId, new Vector4(0));

            // Assume no edge lighting if not present.
            AddVec4("paramA6", genericMaterial, 0xA6, new Vector4(0));

            // Some sort of skin subsurface color?
            AddVec4("paramA3", genericMaterial, 0xA3, new Vector4(1));

            // Mario Galaxy rim light?
            AddVec4("paramA0", genericMaterial, 0xA0, new Vector4(1));

            // Diffuse color multiplier?
            AddVec4("paramA5", genericMaterial, 0xA5, new Vector4(1));

            // Sprite sheet UV parameters.
            AddVec4("paramAA", genericMaterial, 0xAA, new Vector4(1));

            // Enables/disables specular occlusion.
            AddBool("paramE9", genericMaterial, 0xE9, true);

            // Controls anisotropic specular.
            AddFloat("paramCA", genericMaterial, 0xCA, 0.0f);

            // Some sort of sprite sheet scale toggle.
            AddBool("paramF1", genericMaterial, 0xF1, true);

            // Alpha offset.
            AddVec4("param98", genericMaterial, 0x98, new Vector4(0, 0, 0, 0));
        }

        private void AddMaterialTextures(GenericMaterial genericMaterial)
        {
            // Use black for the default value.
            // Some materials seem to use emission as the main diffuse.
            genericMaterial.AddTexture("colMap", col);

            // Use the first texture for both layers if the second layer isn't present.
            if (HasCol2)
                genericMaterial.AddTexture("col2Map", col2);
            else
                genericMaterial.AddTexture("col2Map", col);

            genericMaterial.AddTexture("prmMap", prm);
            genericMaterial.AddTexture("norMap", nor);
            genericMaterial.AddTexture("inkNorMap", inkNor);

            genericMaterial.AddTexture("emiMap", emi);
            if (HasEmi2)
                genericMaterial.AddTexture("emi2Map", emi2);
            else
                genericMaterial.AddTexture("emi2Map", emi);

            genericMaterial.AddTexture("bakeLitMap", bakeLit);
            genericMaterial.AddTexture("gaoMap", gao);
            genericMaterial.AddTexture("projMap", proj);
            genericMaterial.AddTexture("difCubemap", difCube);

            genericMaterial.AddTexture("difMap", dif);
            genericMaterial.AddBoolToInt("hasDiffuse", HasDiffuse);
        }

        private void AddRenderModeTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("uvPattern", defaultTextures.uvPattern);
        }

        private void AddImageBasedLightingTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("diffusePbrCube", defaultTextures.diffusePbr);
            genericMaterial.AddTexture("specularPbrCube", specularIbl);
            genericMaterial.AddTexture("iblLut", defaultTextures.iblLut);
        }

        private void AddBool(string name, GenericMaterial genericMaterial, long paramId, bool defaultValue)
        {
            if (boolByParamId.ContainsKey(paramId))
            {
                var value = boolByParamId[paramId];
                genericMaterial.AddBoolToInt(name, value);
            }
            else
            {
                genericMaterial.AddBoolToInt(name, defaultValue);
            }
        }

        private void AddFloat(string name, GenericMaterial genericMaterial, long paramId, float defaultValue)
        {
            if (floatByParamId.ContainsKey(paramId))
            {
                var value = floatByParamId[paramId];
                genericMaterial.AddFloat(name, value);
            }
            else
            {
                genericMaterial.AddFloat(name, defaultValue);
            }
        }

        private void AddVec4(string name, GenericMaterial genericMaterial, long paramId, Vector4 defaultValue)
        {
            // Convert parameters into colors for easier visualization.
            if (vec4ByParamId.ContainsKey(paramId))
            {
                var value = vec4ByParamId[paramId];
                genericMaterial.AddVector4(name, value);
            }
            else if (boolByParamId.ContainsKey(paramId))
            {
                var value = boolByParamId[paramId];
                if (value)
                    genericMaterial.AddVector4(name, new Vector4(1, 0, 1, 0));
                else
                    genericMaterial.AddVector4(name, new Vector4(0, 0, 1, 0));
            }
            else if (floatByParamId.ContainsKey(paramId))
            {
                var value = floatByParamId[paramId];
                genericMaterial.AddVector4(name, new Vector4(value, value, value, 0));
            }
            else
            {
                genericMaterial.AddVector4(name, defaultValue);
            }
        }
    }
}
