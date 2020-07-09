using System.Collections.Generic;
using SFGraphics.GLObjects.Textures;
using SFGenericModel.Materials;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SSBHLib.Formats.Materials;
using SFGraphics.GLObjects.Samplers;
using CrossMod.Rendering.Resources;
using SFGraphics.GLObjects.Shaders;

namespace CrossMod.Rendering.GlTools
{
    /// <summary>
    /// Stores <see cref="MatlEntry"/> material values as OpenGL uniforms and render state.
    /// </summary>
    public class Material
    {
        public string Name { get; set; }

        private GenericMaterial genericMaterial = null;
        private UniformBlock uniformBlock = null;

        public float CurrentFrame { get; set; } = 0;

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

        // TODO: Just expose public get/update methods to simplify keeping track of changes.
        public Dictionary<MatlEnums.ParamId, Vector4> vec4ByParamId = new Dictionary<MatlEnums.ParamId, Vector4>();
        public Dictionary<MatlEnums.ParamId, bool> boolByParamId = new Dictionary<MatlEnums.ParamId, bool>();
        public Dictionary<MatlEnums.ParamId, float> floatByParamId = new Dictionary<MatlEnums.ParamId, float>();

        // Add a flag to ensure the uniforms get updated for rendering.
        // TODO: Updating the uniform block from the update methods doesn't work.
        // TODO: The performance impact is negligible, but not all uniforms need to be updated at once
        private bool shouldUpdateUniformBlock = false;

        public void UpdateVec4(MatlEnums.ParamId paramId, Vector4 value)
        {
            vec4ByParamId[paramId] = value;
            shouldUpdateUniformBlock = true;
        }

        public void UpdateFloat(MatlEnums.ParamId paramId, float value)
        {
            floatByParamId[paramId] = value;
            shouldUpdateUniformBlock = true;
        }

        public void UpdateBoolean(MatlEnums.ParamId paramId, bool value)
        {
            boolByParamId[paramId] = value;
            shouldUpdateUniformBlock = true;
        }

        public Dictionary<MatlEnums.ParamId, Vector4> Vec4ParamsMaterialAnimation { get; } = new Dictionary<MatlEnums.ParamId, Vector4>();

        public Material()
        {
            // Ensure the textures are never null, so we can modify their state later.
            col = DefaultTextures.Instance.DefaultWhite;
            col2 = DefaultTextures.Instance.DefaultWhite;
            proj = DefaultTextures.Instance.DefaultBlack;
            nor = DefaultTextures.Instance.DefaultNormal;
            inkNor = DefaultTextures.Instance.DefaultWhite;
            prm = DefaultTextures.Instance.DefaultPrm;
            emi = DefaultTextures.Instance.DefaultBlack;
            emi2 = DefaultTextures.Instance.DefaultBlack;
            bakeLit = DefaultTextures.Instance.DefaultWhite;
            gao = DefaultTextures.Instance.DefaultWhite;
            specularCubeMap = DefaultTextures.Instance.BlackCube;
            difCube = DefaultTextures.Instance.BlackCube;
            dif = DefaultTextures.Instance.DefaultWhite;
            dif2 = DefaultTextures.Instance.DefaultWhite;
            dif3 = DefaultTextures.Instance.DefaultWhite;
        }

        public void SetMaterialUniforms(Shader shader, Material previousMaterial)
        {
            // TODO: The uniform block and generic material should also be updated when the uniform values are changed.
            if (genericMaterial == null)
                genericMaterial = CreateGenericMaterial();

            if (uniformBlock == null)
            {
                uniformBlock = new UniformBlock(shader, "MaterialParams") { BlockBinding = 1 };
                SetMaterialParams(uniformBlock);
            }

            if (shouldUpdateUniformBlock)
            {
                SetMaterialParams(uniformBlock);
                shouldUpdateUniformBlock = false;
            }

            // This needs to be updated more than once.
            AddDebugParams(uniformBlock);
            
            // TODO: The above code could be moved to the constructor.
            genericMaterial.SetShaderUniforms(shader, previousMaterial?.genericMaterial);
            uniformBlock.BindBlock(shader);
        }

