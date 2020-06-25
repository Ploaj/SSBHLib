using SSBHLib.IO;
using System;
using System.Collections.Generic;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace SSBHLib.Formats.Materials
{
    public partial class MatlAttribute : SsbhFile
    {
        public ParamId ParamId { get; set; }

        public SsbhOffset OffsetToData { get; set; }

        public ParamDataType DataType { get; set; }

        // not part of the entry
        [ParseTag(Ignore = true)]
        public object DataObject {
            get => dataObject;
            set
            {
                dataObject = value;
                DataType = typeToParamType[dataObject.GetType()];
            }
        }
        [ParseTag(Ignore = true)]
        private object dataObject;

        [ParseTag(Ignore = true)]
        private static Dictionary<Type, MatlEnums.ParamDataType> typeToParamType = new Dictionary<Type, MatlEnums.ParamDataType>()
            {
            { typeof(float), ParamDataType.Float},
            { typeof(bool), ParamDataType.Boolean},
            { typeof(MatlBlendState), ParamDataType.BlendState},
            { typeof(MatlRasterizerState), ParamDataType.RasterizerState},
            { typeof(MatlSampler), ParamDataType.Sampler},
            { typeof(MatlString), ParamDataType.String},
            { typeof(MatlUvTransform), ParamDataType.UvTransform},
            { typeof(MatlVector4), ParamDataType.Vector4},
        };

        public override void PostProcess(SsbhParser parser)
        {
            parser.Seek(OffsetToData);

            if (DataType == MatlEnums.ParamDataType.Float)
                DataObject = parser.ReadSingle();
            else if (DataType == MatlEnums.ParamDataType.Boolean)
                DataObject = parser.ReadUInt32() == 1; // should this just be > 0?
            else if (DataType == MatlEnums.ParamDataType.Vector4)
                DataObject = parser.ParseMatlVector4();
            else if (DataType == MatlEnums.ParamDataType.String)
                DataObject = parser.ParseMatlString();
            else if (DataType == MatlEnums.ParamDataType.Sampler)
                DataObject = parser.ParseMatlSampler();
            else if (DataType == MatlEnums.ParamDataType.UvTransform)
                DataObject = parser.ParseMatlUvTransform();
            else if (DataType == MatlEnums.ParamDataType.BlendState)
                DataObject = parser.ParseMatlBlendState();
            else if (DataType == MatlEnums.ParamDataType.RasterizerState)
                DataObject = parser.ParseMatlRasterizerState();
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