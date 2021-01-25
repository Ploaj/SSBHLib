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

        [XmlElement("BlendState", typeof(MatlAttribute.MatlBlendState))]
        [XmlElement("RasterizerState", typeof(MatlAttribute.MatlRasterizerState))]
        [XmlElement("Vector4", typeof(MatlAttribute.MatlVector4))]
        [XmlElement("Sampler", typeof(MatlAttribute.MatlSampler))]
        [XmlElement("String", typeof(MatlAttribute.MatlString))]
        [XmlElement("UVtransform", typeof(MatlAttribute.MatlUvTransform))]
        [XmlElement("Float", typeof(float))]
        [XmlElement("Bool", typeof(bool))]
        public object value;
    }
}
