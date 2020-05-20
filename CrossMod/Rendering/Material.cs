using System.Collections.Generic;
using SFGraphics.GLObjects.Textures;
using SFGenericModel.Materials;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SSBHLib.Formats.Materials;

namespace CrossMod.Rendering
{
    public class Material
    {
        public string Name { get; set; }

        public float CurrentFrame { get; set; } = 0;

        public Resources.DefaultTextures defaultTextures;

        public BlendingFactor BlendSrc { get; set; } = BlendingFactor.One;
        public BlendingFactor BlendDst { get; set; } = BlendingFactor.Zero;
        public bool IsTransparent { get; set; } = false;

        public bool UseAlphaSampleCoverage { get; set; } = false;

        public Texture col = null;
        public bool HasCol { get; set; } = false;

        public Texture col2 = null;
        public bool HasCol2 { get; set; } = false;

        public Texture dif = null;
        public bool HasDiffuse { get; set; } = false;

        public Texture dif2 = null;
        public bool HasDiffuse2 { get; set; } = false;

        public Texture dif3 = null;
        public bool HasDiffuse3 { get; set; } = false;

        public Texture nor = null;
        public Texture prm = null;

        public Texture emi = null;
        public bool HasEmi { get; set; } = false;

        public Texture emi2 = null;
        public bool HasEmi2 { get; set; } = false;

        public Texture bakeLit = null;
        public Texture proj = null;
        public Texture gao = null;

        public Texture difCube = null;
        public bool HasDifCube { get; set; } = false;

        public Texture inkNor = null;
        public bool HasInkNorMap { get; set; } = false;

        public Texture specularCubeMap = null;

        public Dictionary<MatlEnums.ParamId, Vector4> vec4ByParamId = new Dictionary<MatlEnums.ParamId, Vector4>();
        public Dictionary<MatlEnums.ParamId, bool> boolByParamId = new Dictionary<MatlEnums.ParamId, bool>();
        public Dictionary<MatlEnums.ParamId, float> floatByParamId = new Dictionary<MatlEnums.ParamId, float>();

        public Dictionary<MatlEnums.ParamId, Vector4> MaterialAnimation { get; } = new Dictionary<MatlEnums.ParamId, Vector4>();

        public Material(Resources.DefaultTextures defaultTextures)
        {
            // TODO: Don't store another reference.
            this.defaultTextures = defaultTextures;

            // Ensure the textures are never null, so we can modify their state later.
            col = defaultTextures.defaultWhite;
            col2 = defaultTextures.defaultWhite;
            proj = defaultTextures.defaultBlack;
            nor = defaultTextures.defaultNormal;
            inkNor = defaultTextures.defaultWhite;
            prm = defaultTextures.defaultPrm;
            emi = defaultTextures.defaultBlack;
            emi2 = defaultTextures.defaultBlack;
            bakeLit = defaultTextures.defaultBlack;
            gao = defaultTextures.defaultWhite;
            specularCubeMap = defaultTextures.blackCube;
            difCube = defaultTextures.blackCube;
            dif = defaultTextures.defaultWhite;
            dif2 = defaultTextures.defaultWhite;
            dif3 = defaultTextures.defaultWhite;
        }

        public GenericMaterial CreateGenericMaterial(Material material)
        {
            // Don't use the default texture unit.
            var genericMaterial = new GenericMaterial(1);

            AddTextures(genericMaterial);

            // HACK: There isn't an easy way to access the current frame.
            genericMaterial.AddFloat("currentFrame", CurrentFrame);

            genericMaterial.AddBoolToInt("hasCustomVector44", vec4ByParamId.ContainsKey(MatlEnums.ParamId.CustomVector44));
            genericMaterial.AddBoolToInt("hasCustomVector47", vec4ByParamId.ContainsKey(MatlEnums.ParamId.CustomVector47));

            // TODO: Convert from quaternion values in light.nuanimb.
            AddQuaternion("chrLightDir", genericMaterial, -0.453154f, -0.365998f, -0.211309f, 0.784886f);

            return genericMaterial;
        }

        private static void AddQuaternion(string name, GenericMaterial genericMaterial, float x, float y, float z, float w)
        {
            var lightDirection = GetLightDirectionFromQuaternion(x, y, z, w);
            genericMaterial.AddVector3(name, lightDirection);
        }

        private static Vector3 GetLightDirectionFromQuaternion(float x, float y, float z, float w)
        {
            var quaternion = new Quaternion(x, y, z, w);
            var matrix = Matrix4.CreateFromQuaternion(quaternion);
            var lightDirection = Vector4.Transform(new Vector4(0, 0, 1, 0), matrix);
            return lightDirection.Normalized().Xyz;
        }

        private void AddTextures(GenericMaterial genericMaterial)
        {
            AddMaterialTextures(genericMaterial);

            AddImageBasedLightingTextures(genericMaterial);

            AddRenderModeTextures(genericMaterial);
        }

        public void AddDebugParams(UniformBlock uniformBlock)
        {
            // Set specific parameters and use a default value if not present.
            // TODO: Check if this cast is safe.
            AddVec4(uniformBlock, (MatlEnums.ParamId)RenderSettings.Instance.ParamId, new Vector4(0), true);
        }

        public void AddMaterialParams(UniformBlock uniformBlock)
        {
            // Assume no edge tint if not present.
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector14, Vector4.One);

