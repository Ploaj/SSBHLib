using System.Xml.Serialization;

namespace SsbhLib.MatlXml
{
    public class Material
    {
        [XmlAttribute]
        public string shaderLabel;

        [XmlAttribute]
        public string materialLabel;

        [XmlElement(ElementName = "Parameter")]
        public MatlXmlAttribute[] parameters;
    }
}
