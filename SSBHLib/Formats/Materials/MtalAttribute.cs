using SSBHLib.IO;

namespace SSBHLib.Formats.Materials
{
    public partial class MtalAttribute : ISSBH_File
    {
        public MatlEnums.ParamId ParamID { get; set; }

        public SSBHOffset OffsetToData { get; set; }

        public long DataType { get; set; }

        // not part of the entry
        public object DataObject;

        public override void PostProcess(SSBHParser R)
        {
            R.Seek(OffsetToData);

            if (DataType == (long)MatlEnums.ParamDataType.Float)
                DataObject = R.ReadSingle();
            else if (DataType == (long)MatlEnums.ParamDataType.Boolean)
                DataObject = R.ReadUInt32() == 1;
            else if (DataType == (long)MatlEnums.ParamDataType.Vector4)
                DataObject = R.Parse<MtalVector4>();
            else if (DataType == (long)MatlEnums.ParamDataType.String)
                DataObject = R.Parse<MtalString>();
            else if (DataType == (long)MatlEnums.ParamDataType.Sampler)
                DataObject = R.Parse<MtalSampler>();
            else if (DataType == (long)MatlEnums.ParamDataType.UvTransform)
                DataObject = R.Parse<MTAL_UVTransform>();
            else if (DataType == (long)MatlEnums.ParamDataType.BlendState)
                DataObject = R.Parse<MtalBlendState>();
            else if (DataType == (long)MatlEnums.ParamDataType.RasterizerState)
                DataObject = R.Parse<MtalRasterizerState>();
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