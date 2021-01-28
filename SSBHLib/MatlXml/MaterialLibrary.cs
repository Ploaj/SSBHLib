using System.Xml.Serialization;

namespace SsbhLib.MatlXml
{
    public class MaterialLibrary
    {
        [XmlElement(ElementName = "Material")]
        public Material[] materials;
    }
}