            // Some sort of skin subsurface color?
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector11, Vector4.Zero);

            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector30, Vector4.Zero);

            // RGB color multiplier that affects all passes.
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector8, Vector4.One);

            // RGB diffuse pass color multiplier.
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector13, Vector4.One);

            // Sprite sheet UV parameters.
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector18, Vector4.One);

            // Color channels work like a PRM map.
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector47, Vector4.Zero);

            // TODO: ???
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean1, false);

            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean2, true);

            // Controls anisotropic specular.
            AddFloat(uniformBlock, MatlEnums.ParamId.CustomFloat10, 0.0f);

            // Controls specular tint.
            AddFloat(uniformBlock, MatlEnums.ParamId.CustomFloat8, 0.0f);

            // TODO: Refraction?
            AddFloat(uniformBlock, MatlEnums.ParamId.CustomFloat19, 0.0f);

            // TODO: du dv intensity?
            AddFloat(uniformBlock, MatlEnums.ParamId.CustomFloat4, 0.0f);

            // Some sort of sprite sheet scale toggle.
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean9, false);

            // Enables/disables UV scrolling animations.
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean5, false);
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean6, false);

            // Alpha offset?
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector0, Vector4.Zero);

            // UV transforms.
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector31, new Vector4(1, 1, 0, 0));
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector32, new Vector4(1, 1, 0, 0));

            // UV transform for emissive map layer 1.
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector6, new Vector4(1, 1, 0, 0));

            // Wii Fit trainer stage color.
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector44, Vector4.Zero);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector45, Vector4.Zero);

            // Some sort of emission color.
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector3, Vector4.One);
        }

        private void AddMaterialTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("colMap", col);
            genericMaterial.AddBoolToInt("hasColMap", HasCol);

            genericMaterial.AddTexture("col2Map", col2);
            genericMaterial.AddBoolToInt("hasCol2Map", HasCol2);

            genericMaterial.AddTexture("prmMap", prm);
            genericMaterial.AddTexture("norMap", nor);

            genericMaterial.AddTexture("inkNorMap", inkNor);
            genericMaterial.AddBoolToInt("hasInkNorMap", HasInkNorMap);

            genericMaterial.AddTexture("emiMap", emi);
            genericMaterial.AddTexture("emi2Map", emi2);

            genericMaterial.AddTexture("bakeLitMap", bakeLit);
            genericMaterial.AddTexture("gaoMap", gao);
            genericMaterial.AddTexture("projMap", proj);

            genericMaterial.AddTexture("difCubeMap", difCube);
            genericMaterial.AddBoolToInt("hasDifCubeMap", HasDifCube);

            genericMaterial.AddTexture("difMap", dif);
            genericMaterial.AddBoolToInt("hasDiffuse", HasDiffuse);

            genericMaterial.AddTexture("dif2Map", dif2);
            genericMaterial.AddBoolToInt("hasDiffuse2", HasDiffuse2);

            genericMaterial.AddTexture("dif3Map", dif3);
            genericMaterial.AddBoolToInt("hasDiffuse3", HasDiffuse3);

            // HACK: There's probably a better way to handle blending emission and base color maps.
            var hasDiffuseMaps = HasCol || HasCol2 || HasDiffuse || HasDiffuse2 || HasDiffuse3;
            var hasEmiMaps = HasEmi || HasEmi2;
            genericMaterial.AddBoolToInt("emissionOverride", hasEmiMaps && !hasDiffuseMaps);
        }

        private void AddRenderModeTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("uvPattern", defaultTextures.uvPattern);
        }

        private void AddImageBasedLightingTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("diffusePbrCube", defaultTextures.diffusePbr);
            genericMaterial.AddTexture("specularPbrCube", specularCubeMap);
            genericMaterial.AddTexture("iblLut", defaultTextures.iblLut);
        }

        private void AddBool(UniformBlock genericMaterial, MatlEnums.ParamId paramId, bool defaultValue)
        {
            var name = paramId.ToString();
            if (boolByParamId.ContainsKey(paramId))
            {
                var value = boolByParamId[paramId];
                genericMaterial.SetValue(name, value ? 1 : 0);
            }
            else
            {
                genericMaterial.SetValue(name, defaultValue ? 1 : 0);
            }
        }

        private void AddFloat(UniformBlock genericMaterial, MatlEnums.ParamId paramId, float defaultValue)
        {
            var name = paramId.ToString();
            if (floatByParamId.ContainsKey(paramId))
            {
                var value = floatByParamId[paramId];
                genericMaterial.SetValue(name, value);
            }
            else
            {
                genericMaterial.SetValue(name, defaultValue);
            }
        }

        private void AddVec4(UniformBlock uniformBlock, MatlEnums.ParamId paramId, Vector4 defaultValue, bool isDebug = false)
        {
            // Convert parameters into colors for easier visualization.
            var name = paramId.ToString();
            if (isDebug)
                name = "vec4Param";

            if (MaterialAnimation.ContainsKey(paramId))
            {
                var value = MaterialAnimation[paramId];
                uniformBlock.SetValue(name, value);
            }
            else if (vec4ByParamId.ContainsKey(paramId))
            {
                var value = vec4ByParamId[paramId];
                uniformBlock.SetValue(name, value);
            }
            else if (boolByParamId.ContainsKey(paramId))
            {
                var value = boolByParamId[paramId];
                if (value)
                    uniformBlock.SetValue(name, new Vector4(1, 0, 1, 0));
                else
                    uniformBlock.SetValue(name, new Vector4(0, 0, 1, 0));
            }
            else if (floatByParamId.ContainsKey(paramId))
            {
                var value = floatByParamId[paramId];
                uniformBlock.SetValue(name, new Vector4(value, value, value, 0));
            }
            else
            {
                uniformBlock.SetValue(name, defaultValue);
            }
        }
    }
}
