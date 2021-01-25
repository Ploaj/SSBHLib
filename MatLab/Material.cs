using System.Xml.Serialization;

namespace MatLab
{
    public class Material
    {
        [XmlAttribute]
        public string shaderLabel;

        [XmlAttribute]
        public string materialLabel;

        [XmlElement(ElementName = "Parameter")]
        public MatlXmlAttribute[] parameter;
    }
}
