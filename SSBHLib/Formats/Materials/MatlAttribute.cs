using SSBHLib.IO;

namespace SSBHLib.Formats.Materials
{
    public partial class MatlAttribute : ISSBH_File
    {
        public MatlEnums.ParamId ParamID { get; set; }

        public SSBHOffset OffsetToData { get; set; }

        public MatlEnums.ParamDataType DataType { get; set; }

        // not part of the entry
        [ParseTag(Ignore = true)]
        public object DataObject { get; set; }

        public override void PostProcess(SSBHParser R)
        {
            R.Seek(OffsetToData);

            if (DataType == MatlEnums.ParamDataType.Float)
                DataObject = R.ReadSingle();
            else if (DataType == MatlEnums.ParamDataType.Boolean)
                DataObject = R.ReadUInt32() == 1;
            else if (DataType == MatlEnums.ParamDataType.Vector4)
                DataObject = R.Parse<MatlVector4>();
            else if (DataType == MatlEnums.ParamDataType.String)
                DataObject = R.Parse<MtalString>();
            else if (DataType == MatlEnums.ParamDataType.Sampler)
                DataObject = R.Parse<MtalSampler>();
            else if (DataType == MatlEnums.ParamDataType.UvTransform)
                DataObject = R.Parse<MTAL_UVTransform>();
            else if (DataType == MatlEnums.ParamDataType.BlendState)
                DataObject = R.Parse<MatlBlendState>();
            else if (DataType == MatlEnums.ParamDataType.RasterizerState)
                DataObject = R.Parse<MatlRasterizerState>();
        }

        private static string GetPropertyValues(System.Type type, object obj)
        {
            string result = "(";
            foreach (var property in type.GetProperties())
            {
                result += property.GetValue(obj).ToString() + ", ";
            }
            result = result.TrimEnd(',', ' ');
            result += ")";
            return result;
        }
    }
}