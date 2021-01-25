using System.Xml.Serialization;

namespace MatLab
{
    public class MaterialLibrary
    {
        [XmlElement(ElementName = "Material")]
        public Material[] materials;
    }
}
