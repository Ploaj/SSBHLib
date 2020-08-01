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
        public object DataObject
        {
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
        private static readonly Dictionary<Type, ParamDataType> typeToParamType = new Dictionary<Type, ParamDataType>()
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

        public void PostProcess(SsbhParser parser)
        {
            parser.Seek(OffsetToData);

            switch (DataType)
            {
                case ParamDataType.Float:
                    DataObject = parser.ReadSingle();
                    break;
                case ParamDataType.Boolean:
                    DataObject = parser.ReadUInt32() == 1; // should this just be > 0?
                    break;
                case ParamDataType.Vector4:
                    DataObject = parser.ParseMatlVector4();
                    break;
                case ParamDataType.String:
                    DataObject = parser.ParseMatlString();
                    break;
                case ParamDataType.Sampler:
                    DataObject = parser.ParseMatlSampler();
                    break;
                case ParamDataType.UvTransform:
                    DataObject = parser.ParseMatlUvTransform();
                    break;
                case ParamDataType.BlendState:
                    DataObject = parser.ParseMatlBlendState();
                    break;
                case ParamDataType.RasterizerState:
                    DataObject = parser.ParseMatlRasterizerState();
                    break;
            }
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