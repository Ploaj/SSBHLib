using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Samplers;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

namespace CrossMod.Rendering.GlTools
{
    // TODO: This class could just be part of the material class.
    public static class MatlToMaterial
    {
        public enum TextureParams
        {
            ColMap = 0x5C,
            ColMap2 = 0x5D,
            GaoMap = 0x5F,
            NorMap = 0x60,
            EmiMap = 0x61,
            EmiMap2 = 0x6A,
            PrmMap = 0x62,
            SpecularCubeMap = 0x63,
            DifCubeMap = 0x64,
            BakeLitMap = 0x65,
            DiffuseMap = 0x66,
            DiffuseMap2 = 0x67,
            DiffuseMap3 = 0x68,
            ProjMap = 0x69,
            InkNorMap = 0x133,
            ColSampler = 0x6C,
            NorSampler = 0x70,
            PrmSampler = 0x72,
            EmiSampler = 0x71
        }

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
                        var vec4 = (MatlAttribute.MatlVector4)attribute.DataObject;
                        meshMaterial.vec4ByParamId[attribute.ParamId] = new Vector4(vec4.X, vec4.Y, vec4.Z, vec4.W);
                        break;
                    case MatlEnums.ParamDataType.Boolean:
                        bool boolValue = (bool)attribute.DataObject;
                        meshMaterial.boolByParamId[attribute.ParamId] = boolValue;
                        break;
                    case MatlEnums.ParamDataType.Float:
                        float floatValue = (float)attribute.DataObject;
                        meshMaterial.floatByParamId[attribute.ParamId] = floatValue;
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
                TextureWrapS = MatlToGl.GetWrapMode(samplerStruct.WrapS),
                TextureWrapT = MatlToGl.GetWrapMode(samplerStruct.WrapT),
                TextureWrapR = MatlToGl.GetWrapMode(samplerStruct.WrapR),
                MagFilter = MatlToGl.GetMagFilter(samplerStruct.MagFilter),
                MinFilter = MatlToGl.GetMinFilter(samplerStruct.MinFilter),
                TextureLodBias = samplerStruct.LodBias,
            };

            if (samplerStruct.Unk6 == 2 && RenderSettings.Instance.EnableExperimental)
                sampler.TextureMaxAnisotropy = samplerStruct.MaxAnisotropy;
            else
                sampler.TextureMaxAnisotropy = 1.0f;

            material.samplerByParamId[a.ParamId] = sampler;
        }

        private static void SetRasterizerState(RMaterial meshMaterial, MatlAttribute a)
        {
            var rasterizerState = (MatlAttribute.MatlRasterizerState)a.DataObject;

            meshMaterial.DepthBias = rasterizerState.DepthBias;
            meshMaterial.CullMode = MatlToGl.GetCullMode(rasterizerState.CullMode);
            meshMaterial.EnableFaceCulling = rasterizerState.CullMode != MatlCullMode.None;
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
            meshMaterial.textureNameByParamId[a.ParamId] = textureName;
        }
    }
}
