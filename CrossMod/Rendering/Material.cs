using System.Collections.Generic;
using SFGraphics.GLObjects.Textures;
using SFGenericModel.Materials;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SSBHLib.Formats.Materials;
using SFGraphics.GLObjects.Samplers;

namespace CrossMod.Rendering
{
    public class Material
    {
        public string Name { get; set; }

        public float CurrentFrame { get; set; } = 0;

        public Resources.DefaultTextures defaultTextures;

        public CullFaceMode CullMode { get; set; } = CullFaceMode.Back;

        public BlendingFactor BlendSrc { get; set; } = BlendingFactor.One;
        public BlendingFactor BlendDst { get; set; } = BlendingFactor.Zero;
        public bool IsTransparent { get; set; } = false;

        public bool UseAlphaSampleCoverage { get; set; } = false;

        public SamplerObject colSampler = new SamplerObject();
        public Texture col;
        public bool HasCol { get; set; } = false;

        public SamplerObject col2Sampler = new SamplerObject();
        public Texture col2;
        public bool HasCol2 { get; set; } = false;

        public SamplerObject difSampler;
        public Texture dif;
        public bool HasDiffuse { get; set; } = false;

        public SamplerObject dif2Sampler = new SamplerObject();
        public Texture dif2;
        public bool HasDiffuse2 { get; set; } = false;

        public SamplerObject dif3Sampler = new SamplerObject();
        public Texture dif3 = null;
        public bool HasDiffuse3 { get; set; } = false;

        public SamplerObject norSampler = new SamplerObject();
        public Texture nor = null;

        public SamplerObject prmSampler = new SamplerObject();
        public Texture prm = null;

        public SamplerObject emiSampler = new SamplerObject();
        public Texture emi = null;
        public bool HasEmi { get; set; } = false;

        public SamplerObject emi2Sampler = new SamplerObject();
        public Texture emi2 = null;
        public bool HasEmi2 { get; set; } = false;

        public SamplerObject bakeLitSampler = new SamplerObject();
        public Texture bakeLit = null;

        public SamplerObject projSampler = new SamplerObject();
        public Texture proj = null;

        public SamplerObject gaoSampler = new SamplerObject();
        public Texture gao = null;

        public SamplerObject difCubeSampler = new SamplerObject();
        public Texture difCube = null;
        public bool HasDifCube { get; set; } = false;

        public SamplerObject inkNorSampler = new SamplerObject();
        public Texture inkNor = null;
        public bool HasInkNorMap { get; set; } = false;

        public SamplerObject specualrCubeSampler = new SamplerObject();
        public Texture specularCubeMap = null;

        public float DepthBias { get; set; } = 0.0f;

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
            bakeLit = defaultTextures.defaultWhite;
            gao = defaultTextures.defaultWhite;
            specularCubeMap = defaultTextures.blackCube;
            difCube = defaultTextures.blackCube;
            dif = defaultTextures.defaultWhite;
            dif2 = defaultTextures.defaultWhite;
            dif3 = defaultTextures.defaultWhite;
        }

        public GenericMaterial CreateGenericMaterial()
        {
            // Don't use the default texture unit.
            var genericMaterial = new GenericMaterial(1);

            AddTextures(genericMaterial);

            // HACK: There isn't an easy way to access the current frame.
            genericMaterial.AddFloat("currentFrame", CurrentFrame);

            genericMaterial.AddBoolToInt("hasCustomVector11", vec4ByParamId.ContainsKey(MatlEnums.ParamId.CustomVector11));
            genericMaterial.AddBoolToInt("hasCustomVector44", vec4ByParamId.ContainsKey(MatlEnums.ParamId.CustomVector44));
            genericMaterial.AddBoolToInt("hasCustomVector47", vec4ByParamId.ContainsKey(MatlEnums.ParamId.CustomVector47));
            genericMaterial.AddBoolToInt("hasCustomFloat10", floatByParamId.ContainsKey(MatlEnums.ParamId.CustomFloat10));
            genericMaterial.AddBoolToInt("hasCustomBoolean1", boolByParamId.ContainsKey(MatlEnums.ParamId.CustomBoolean1));

            genericMaterial.AddFloat("depthBias", DepthBias);

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
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector0, Vector4.Zero);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector3, Vector4.One);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector6, new Vector4(1, 1, 0, 0));
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector8, Vector4.One);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector11, Vector4.Zero);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector13, Vector4.One);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector14, Vector4.One);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector18, Vector4.One);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector30, Vector4.Zero);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector31, new Vector4(1, 1, 0, 0));
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector32, new Vector4(1, 1, 0, 0));
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector44, Vector4.Zero);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector45, Vector4.Zero);
            AddVec4(uniformBlock, MatlEnums.ParamId.CustomVector47, Vector4.Zero);

            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean1, false);
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean2, true);
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean3, true);
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean4, true);
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean5, false);
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean6, false);
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean9, false);
            AddBool(uniformBlock, MatlEnums.ParamId.CustomBoolean11, true);

            AddFloat(uniformBlock, MatlEnums.ParamId.CustomFloat1, 0.0f);
            AddFloat(uniformBlock, MatlEnums.ParamId.CustomFloat4, 0.0f);
            AddFloat(uniformBlock, MatlEnums.ParamId.CustomFloat8, 0.0f);
            AddFloat(uniformBlock, MatlEnums.ParamId.CustomFloat10, 0.0f);
            AddFloat(uniformBlock, MatlEnums.ParamId.CustomFloat19, 0.0f);
        }

        private void AddMaterialTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("colMap", col, colSampler);
            genericMaterial.AddBoolToInt("hasColMap", HasCol);

            genericMaterial.AddTexture("col2Map", col2, col2Sampler);
            genericMaterial.AddBoolToInt("hasCol2Map", HasCol2);

            genericMaterial.AddTexture("prmMap", prm, prmSampler);
            genericMaterial.AddTexture("norMap", nor, norSampler);

            genericMaterial.AddTexture("inkNorMap", inkNor, inkNorSampler);
            genericMaterial.AddBoolToInt("hasInkNorMap", HasInkNorMap);

            genericMaterial.AddTexture("emiMap", emi, emiSampler);
            genericMaterial.AddTexture("emi2Map", emi2, emi2Sampler);

            genericMaterial.AddTexture("bakeLitMap", bakeLit, bakeLitSampler);
            genericMaterial.AddTexture("gaoMap", gao, gaoSampler);
            genericMaterial.AddTexture("projMap", proj, projSampler);

            genericMaterial.AddTexture("difCubeMap", difCube, difCubeSampler);
            genericMaterial.AddBoolToInt("hasDifCubeMap", HasDifCube);

            genericMaterial.AddTexture("difMap", dif, difSampler);
            genericMaterial.AddBoolToInt("hasDiffuse", HasDiffuse);

            genericMaterial.AddTexture("dif2Map", dif2, dif2Sampler);
            genericMaterial.AddBoolToInt("hasDiffuse2", HasDiffuse2);

            genericMaterial.AddTexture("dif3Map", dif3, dif3Sampler);
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
