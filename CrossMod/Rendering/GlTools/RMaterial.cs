using CrossMod.Rendering.Models;
using CrossMod.Rendering.Resources;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.Materials;
using SFGraphics.GLObjects.Samplers;
using SFGraphics.GLObjects.Shaders;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

namespace CrossMod.Rendering.GlTools
{
    /// <summary>
    /// Stores <see cref="MatlEntry"/> material values as OpenGL uniforms and render state.
    /// </summary>
    public class RMaterial
    {
        public string MaterialLabel { get; set; }
        public string ShaderLabel { get; set; }
        public int Index { get; set; }

        public Vector3 MaterialIdColorRgb255 => UniqueColors.IndexToColor(Index);

        private GenericMaterial genericMaterial = null;
        private UniformBlock uniformBlock = null;

        // The parameters don't matter because the default texture are solid color.
        private readonly SamplerObject defaultSampler = new SamplerObject();

        public Dictionary<string, RTexture> TextureByName { get; set; } = new Dictionary<string, RTexture>();

        public float CurrentFrame { get; set; } = 0;

        public bool EnableFaceCulling { get; set; }
        public CullFaceMode CullMode { get; set; } = CullFaceMode.Back;

        public PolygonMode FillMode { get; set; } = PolygonMode.Fill;

        public BlendingFactor BlendSrc { get; set; } = BlendingFactor.One;
        public BlendingFactor BlendDst { get; set; } = BlendingFactor.Zero;
        public bool HasSortLabel => ShaderLabel.EndsWith("_sort");

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

        // TODO: Make these private to ensure changes are updated in the viewport.
        public Dictionary<MatlEnums.ParamId, Vector4> vec4ByParamId = new Dictionary<MatlEnums.ParamId, Vector4>();
        public Dictionary<MatlEnums.ParamId, bool> boolByParamId = new Dictionary<MatlEnums.ParamId, bool>();
        public Dictionary<MatlEnums.ParamId, float> floatByParamId = new Dictionary<MatlEnums.ParamId, float>();

        public Dictionary<MatlEnums.ParamId, string> textureNameByParamId = new Dictionary<MatlEnums.ParamId, string>();
        public Dictionary<MatlEnums.ParamId, SamplerObject> samplerByParamId = new Dictionary<MatlEnums.ParamId, SamplerObject>();

        // Add a flag to ensure the uniforms get updated for rendering.
        // TODO: Updating the uniform block from the update methods doesn't work.
        // TODO: The performance impact is negligible, but not all uniforms need to be updated at once
        private bool shouldUpdateUniformBlock = false;
        private bool shouldUpdateTextures = false;

        public void UpdateVec4(MatlEnums.ParamId paramId, Vector4 value)
        {
            vec4ByParamId[paramId] = value;
            shouldUpdateUniformBlock = true;
        }

