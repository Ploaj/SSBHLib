using SSBHLib.Formats.Materials;
using System.Collections.Generic;
using System.Linq;

namespace CrossMod.Tools
{
    // TODO: This can be part of SSBHLib itself because there are no additional dependencies.
    public static class MatlExtensions
    {
        public static Dictionary<MatlEnums.ParamId, float> GetFloats(this MatlEntry entry)
        {
            return entry.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.Float)
                .ToDictionary(a => a.ParamId, a => (float)a.DataObject);
        }

        public static Dictionary<MatlEnums.ParamId, bool> GetBools(this MatlEntry entry)
        {
            return entry.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.Boolean)
                .ToDictionary(a => a.ParamId, a => (bool)a.DataObject);
        }

        public static Dictionary<MatlEnums.ParamId, string> GetTextures(this MatlEntry entry)
        {
            return entry.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.String)
                .ToDictionary(a => a.ParamId, a => ((MatlAttribute.MatlString)a.DataObject).Text);
        }

        public static Dictionary<MatlEnums.ParamId, MatlAttribute.MatlVector4> GetVectors(this MatlEntry entry)
        {
            return entry.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.Vector4)
                .ToDictionary(a => a.ParamId, a => (MatlAttribute.MatlVector4)a.DataObject);
        }

        public static Dictionary<MatlEnums.ParamId, MatlAttribute.MatlSampler> GetSamplers(this MatlEntry entry)
        {
            return entry.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.Sampler)
                .ToDictionary(a => a.ParamId, a => (MatlAttribute.MatlSampler)a.DataObject);
        }

        public static Dictionary<MatlEnums.ParamId, MatlAttribute.MatlRasterizerState> GetRasterizerStates(this MatlEntry entry)
        {
            return entry.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.RasterizerState)
                .ToDictionary(a => a.ParamId, a => (MatlAttribute.MatlRasterizerState)a.DataObject);
        }

        public static Dictionary<MatlEnums.ParamId, MatlAttribute.MatlBlendState> GetBlendStates(this MatlEntry entry)
        {
            return entry.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.BlendState)
                .ToDictionary(a => a.ParamId, a => (MatlAttribute.MatlBlendState)a.DataObject);
        }
    }
}