        public void SetRenderState()
        {
            var alphaBlendSettings = new SFGenericModel.RenderState.AlphaBlendSettings(true, BlendSrc, BlendDst, BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
            SFGenericModel.RenderState.GLRenderSettings.SetAlphaBlending(alphaBlendSettings);

            // Meshes with screen door transparency enable this OpenGL extension.
            if (RenderSettings.Instance.EnableExperimental && UseAlphaSampleCoverage)
                GL.Enable(EnableCap.SampleAlphaToCoverage);
            else
                GL.Disable(EnableCap.SampleAlphaToCoverage);

            SFGenericModel.RenderState.GLRenderSettings.SetFaceCulling(new SFGenericModel.RenderState.FaceCullingSettings(true, CullMode));
        }

        private GenericMaterial CreateGenericMaterial()
        {
            // Don't use the default texture unit.
            var genericMaterial = new GenericMaterial(1);

            AddTextures(genericMaterial);

            // HACK: There isn't an easy way to access the current frame.
            genericMaterial.AddFloat("currentFrame", CurrentFrame);
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

        private void AddDebugParams(UniformBlock uniformBlock)
        {
            // Set specific parameters and use a default value if not present.
            // TODO: Check if this cast is safe.
            SetVec4(uniformBlock, (MatlEnums.ParamId)RenderSettings.Instance.ParamId, new Vector4(0), true);
        }

        private void SetMaterialParams(UniformBlock uniformBlock)
        {
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector0, Vector4.Zero);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector3, Vector4.One);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector6, new Vector4(1, 1, 0, 0));
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector8, Vector4.One);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector11, Vector4.Zero);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector13, Vector4.One);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector14, Vector4.One);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector18, Vector4.One);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector30, Vector4.Zero);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector31, new Vector4(1, 1, 0, 0));
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector32, new Vector4(1, 1, 0, 0));
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector44, Vector4.Zero);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector45, Vector4.Zero);
            SetVec4(uniformBlock, MatlEnums.ParamId.CustomVector47, Vector4.Zero);

            SetBool(uniformBlock, MatlEnums.ParamId.CustomBoolean1, false);
            SetBool(uniformBlock, MatlEnums.ParamId.CustomBoolean2, true);
            SetBool(uniformBlock, MatlEnums.ParamId.CustomBoolean3, true);
            SetBool(uniformBlock, MatlEnums.ParamId.CustomBoolean4, true);
            SetBool(uniformBlock, MatlEnums.ParamId.CustomBoolean5, false);
            SetBool(uniformBlock, MatlEnums.ParamId.CustomBoolean6, false);
            SetBool(uniformBlock, MatlEnums.ParamId.CustomBoolean9, false);
            SetBool(uniformBlock, MatlEnums.ParamId.CustomBoolean11, true);

            SetFloat(uniformBlock, MatlEnums.ParamId.CustomFloat1, 0.0f);
            SetFloat(uniformBlock, MatlEnums.ParamId.CustomFloat4, 0.0f);
            SetFloat(uniformBlock, MatlEnums.ParamId.CustomFloat8, 0.0f);
            SetFloat(uniformBlock, MatlEnums.ParamId.CustomFloat10, 0.0f);
            SetFloat(uniformBlock, MatlEnums.ParamId.CustomFloat19, 0.0f);

            uniformBlock.SetValue("hasCustomVector11", vec4ByParamId.ContainsKey(MatlEnums.ParamId.CustomVector11));
            uniformBlock.SetValue("hasCustomVector44", vec4ByParamId.ContainsKey(MatlEnums.ParamId.CustomVector44));
            uniformBlock.SetValue("hasCustomVector47", vec4ByParamId.ContainsKey(MatlEnums.ParamId.CustomVector47));
            uniformBlock.SetValue("hasCustomFloat10", floatByParamId.ContainsKey(MatlEnums.ParamId.CustomFloat10));
            uniformBlock.SetValue("hasCustomBoolean1", boolByParamId.ContainsKey(MatlEnums.ParamId.CustomBoolean1));

            uniformBlock.SetValue("hasColMap", HasCol);
            uniformBlock.SetValue("hasCol2Map", HasCol2);
            uniformBlock.SetValue("hasInkNorMap", HasInkNorMap);
            uniformBlock.SetValue("hasDifCubeMap", HasDifCube);
            uniformBlock.SetValue("hasDiffuse", HasDiffuse);
            uniformBlock.SetValue("hasDiffuse2", HasDiffuse2);
            uniformBlock.SetValue("hasDiffuse3", HasDiffuse3);

            // HACK: There's probably a better way to handle blending emission and base color maps.
            var hasDiffuseMaps = HasCol || HasCol2 || HasDiffuse || HasDiffuse2 || HasDiffuse3;
            var hasEmiMaps = HasEmi || HasEmi2;
            uniformBlock.SetValue("emissionOverride", hasEmiMaps && !hasDiffuseMaps);
        }

        private void AddMaterialTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("colMap", col, colSampler);
            genericMaterial.AddTexture("col2Map", col2, col2Sampler);
            genericMaterial.AddTexture("prmMap", prm, prmSampler);
            genericMaterial.AddTexture("norMap", nor, norSampler);
            genericMaterial.AddTexture("inkNorMap", inkNor, inkNorSampler);
            genericMaterial.AddTexture("emiMap", emi, emiSampler);
            genericMaterial.AddTexture("emi2Map", emi2, emi2Sampler);
            genericMaterial.AddTexture("bakeLitMap", bakeLit, bakeLitSampler);
            genericMaterial.AddTexture("gaoMap", gao, gaoSampler);
            genericMaterial.AddTexture("projMap", proj, projSampler);
            genericMaterial.AddTexture("difCubeMap", difCube, difCubeSampler);
            genericMaterial.AddTexture("difMap", dif, difSampler);
            genericMaterial.AddTexture("dif2Map", dif2, dif2Sampler);
            genericMaterial.AddTexture("dif3Map", dif3, dif3Sampler);
        }

        private void AddRenderModeTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("uvPattern", DefaultTextures.Instance.UvPattern);
        }

        private void AddImageBasedLightingTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("diffusePbrCube", DefaultTextures.Instance.DiffusePbr);
            genericMaterial.AddTexture("specularPbrCube", specularCubeMap);
        }

        private void SetBool(UniformBlock uniformBlock, MatlEnums.ParamId paramId, bool defaultValue)
        {
            var name = paramId.ToString();
            if (boolByParamId.ContainsKey(paramId))
            {
                var value = boolByParamId[paramId];
                uniformBlock.SetValue(name, value ? 1 : 0);
            }
            else
            {
                uniformBlock.SetValue(name, defaultValue ? 1 : 0);
            }
        }

        private void SetFloat(UniformBlock uniformBlock, MatlEnums.ParamId paramId, float defaultValue)
        {
            var name = paramId.ToString();
            if (floatByParamId.ContainsKey(paramId))
            {
                var value = floatByParamId[paramId];
                uniformBlock.SetValue(name, value);
            }
            else
            {
                uniformBlock.SetValue(name, defaultValue);
            }
        }

        private void SetVec4(UniformBlock uniformBlock, MatlEnums.ParamId paramId, Vector4 defaultValue, bool isDebug = false)
        {
            // Convert parameters into colors for easier visualization.
            var name = paramId.ToString();
            if (isDebug)
                name = "vec4Param";

            if (Vec4ParamsMaterialAnimation.ContainsKey(paramId))
            {
                var value = Vec4ParamsMaterialAnimation[paramId];
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
