using SSBHLib.Formats.Materials;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SsbhLib.MatlXml
{
    public static class MatlToXml
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(MaterialLibrary));

        public static Matl DeserializeMatl(TextReader reader)
        {
            // TODO: This may fail, so return null on failure?
            var result = (MaterialLibrary)serializer.Deserialize(reader);
            return LibraryToMATL(result);
        }

        public static void SerializeMatl(TextWriter writer, Matl matl)
        {
            var library = MatlToLibrary(matl);
            serializer.Serialize(writer, library);
        }

        private static Matl LibraryToMATL(MaterialLibrary library)
        {
            var matl = new Matl
            {
                Entries = library.materials.Select(m => CreateEntry(m)).ToArray()
            };

            return matl;
        }

        private static MatlEntry CreateEntry(Material material)
        {
            var entry = new MatlEntry
            {
                MaterialLabel = material.materialLabel,
                ShaderLabel = material.shaderLabel,
                Attributes = material.parameters
                    .Select(p =>
                        new MatlAttribute
                        {
                            ParamId = p.name,
                            DataObject = p.value
                        })
                    .ToArray()
            };
            return entry;
        }

        private static MaterialLibrary MatlToLibrary(Matl matlFile)
        {
            var library = new MaterialLibrary
            {
                materials = matlFile.Entries.Select(e => CreateMaterial(e)).ToArray()
            };

            return library;
        }

        private static Material CreateMaterial(MatlEntry entry)
        {
            return new Material
            {
                shaderLabel = entry.ShaderLabel,
                materialLabel = entry.MaterialLabel,
                parameters = entry.Attributes
                    .Select(a =>
                        new MatlXmlAttribute
                        {
                            name = a.ParamId,
                            value = a.DataObject
                        })
                    .ToArray()
            };
        }
    }
}
