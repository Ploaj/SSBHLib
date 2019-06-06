using SSBHLib.IO;
using System;
using System.Collections.Generic;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace SSBHLib.Formats.Materials
{
    public partial class MatlAttribute : ISSBH_File
    {
        public ParamId ParamID { get; set; }

        public SSBHOffset OffsetToData { get; set; }

        public ParamDataType DataType { get; set; }

        // not part of the entry
        [ParseTag(Ignore = true)]
        public object DataObject {
            get
            {
                return _dataObject;
            }
            set
            {
                _dataObject = value;
                DataType = typeToParamType[_dataObject.GetType()];
            }
        }
        [ParseTag(Ignore = true)]
        private object _dataObject;

        [ParseTag(Ignore = true)]
        private static Dictionary<Type, MatlEnums.ParamDataType> typeToParamType = new Dictionary<Type, MatlEnums.ParamDataType>()
            {
            { typeof(float), ParamDataType.Float},
            { typeof(bool), ParamDataType.Boolean},
            { typeof(MatlBlendState), ParamDataType.BlendState},
            { typeof(MatlRasterizerState), ParamDataType.RasterizerState},
            { typeof(MatlSampler), ParamDataType.Sampler},
            { typeof(MatlString), ParamDataType.String},
            { typeof(MatlUVTransform), ParamDataType.UvTransform},
            { typeof(MatlVector4), ParamDataType.Vector4},
            };

        public override void PostProcess(SSBHParser parser)
        {
            parser.Seek(OffsetToData);

            if (DataType == MatlEnums.ParamDataType.Float)
                DataObject = parser.ReadSingle();
            else if (DataType == MatlEnums.ParamDataType.Boolean)
                DataObject = parser.ReadUInt32() == 1;
            else if (DataType == MatlEnums.ParamDataType.Vector4)
                DataObject = parser.Parse<MatlVector4>();
            else if (DataType == MatlEnums.ParamDataType.String)
                DataObject = parser.Parse<MatlString>();
            else if (DataType == MatlEnums.ParamDataType.Sampler)
                DataObject = parser.Parse<MatlSampler>();
            else if (DataType == MatlEnums.ParamDataType.UvTransform)
                DataObject = parser.Parse<MatlUVTransform>();
            else if (DataType == MatlEnums.ParamDataType.BlendState)
                DataObject = parser.Parse<MatlBlendState>();
            else if (DataType == MatlEnums.ParamDataType.RasterizerState)
                DataObject = parser.Parse<MatlRasterizerState>();
        }

        private static string GetPropertyValues(Type type, object obj)
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