        public void UpdateTexture(MatlEnums.ParamId paramId, string value)
        {
            textureNameByParamId[paramId] = value;
            shouldUpdateTextures = true;
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

        public RMaterial()
        {

        }

        public void SetMaterialUniforms(Shader shader, RMaterial previousMaterial)
        {
            // TODO: This code could be moved to the constructor.
            if (genericMaterial == null || shouldUpdateTextures)
            {
                genericMaterial = CreateGenericMaterial();
                shouldUpdateTextures = false;
            }

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

            // Update the uniform values.
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

            SFGenericModel.RenderState.GLRenderSettings.SetFaceCulling(new SFGenericModel.RenderState.FaceCullingSettings(EnableFaceCulling, CullMode));
            SFGenericModel.RenderState.GLRenderSettings.SetPolygonModeSettings(new SFGenericModel.RenderState.PolygonModeSettings(MaterialFace.FrontAndBack, FillMode));
        }

        private GenericMaterial CreateGenericMaterial()
        {
            // Don't use the default texture unit.
            var genericMaterial = new GenericMaterial(1);

            AddTextures(genericMaterial);

            // HACK: There isn't an easy way to access the current frame.
            genericMaterial.AddFloat("currentFrame", CurrentFrame);
            genericMaterial.AddFloat("depthBias", DepthBias);
            genericMaterial.AddVector3("materialId", MaterialIdColorRgb255 / 255f);

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
            SetVec4(uniformBlock, RenderSettings.Instance.ParamId, new Vector4(0));
        }

        private void SetMaterialParams(UniformBlock uniformBlock)
        {
            // TODO: Refactor this to be cleaner.
            // Set defaults
            var customVectors = new Vector4[64];
            customVectors[3] = Vector4.One;
            customVectors[6] = new Vector4(1, 1, 0, 0);
            customVectors[8] = Vector4.One;
            customVectors[13] = Vector4.One;
            customVectors[14] = Vector4.One;
            customVectors[18] = Vector4.One;
            customVectors[31] = new Vector4(1, 1, 0, 0);
            customVectors[32] = new Vector4(1, 1, 0, 0);

            // Set values from the material.
            foreach (var param in vec4ByParamId)
            {
                customVectors[param.Key.ToVectorIndex()] = param.Value;
            }
            uniformBlock.SetValues("CustomVector", customVectors);

            // Set defaults.
            var customBooleans = new bool[20];
            customBooleans[1] = false;
            customBooleans[2] = true;
            customBooleans[3] = true;
            customBooleans[4] = true;
            customBooleans[5] = false;
            customBooleans[6] = false;
            customBooleans[9] = false;
            customBooleans[11] = true;

            // Set values from material.
            // HACK: A temporary workaround for integers being 16 byte aligned.
            // TODO: Query the stride?
            var customBooleanData = new IVec4[20];
            foreach (var param in boolByParamId)
            {
                customBooleanData[param.Key.ToBooleanIndex()] = new IVec4 { X = param.Value ? 1 : 0 };
            }
            uniformBlock.SetValues("CustomBoolean", customBooleanData);

            // Set values from material.
            // HACK: A temporary workaround for integers being 16 byte aligned.
            // TODO: Query the stride?
            var customFloatData = new Vector4[20];
            foreach (var param in floatByParamId)
            {
                customFloatData[param.Key.ToFloatIndex()] = new Vector4(0f, 0f, 0f, param.Value);
            }
            uniformBlock.SetValues("CustomFloat", customFloatData);

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
            genericMaterial.AddTexture("colMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture0), GetSampler(MatlEnums.ParamId.Sampler0));
            genericMaterial.AddTexture("col2Map", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture1), GetSampler(MatlEnums.ParamId.Sampler1));
            genericMaterial.AddTexture("prmMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture6), GetSampler(MatlEnums.ParamId.Sampler6));
            genericMaterial.AddTexture("norMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture4), GetSampler(MatlEnums.ParamId.Sampler4));
            genericMaterial.AddTexture("inkNorMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture16), GetSampler(MatlEnums.ParamId.Sampler16));
            genericMaterial.AddTexture("emiMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture5), GetSampler(MatlEnums.ParamId.Sampler5));
            genericMaterial.AddTexture("emi2Map", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture14), GetSampler(MatlEnums.ParamId.Sampler14));
            genericMaterial.AddTexture("bakeLitMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture9), GetSampler(MatlEnums.ParamId.Sampler9));
            genericMaterial.AddTexture("gaoMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture3), GetSampler(MatlEnums.ParamId.Sampler3));
            genericMaterial.AddTexture("projMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture13), GetSampler(MatlEnums.ParamId.Sampler13));
            genericMaterial.AddTexture("difCubeMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture8), GetSampler(MatlEnums.ParamId.Sampler8));
            genericMaterial.AddTexture("difMap", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture10), GetSampler(MatlEnums.ParamId.Sampler10));
            genericMaterial.AddTexture("dif2Map", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture11), GetSampler(MatlEnums.ParamId.Sampler11));
            genericMaterial.AddTexture("dif3Map", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture12), GetSampler(MatlEnums.ParamId.Sampler12));
        }

        public SamplerObject GetSampler(MatlEnums.ParamId paramId)
        {
            if (!samplerByParamId.ContainsKey(paramId))
                return defaultSampler;

            // TODO: Log this error?
            return samplerByParamId[paramId];
        }

        private void AddRenderModeTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("uvPattern", DefaultTextures.Instance.UvPattern);
        }

        private void AddImageBasedLightingTextures(GenericMaterial genericMaterial)
        {
            genericMaterial.AddTexture("diffusePbrCube", DefaultTextures.Instance.DiffusePbr);
            genericMaterial.AddTexture("specularPbrCube", TextureAssignment.GetTexture(this, MatlEnums.ParamId.Texture7));
        }

        private void SetVec4(UniformBlock uniformBlock, MatlEnums.ParamId paramId, Vector4 defaultValue)
        {
            // Convert parameters into colors for easier visualization.
            var name = "vec4Param";

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
