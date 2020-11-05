using CrossMod.Rendering.GlTools;
using SFGraphics.GLObjects.Samplers;
using SSBHLib.Formats.Materials;

namespace CrossMod.Tools
{
    public static class MatlToSFGraphicsExtensions
    {
        public static SamplerObject ToSfGraphics(this MatlAttribute.MatlSampler samplerStruct)
        {
            SamplerObject sampler = new SamplerObject
            {
                TextureWrapS = samplerStruct.WrapS.ToOpenTk(),
                TextureWrapT = samplerStruct.WrapT.ToOpenTk(),
                TextureWrapR = samplerStruct.WrapR.ToOpenTk(),
                MagFilter = samplerStruct.MagFilter.ToOpenTk(),
                MinFilter = samplerStruct.MinFilter.ToOpenTk(),
                TextureLodBias = samplerStruct.LodBias,
            };

            if (samplerStruct.Unk6 == 2)
                sampler.TextureMaxAnisotropy = samplerStruct.MaxAnisotropy;
            else
                sampler.TextureMaxAnisotropy = 1.0f;

            return sampler;
        }
    }
}
