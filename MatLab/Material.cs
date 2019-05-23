using System.Xml.Serialization;

namespace MatLab
{
    public class Material
    {
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public string label;

        [XmlElement]
        public MatlXmlAttribute[] param;
    }
}
