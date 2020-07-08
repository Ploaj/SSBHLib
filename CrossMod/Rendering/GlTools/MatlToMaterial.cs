using CrossMod.Rendering.Resources;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Samplers;
using SFGraphics.GLObjects.Textures;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

namespace CrossMod.Rendering.GlTools
{
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

        public static Material CreateMaterial(MatlEntry currentEntry, Dictionary<string, Texture> textureByName)
        {
            Material meshMaterial = new Material()
            {
                Name = currentEntry.MaterialLabel
            };

            foreach (MatlAttribute attribute in currentEntry.Attributes)
            {
                if (attribute.DataObject == null)
                    continue;

                switch (attribute.DataType)
                {
                    case MatlEnums.ParamDataType.String:
                        SetTextureParameter(meshMaterial, attribute, textureByName);
                        // Just render as white if texture is present.
                        // TODO: Add proper debug rendering of textures
                        meshMaterial.floatByParamId[attribute.ParamId] = 1;
                        break;
                    case MatlEnums.ParamDataType.Vector4:
                        var vec4 = (MatlAttribute.MatlVector4)attribute.DataObject;
                        meshMaterial.vec4ByParamId[attribute.ParamId] = new Vector4(vec4.X, vec4.Y, vec4.Z, vec4.W);
                        break;
                    case MatlEnums.ParamDataType.Boolean:
                        // Convert to vec4 to use with rendering.
                        // Use cyan to differentiate with no value (blue).
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
                }
            }

            foreach (MatlAttribute a in currentEntry.Attributes)
            {
                if (a.DataObject == null || a.DataType != MatlEnums.ParamDataType.Sampler)
                    continue;

                SetSamplerInformation(meshMaterial, a);
            }

            return meshMaterial;
        }
        private static void SetSamplerInformation(Material material, MatlAttribute a)
        {
            // TODO: This could be cleaner.
            SamplerObject sampler = null;
            switch ((long)a.ParamId)
            {
                case (long)TextureParams.ColSampler:
                    sampler = material.colSampler;
                    break;
                case (long)TextureParams.NorSampler:
                    sampler = material.norSampler;
                    break;
                case (long)TextureParams.PrmSampler:
                    sampler = material.prmSampler;
                    break;
                case (long)TextureParams.EmiSampler:
                    sampler = material.emiSampler;
                    break;
            }

            if (sampler != null)
            {
                var samplerStruct = (MatlAttribute.MatlSampler)a.DataObject;
                sampler.TextureWrapS = MatlToGl.GetWrapMode(samplerStruct.WrapS);
                sampler.TextureWrapT = MatlToGl.GetWrapMode(samplerStruct.WrapT);
                sampler.TextureWrapR = MatlToGl.GetWrapMode(samplerStruct.WrapR);
                sampler.MagFilter = MatlToGl.GetMagFilter(samplerStruct.MagFilter);
                sampler.MinFilter = MatlToGl.GetMinFilter(samplerStruct.MinFilter);

                GL.SamplerParameter(sampler.Id, SamplerParameterName.TextureLodBias, samplerStruct.LodBias);

                if (samplerStruct.Unk6 == 2 && RenderSettings.Instance.EnableExperimental)
                    GL.SamplerParameter(sampler.Id, SamplerParameterName.TextureMaxAnisotropyExt, (float)samplerStruct.MaxAnisotropy);
                else
                    GL.SamplerParameter(sampler.Id, SamplerParameterName.TextureMaxAnisotropyExt, 1.0f);
            }
        }

        private static void SetRasterizerState(Material meshMaterial, MatlAttribute a)
        {
            var rasterizerState = (MatlAttribute.MatlRasterizerState)a.DataObject;

            meshMaterial.DepthBias = rasterizerState.DepthBias;
            meshMaterial.CullMode = MatlToGl.GetCullMode(rasterizerState.CullMode);
        }
        private static void SetBlendState(Material meshMaterial, MatlAttribute a)
        {
            // TODO: Add enums for matl type fields that can only have a few values.
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

            meshMaterial.IsTransparent = blendState.BlendFactor1 != 0 || blendState.BlendFactor2 != 0;

            // TODO: Do both need to be set?
            meshMaterial.UseAlphaSampleCoverage = blendState.Unk7 == 1 || blendState.Unk8 == 1;
        }

        private static void SetTextureParameter(Material meshMaterial, MatlAttribute a, Dictionary<string, Texture> textureByName)
        {
            // Don't make texture names case sensitive.
            var textureName = ((MatlAttribute.MatlString)a.DataObject).Text.ToLower();

            if (textureByName.TryGetValue(textureName, out Texture texture))
            {
                switch ((long)a.ParamId)
                {
                    case (long)TextureParams.ColMap:
                        meshMaterial.HasCol = true;
                        meshMaterial.col = texture;
                        break;
                    case (long)TextureParams.SpecularCubeMap:
                        meshMaterial.specularCubeMap = texture;
                        break;
                    case (long)TextureParams.GaoMap:
                        meshMaterial.gao = texture;
                        break;
                    case (long)TextureParams.ColMap2:
                        meshMaterial.col2 = texture;
                        meshMaterial.HasCol2 = true;
                        break;
                    case (long)TextureParams.NorMap:
                        meshMaterial.nor = texture;
                        break;
                    case (long)TextureParams.ProjMap:
                        meshMaterial.proj = texture;
                        break;
                    case (long)TextureParams.DifCubeMap:
                        meshMaterial.difCube = texture;
                        meshMaterial.HasDifCube = true;
                        break;
                    case (long)TextureParams.PrmMap:
                        meshMaterial.prm = texture;
                        break;
                    case (long)TextureParams.EmiMap:
                        meshMaterial.emi = texture;
                        meshMaterial.HasEmi = true;
                        break;
                    case (long)TextureParams.EmiMap2:
                        meshMaterial.emi2 = texture;
                        meshMaterial.HasEmi2 = true;
                        break;
                    case (long)TextureParams.BakeLitMap:
                        meshMaterial.bakeLit = texture;
                        break;
                    case (long)TextureParams.InkNorMap:
                        meshMaterial.HasInkNorMap = true;
                        meshMaterial.inkNor = texture;
                        break;
                    case (long)TextureParams.DiffuseMap:
                        meshMaterial.dif = texture;
                        meshMaterial.HasDiffuse = true;
                        break;
                    case (long)TextureParams.DiffuseMap2:
                        meshMaterial.dif2 = texture;
                        meshMaterial.HasDiffuse2 = true;
                        break;
                    case (long)TextureParams.DiffuseMap3:
                        meshMaterial.dif3 = texture;
                        meshMaterial.HasDiffuse3 = true;
                        break;
                }
            }

            // TODO: Find a better way to handle this case.
            if (((long)a.ParamId == (long)TextureParams.SpecularCubeMap) && textureName == "#replace_cubemap")
                meshMaterial.specularCubeMap = DefaultTextures.Instance.SpecularPbr;
        }
    }
}
