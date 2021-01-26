using System;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace CrossMod.Rendering
{
    // TODO: This should be part of SSBHLib.
    public static class ParamIdExtensions
    {
        public static ParamId GetSampler(ParamId texture)
        {
            return texture switch
            {
                ParamId.Texture0 => ParamId.Sampler0,
                ParamId.Texture1 => ParamId.Sampler1,
                ParamId.Texture2 => ParamId.Sampler2,
                ParamId.Texture3 => ParamId.Sampler3,
                ParamId.Texture4 => ParamId.Sampler4,
                ParamId.Texture5 => ParamId.Sampler5,
                ParamId.Texture6 => ParamId.Sampler6,
                ParamId.Texture7 => ParamId.Sampler7,
                ParamId.Texture8 => ParamId.Sampler8,
                ParamId.Texture9 => ParamId.Sampler9,
                ParamId.Texture10 => ParamId.Sampler10,
                ParamId.Texture11 => ParamId.Sampler11,
                ParamId.Texture12 => ParamId.Sampler12,
                ParamId.Texture13 => ParamId.Sampler13,
                ParamId.Texture14 => ParamId.Sampler14,
                ParamId.Texture15 => ParamId.Sampler15,
                ParamId.Texture16 => ParamId.Sampler16,
                ParamId.Texture17 => ParamId.Sampler17,
                ParamId.Texture18 => ParamId.Sampler18,
                ParamId.Texture19 => ParamId.Sampler19,
                _ => throw new ArgumentOutOfRangeException(nameof(texture), $"{texture} has no associated sampler")
            };
        }

        public static int ToFloatIndex(this ParamId paramId)
        {
            if (paramId < ParamId.CustomFloat0 || paramId > ParamId.CustomFloat19)
                throw new ArgumentOutOfRangeException(nameof(paramId));

            return (int)paramId - (int)ParamId.CustomFloat0;
        }

        public static int ToBooleanIndex(this ParamId paramId)
        {
            if (paramId < ParamId.CustomBoolean0 || paramId > ParamId.CustomBoolean19)
                throw new ArgumentOutOfRangeException(nameof(paramId));

            return (int)paramId - (int)ParamId.CustomBoolean0;
        }

        public static int ToVectorIndex(this ParamId paramId)
        {
            if (paramId < ParamId.CustomVector0 || paramId > ParamId.CustomVector63)
                throw new ArgumentOutOfRangeException(nameof(paramId));

            // Vector param IDs don't take up a single range.
            if (paramId > ParamId.CustomVector19)
                return (int)paramId - (int)ParamId.CustomVector20 + 20;

            return (int)paramId - (int)ParamId.CustomVector0;
        }

        public static int ToTextureIndex(this ParamId paramId)
        {
            if (paramId < ParamId.Texture0 || paramId > ParamId.Texture19)
                throw new ArgumentOutOfRangeException(nameof(paramId));

            // Texture param IDs don't take up a single range.
            if (paramId > ParamId.Texture15)
                return (int)paramId - (int)ParamId.Texture16 + 16;

            return (int)paramId - (int)ParamId.Texture0;
        }

        public static int ToSamplerIndex(this ParamId paramId)
        {
            if (paramId < ParamId.Sampler0 || paramId > ParamId.Sampler19)
                throw new ArgumentOutOfRangeException(nameof(paramId));

            // Sampler param IDs don't take up a single range.
            if (paramId > ParamId.Sampler15)
                return (int)paramId - (int)ParamId.Sampler16 + 16;

            return (int)paramId - (int)ParamId.Sampler0;
        }
    }
}
