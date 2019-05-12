using System.Xml.Serialization;

namespace MatLab
{
    public class MaterialLibrary
    {
        [XmlElement]
        public Material[] material;
    }
}
