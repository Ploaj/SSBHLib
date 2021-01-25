using SSBHLib;
using SSBHLib.Formats.Materials;
using System;
using System.IO;
using System.Xml.Serialization;

namespace MatLab
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("Usage: MatLab.exe <input> [output]");
                return;
            }

            string inputPath = args[0];
            string outputPath = GetFullPathWithoutExtension(inputPath);

            bool hasSpecifiedOutPath = args.Length == 2;
            if (hasSpecifiedOutPath)
                outputPath = args[1];

            ConvertFiles(inputPath, outputPath, hasSpecifiedOutPath);
        }

        public static void ConvertFiles(string inputPath, string outputPath, bool hasSpecifiedOutPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MaterialLibrary));
            switch (Path.GetExtension(inputPath))
            {
                case ".numatb":
                    if (hasSpecifiedOutPath)
                        SerializeMatl(inputPath, outputPath, serializer);
                    else
                        SerializeMatl(inputPath, outputPath + "_out.xml", serializer);
                    break;
                case ".xml":
                    if (hasSpecifiedOutPath)
                        DeserializeXml(inputPath, outputPath, serializer);
                    else
                        DeserializeXml(inputPath, outputPath + "_out.numatb", serializer);
                    break;
            }
        }

        private static void DeserializeXml(string inputPath, string outputPath, XmlSerializer serializer)
        {
            Console.WriteLine($"Converting {Path.GetFileName(inputPath)} to {outputPath}.numatb...");
            using (TextReader reader = new StringReader(File.ReadAllText(inputPath)))
            {
                var result = (MaterialLibrary)serializer.Deserialize(reader);

                Matl newmatl = LibraryToMATL(result);

                Ssbh.TrySaveSsbhFile(outputPath, newmatl);
            }
        }

        private static void SerializeMatl(string inputPath, string outputPath, XmlSerializer serializer)
        {
            Console.WriteLine($"Converting {Path.GetFileName(inputPath)} to {outputPath}...");
            if (Ssbh.TryParseSsbhFile(inputPath, out Matl matlFile))
            {
                MaterialLibrary library = MATLtoLibrary(matlFile);

                using (TextWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, library);
                    string serial = writer.ToString();
                    File.WriteAllText(outputPath, serial);
                }
            }
            else
            {
                Console.WriteLine("Error reading matl file");
            }
        }

        public static string GetFullPathWithoutExtension(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        }

        public static Matl LibraryToMATL(MaterialLibrary library)
        {
            Matl matl = new Matl
            {
                Entries = new MatlEntry[library.materials.Length]
            };

            for (int i = 0; i < library.materials.Length; i++)
            {
                MatlEntry entry = new MatlEntry
                {
                    MaterialLabel = library.materials[i].materialLabel,
                    ShaderLabel = library.materials[i].shaderLabel,
                    Attributes = new MatlAttribute[library.materials[i].parameter.Length]
                };

                for (int j = 0; j < library.materials[i].parameter.Length; j++)
                {
                    entry.Attributes[j] = new MatlAttribute
                    {
                        ParamId = library.materials[i].parameter[j].name,

                        DataObject = library.materials[i].parameter[j].value
                    };
                }

                matl.Entries[i] = entry;
            }

            return matl;
        }

        public static MaterialLibrary MATLtoLibrary(Matl matlFile)
        {
            MaterialLibrary library = new MaterialLibrary
            {
                materials = new Material[matlFile.Entries.Length]
            };

            int entryIndex = 0;
            foreach (var entry in matlFile.Entries)
            {
                Material mat = new Material();
                mat.shaderLabel = entry.ShaderLabel;
                mat.materialLabel = entry.MaterialLabel;

                mat.parameter = new MatlXmlAttribute[entry.Attributes.Length];

                int attribIndex = 0;
                foreach (var attr in entry.Attributes)
                {
                    MatlXmlAttribute attrib = new MatlXmlAttribute();
                    attrib.name = attr.ParamId;
                    attrib.value = attr.DataObject;
                    mat.parameter[attribIndex++] = attrib;
                }

                library.materials[entryIndex] = mat;
                entryIndex++;
            }

            return library;
        }
    }
}
