using SSBHLib.Formats.Materials;
using System.Xml.Serialization;

namespace MatLab
{
    [XmlInclude(typeof(MatlAttribute.MatlBlendState)), XmlInclude(typeof(MatlAttribute.MatlRasterizerState)), 
        XmlInclude(typeof(MatlAttribute.MatlVector4)), XmlInclude(typeof(MatlAttribute.MatlSampler)), 
        XmlInclude(typeof(MatlAttribute.MatlString)), XmlInclude(typeof(MatlAttribute.MatlUvTransform))]
    public class MatlXmlAttribute
    {
        [XmlAttribute]
        public MatlEnums.ParamId name;

        [XmlElement("blend_state", typeof(MatlAttribute.MatlBlendState))]
        [XmlElement("rasterizer_state", typeof(MatlAttribute.MatlRasterizerState))]
        [XmlElement("vector4", typeof(MatlAttribute.MatlVector4))]
        [XmlElement("sampler", typeof(MatlAttribute.MatlSampler))]
        [XmlElement("string", typeof(MatlAttribute.MatlString))]
        [XmlElement("UVtransform", typeof(MatlAttribute.MatlUvTransform))]
        [XmlElement("float", typeof(float))]
        [XmlElement("bool", typeof(bool))]
        public object value;
    }
}
