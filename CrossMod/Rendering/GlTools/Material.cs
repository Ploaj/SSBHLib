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

        private readonly Dictionary<MatlEnums.ParamId, Texture> defaultTextureByParamId = new Dictionary<MatlEnums.ParamId, Texture>
        {
            { MatlEnums.ParamId.Texture0, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture1, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture3, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture4, DefaultTextures.Instance.DefaultNormal },
            { MatlEnums.ParamId.Texture5, DefaultTextures.Instance.DefaultBlack },
            { MatlEnums.ParamId.Texture6, DefaultTextures.Instance.DefaultPrm },
            { MatlEnums.ParamId.Texture7, DefaultTextures.Instance.BlackCube },
            { MatlEnums.ParamId.Texture8, DefaultTextures.Instance.BlackCube },
            { MatlEnums.ParamId.Texture9, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture10, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture11, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture12, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture13, DefaultTextures.Instance.DefaultBlack },
            { MatlEnums.ParamId.Texture14, DefaultTextures.Instance.DefaultBlack },
            { MatlEnums.ParamId.Texture16, DefaultTextures.Instance.DefaultWhite },
        };

        // The parameters don't matter because the default texture are solid color.
        private readonly SamplerObject defaultSampler = new SamplerObject();

        public Dictionary<string, Texture> TextureByName { get; set; } = new Dictionary<string, Texture>();

        public float CurrentFrame { get; set; } = 0;

        public CullFaceMode CullMode { get; set; } = CullFaceMode.Back;

        public BlendingFactor BlendSrc { get; set; } = BlendingFactor.One;
        public BlendingFactor BlendDst { get; set; } = BlendingFactor.Zero;
        public bool IsTransparent { get; set; } = false;

        public bool UseAlphaSampleCoverage { get; set; } = false;

        public float DepthBias { get; set; } = 0.0f;

        public bool HasCol => textureNameByParamId.ContainsKey(MatlEnums.ParamId.Texture0);
        public bool HasCol2 => textureNameByParamId.ContainsKey(MatlEnums.ParamId.Texture1);
        public bool HasInkNorMap => textureNameByParamId.ContainsKey(MatlEnums.ParamId.Texture16);
        public bool HasDifCube => textureNameByParamId.ContainsKey(MatlEnums.ParamId.Texture8);
        public bool HasDiffuse => textureNameByParamId.ContainsKey(MatlEnums.ParamId.Texture10);
        public bool HasDiffuse2 => textureNameByParamId.ContainsKey(MatlEnums.ParamId.Texture11);
        public bool HasDiffuse3 => textureNameByParamId.ContainsKey(MatlEnums.ParamId.Texture12);
        public bool HasEmi => textureNameByParamId.ContainsKey(MatlEnums.ParamId.Texture5);
        public bool HasEmi2 => textureNameByParamId.ContainsKey(MatlEnums.ParamId.Texture14);

        // TODO: Just expose public get/update methods to simplify keeping track of changes.
        public Dictionary<MatlEnums.ParamId, Vector4> vec4ByParamId = new Dictionary<MatlEnums.ParamId, Vector4>();
        public Dictionary<MatlEnums.ParamId, bool> boolByParamId = new Dictionary<MatlEnums.ParamId, bool>();
        public Dictionary<MatlEnums.ParamId, float> floatByParamId = new Dictionary<MatlEnums.ParamId, float>();

        public Dictionary<MatlEnums.ParamId, string> textureNameByParamId = new Dictionary<MatlEnums.ParamId, string>();
        public Dictionary<MatlEnums.ParamId, SamplerObject> samplerByParamId = new Dictionary<MatlEnums.ParamId, SamplerObject>();

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
            genericMaterial.AddTexture("colMap", GetTexture(MatlEnums.ParamId.Texture0), GetSampler(MatlEnums.ParamId.Sampler0));
            genericMaterial.AddTexture("col2Map", GetTexture(MatlEnums.ParamId.Texture1), GetSampler(MatlEnums.ParamId.Sampler1));
            genericMaterial.AddTexture("prmMap", GetTexture(MatlEnums.ParamId.Texture6), GetSampler(MatlEnums.ParamId.Sampler6));
            genericMaterial.AddTexture("norMap", GetTexture(MatlEnums.ParamId.Texture4), GetSampler(MatlEnums.ParamId.Sampler4));
            genericMaterial.AddTexture("inkNorMap", GetTexture(MatlEnums.ParamId.Texture16), GetSampler(MatlEnums.ParamId.Sampler16));
            genericMaterial.AddTexture("emiMap", GetTexture(MatlEnums.ParamId.Texture5), GetSampler(MatlEnums.ParamId.Sampler5));
            genericMaterial.AddTexture("emi2Map", GetTexture(MatlEnums.ParamId.Texture14), GetSampler(MatlEnums.ParamId.Sampler14));
            genericMaterial.AddTexture("bakeLitMap", GetTexture(MatlEnums.ParamId.Texture9), GetSampler(MatlEnums.ParamId.Sampler9));
            genericMaterial.AddTexture("gaoMap", GetTexture(MatlEnums.ParamId.Texture3), GetSampler(MatlEnums.ParamId.Sampler3));
            genericMaterial.AddTexture("projMap", GetTexture(MatlEnums.ParamId.Texture13), GetSampler(MatlEnums.ParamId.Sampler13));
            genericMaterial.AddTexture("difCubeMap", GetTexture(MatlEnums.ParamId.Texture8), GetSampler(MatlEnums.ParamId.Sampler8));
            genericMaterial.AddTexture("difMap", GetTexture(MatlEnums.ParamId.Texture10), GetSampler(MatlEnums.ParamId.Sampler10));
            genericMaterial.AddTexture("dif2Map", GetTexture(MatlEnums.ParamId.Texture11), GetSampler(MatlEnums.ParamId.Sampler11));
            genericMaterial.AddTexture("dif3Map", GetTexture(MatlEnums.ParamId.Texture12), GetSampler(MatlEnums.ParamId.Sampler12));
        }

        public SamplerObject GetSampler(MatlEnums.ParamId paramId)
        {
            if (!samplerByParamId.ContainsKey(paramId))
                return defaultSampler;

            return samplerByParamId[paramId];
        }

        private Texture GetTexture(MatlEnums.ParamId paramId)
        {
            // Set a default to avoid unnecessary conditionals in the shader.
            if (!textureNameByParamId.ContainsKey(paramId))
                return defaultTextureByParamId[paramId];

            // TODO: Add the missing default textures.
            // TODO: The texture colors can be determined using an OpenGL debugger.
            if (!TextureByName.ContainsKey(textureNameByParamId[paramId]))
                return defaultTextureByParamId[paramId];

            return TextureByName[textureNameByParamId[paramId]];
        }
   
        private void AddRenderModeTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("uvPattern", DefaultTextures.Instance.UvPattern);
        }

        private void AddImageBasedLightingTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("diffusePbrCube", DefaultTextures.Instance.DiffusePbr);
            genericMaterial.AddTexture("specularPbrCube", GetTexture(MatlEnums.ParamId.Texture7));
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
