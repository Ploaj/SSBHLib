using OpenTK;
using OpenTK.Graphics.OpenGL;
using SSBHLib.Formats.Materials;
using System;

namespace CrossMod.Rendering.GlTools
{
    public static class MatlToOpenTk
    {
        public static SamplerData ToSamplerData(this MatlAttribute.MatlSampler samplerStruct)
        {
            var sampler = new SamplerData
            {
                WrapS = samplerStruct.WrapS.ToOpenTk(),
                WrapT = samplerStruct.WrapT.ToOpenTk(),
                WrapR = samplerStruct.WrapR.ToOpenTk(),
                MagFilter = samplerStruct.MagFilter.ToOpenTk(),
                MinFilter = samplerStruct.MinFilter.ToOpenTk(),
                LodBias = samplerStruct.LodBias,
            };

            if (samplerStruct.TextureFilteringType == FilteringType.AnisotropicFiltering)
                sampler.MaxAnisotropy = samplerStruct.MaxAnisotropy;
            else
                sampler.MaxAnisotropy = 1;

            return sampler;
        }

        public static Vector4 ToOpenTk(this MatlAttribute.MatlVector4 value)
        {
            return new Vector4(value.X, value.Y, value.Z, value.W);
        }

        public static TextureMagFilter ToOpenTk(this MatlMagFilter magFilter)
        {
            switch (magFilter)
            {
                case MatlMagFilter.Nearest:
                    return TextureMagFilter.Nearest;
                case MatlMagFilter.Linear:
                case MatlMagFilter.Linear2:
                    return TextureMagFilter.Linear;
                default:
                    throw new NotSupportedException($"Unsupported conversion for {magFilter}");
            }
        }

        public static TextureMinFilter ToOpenTk(this MatlMinFilter minFilter)
        {
            switch (minFilter)
            {
                case MatlMinFilter.Nearest:
                    return TextureMinFilter.Nearest;
                case MatlMinFilter.LinearMipmapLinear:
                case MatlMinFilter.LinearMipmapLinear2:
                    return TextureMinFilter.LinearMipmapLinear;
                default:
                    throw new NotSupportedException($"Unsupported conversion for {minFilter}");
            }
        }

        public static TextureWrapMode ToOpenTk(this MatlWrapMode wrapMode)
        {
            return wrapMode switch
            {
                MatlWrapMode.Repeat => TextureWrapMode.Repeat,
                MatlWrapMode.ClampToEdge => TextureWrapMode.ClampToEdge,
                MatlWrapMode.MirroredRepeat => TextureWrapMode.MirroredRepeat,
                MatlWrapMode.ClampToBorder => TextureWrapMode.ClampToBorder,
                _ => throw new NotSupportedException($"Unsupported conversion for {wrapMode}"),
            };
        }

        public static CullFaceMode ToOpenTk(this MatlCullMode cullMode)
        {
            switch (cullMode)
            {
                // None requires explicitly disabling culling, so just return back.
                case MatlCullMode.None:
                case MatlCullMode.Back:
                    return CullFaceMode.Back;
                case MatlCullMode.Front:
                    return CullFaceMode.Front;
                default:
                    throw new NotSupportedException($"Unsupported conversion for {cullMode}");
            }
        }

        public static BlendingFactor ToOpenTk(this MatlBlendFactor factor)
        {
            return factor switch
            {
                MatlBlendFactor.Zero => BlendingFactor.Zero,
                MatlBlendFactor.One => BlendingFactor.One,
                MatlBlendFactor.SourceAlpha => BlendingFactor.SrcAlpha,
                MatlBlendFactor.DestinationAlpha => BlendingFactor.DstAlpha,
                MatlBlendFactor.SourceColor => BlendingFactor.DstColor,
                MatlBlendFactor.DestinationColor => BlendingFactor.DstColor,
                MatlBlendFactor.OneMinusSourceAlpha => BlendingFactor.OneMinusSrcAlpha,
                MatlBlendFactor.OneMinusDestinationAlpha => BlendingFactor.OneMinusDstAlpha,
                MatlBlendFactor.OneMinusSourceColor => BlendingFactor.OneMinusDstColor,
                MatlBlendFactor.OneMinusDestinationColor => BlendingFactor.OneMinusDstColor,
                MatlBlendFactor.SourceAlphaSaturate => BlendingFactor.SrcAlphaSaturate,
                _ => throw new NotSupportedException($"Unsupported conversion for {factor}")
            };
        }

        public static PolygonMode ToOpenTk(this MatlFillMode fillMode)
        {
            return fillMode switch
            {
                MatlFillMode.Solid => PolygonMode.Fill,
                MatlFillMode.Line => PolygonMode.Line,
                _ => throw new NotSupportedException($"Unsupported conversion for {fillMode}")
            };
        }
    }
}
