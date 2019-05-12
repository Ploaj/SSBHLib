using System;
using SSBHLib;
using SSBHLib.Formats.Materials;
using System.Xml.Serialization;
using System.IO;

namespace MatLab
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            string inputPath = args[0];
            string path = GetFullPathWithoutExtension(inputPath);

            ConvertFiles(inputPath, path);
        }

        private static void ConvertFiles(string inputPath, string outputPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MaterialLibrary));
            switch (Path.GetExtension(inputPath))
            {
                case ".numatb":
                    SerializeMatl(inputPath, outputPath, serializer);
                    break;
                case ".xml":
                    DeserializeXml(inputPath, outputPath, serializer);
                    break;
            }
        }

        private static void DeserializeXml(string inputPath, string outputPath, XmlSerializer serializer)
        {
            Console.WriteLine($"Converting {Path.GetFileName(inputPath)} to {outputPath + "_out.numatb"}...");
            using (TextReader reader = new StringReader(File.ReadAllText(inputPath)))
            {
                var result = (MaterialLibrary)serializer.Deserialize(reader);

                MATL newmatl = LibraryToMATL(result);

                SSBH.TrySaveSSBHFile(outputPath + "_out.numatb", newmatl);
            }
        }

        private static void SerializeMatl(string inputPath, string outputPath, XmlSerializer serializer)
        {
            Console.WriteLine($"Converting {Path.GetFileName(inputPath)} to {outputPath + "_out.xml"}...");
            if (SSBH.TryParseSSBHFile(inputPath, out ISSBH_File file))
            {
                MATL matlFile = (MATL)file;

                MaterialLibrary library = MATLtoLibrary(matlFile);

                using (TextWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, library);
                    string serial = writer.ToString();
                    File.WriteAllText(outputPath + "_out.xml", serial);
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

        public static MATL LibraryToMATL(MaterialLibrary library)
        {
            MATL matl = new MATL
            {
                Entries = new MatlEntry[library.material.Length]
            };

            for (int i = 0; i < library.material.Length; i++)
            {
                MatlEntry entry = new MatlEntry
                {
                    MaterialLabel = library.material[i].label,
                    MaterialName = library.material[i].name,
                    Attributes = new MatlAttribute[library.material[i].param.Length]
                };

                for (int j = 0; j < library.material[i].param.Length; j++)
                {
                    entry.Attributes[j] = new MatlAttribute
                    {
                        ParamID = library.material[i].param[j].name,

                        DataObject = library.material[i].param[j].Value
                    };
                }

                matl.Entries[i] = entry;
            }

            return matl;
        }

        public static MaterialLibrary MATLtoLibrary(MATL matlFile)
        {
            MaterialLibrary library = new MaterialLibrary
            {
                material = new Material[matlFile.Entries.Length]
            };

            int entryIndex = 0;
            foreach (var entry in matlFile.Entries)
            {
                Material mat = new Material();
                mat.name = entry.MaterialName;
                mat.label = entry.MaterialLabel;

                mat.param = new MatlXmlAttribute[entry.Attributes.Length];

                int attribIndex = 0;
                foreach (var attr in entry.Attributes)
                {
                    MatlXmlAttribute attrib = new MatlXmlAttribute();
                    attrib.name = attr.ParamID;
                    attrib.Value = attr.DataObject;
                    mat.param[attribIndex++] = attrib;
                }

                library.material[entryIndex] = mat;
                entryIndex++;
            }

            return library;
        }
    }
}
