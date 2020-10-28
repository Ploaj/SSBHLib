using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Samplers;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

namespace CrossMod.Rendering.GlTools
{
    public static class MatlToMaterial
    {
        public static RMaterial CreateMaterial(MatlEntry currentEntry, int index, Dictionary<string, RTexture> textureByName)
        {
            RMaterial meshMaterial = new RMaterial()
            {
                MaterialLabel = currentEntry.MaterialLabel,
                ShaderLabel = currentEntry.ShaderLabel,
                Index = index,
                TextureByName = textureByName
            };

            foreach (MatlAttribute attribute in currentEntry.Attributes)
            {
                if (attribute.DataObject == null)
                    continue;

                switch (attribute.DataType)
                {
                    case MatlEnums.ParamDataType.String:
                        SetTextureParameter(meshMaterial, attribute);
                        break;
                    case MatlEnums.ParamDataType.Vector4:
                        meshMaterial.UpdateVec4(attribute.ParamId, ((MatlAttribute.MatlVector4)attribute.DataObject).ToOpenTk());
                        break;
                    case MatlEnums.ParamDataType.Boolean:
                        meshMaterial.UpdateBoolean(attribute.ParamId, (bool)attribute.DataObject);
                        break;
                    case MatlEnums.ParamDataType.Float:
                        meshMaterial.UpdateFloat(attribute.ParamId, (float)attribute.DataObject);
                        break;
                    case MatlEnums.ParamDataType.BlendState:
                        SetBlendState(meshMaterial, attribute);
                        break;
                    case MatlEnums.ParamDataType.RasterizerState:
                        SetRasterizerState(meshMaterial, attribute);
                        break;
                    case MatlEnums.ParamDataType.Sampler:
                        SetSamplerInformation(meshMaterial, attribute);
                        break;
                }
            }

            return meshMaterial;
        }
        private static void SetSamplerInformation(RMaterial material, MatlAttribute a)
        {
            var samplerStruct = (MatlAttribute.MatlSampler)a.DataObject;

            SamplerObject sampler = new SamplerObject
            {
                TextureWrapS = samplerStruct.WrapS.ToOpenTk(),
                TextureWrapT = samplerStruct.WrapT.ToOpenTk(),
                TextureWrapR = samplerStruct.WrapR.ToOpenTk(),
                MagFilter = samplerStruct.MagFilter.ToOpenTk(),
                MinFilter = samplerStruct.MinFilter.ToOpenTk(),
                TextureLodBias = samplerStruct.LodBias,
            };

            if (samplerStruct.Unk6 == 2 && RenderSettings.Instance.EnableExperimental)
                sampler.TextureMaxAnisotropy = samplerStruct.MaxAnisotropy;
            else
                sampler.TextureMaxAnisotropy = 1.0f;

            material.UpdateSampler(a.ParamId, sampler);
        }

        private static void SetRasterizerState(RMaterial meshMaterial, MatlAttribute a)
        {
            var rasterizerState = (MatlAttribute.MatlRasterizerState)a.DataObject;

            meshMaterial.DepthBias = rasterizerState.DepthBias;
            meshMaterial.CullMode = rasterizerState.CullMode.ToOpenTk();
            meshMaterial.EnableFaceCulling = rasterizerState.CullMode != MatlCullMode.None;
            meshMaterial.FillMode = rasterizerState.FillMode.ToOpenTk();
        }

        private static void SetBlendState(RMaterial meshMaterial, MatlAttribute a)
        {
            var blendState = (MatlAttribute.MatlBlendState)a.DataObject;

            // TODO: Does "src factor" toggle something in the shader?
            meshMaterial.BlendSrc = BlendingFactor.One;
            if (blendState.Unk1 == 0)
                meshMaterial.BlendSrc = BlendingFactor.Zero;

            if (blendState.BlendFactor2 == 1)
                meshMaterial.BlendDst = BlendingFactor.One;
            else if (blendState.BlendFactor2 == 2)
                meshMaterial.BlendDst = BlendingFactor.SrcAlpha;
            else if (blendState.BlendFactor2 == 6)
                meshMaterial.BlendDst = BlendingFactor.OneMinusSrcAlpha;

            // TODO: Do both need to be set?
            meshMaterial.UseAlphaSampleCoverage = blendState.Unk7 == 1 || blendState.Unk8 == 1;
        }

        private static void SetTextureParameter(RMaterial meshMaterial, MatlAttribute a)
        {
            // Don't make texture names case sensitive.
            var textureName = ((MatlAttribute.MatlString)a.DataObject).Text.ToLower();
            meshMaterial.UpdateTexture(a.ParamId, textureName);
        }
    }
}
