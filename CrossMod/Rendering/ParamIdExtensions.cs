using System;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace CrossMod.Rendering
{
    public static class ParamIdExtensions
    {
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
