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

            string matlPath = args[0];
            string path = GetFullPathWithoutExtension(matlPath);

            XmlSerializer x = new XmlSerializer(typeof(material_library));
            switch (Path.GetExtension(matlPath))
            {
                case ".numatb":

                    Console.WriteLine($"Converting {Path.GetExtension(matlPath)} to .xml...");
                    ISSBH_File file;
                    if (SSBH.TryParseSSBHFile(matlPath, out file))
                    {
                        MATL matlFile = (MATL)file;

                        material_library library = MATLtoLibrary(matlFile);

                        using (TextWriter writer = new StringWriter())
                        {
                            x.Serialize(writer, library);
                            string serial = writer.ToString();
                            File.WriteAllText(path + "_out.xml", serial);
                        }
                    }
                    else
                        Console.WriteLine("Error reading matl file");

                    break;
                case ".xml":
                    Console.WriteLine($"Converting {Path.GetExtension(matlPath)} to .numatb");
                    using (TextReader reader = new StringReader(File.ReadAllText(matlPath)))
                    {
                        var result = (material_library)x.Deserialize(reader);

                        MATL newmatl = LibraryToMATL(result);

                        SSBH.TrySaveSSBHFile(path + "_out.numatb", newmatl);
                    }
                    break;
            }
            
            Console.WriteLine("Done, press enter to exit");
            Console.ReadLine();
        }

        public static String GetFullPathWithoutExtension(String path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        }

        public static MATL LibraryToMATL(material_library library)
        {
            MATL matl = new MATL();

            matl.Entries = new MatlEntry[library.material.Length];

            for(int i = 0; i < library.material.Length; i++)
            {
                MatlEntry entry = new MatlEntry();

                entry.MaterialLabel = library.material[i].label;
                entry.MaterialName = library.material[i].name;
                entry.Attributes = new MatlAttribute[library.material[i].param.Length];

                for (int j = 0; j < library.material[i].param.Length; j++)
                {
                    entry.Attributes[j] = new MatlAttribute();

                    entry.Attributes[j].ParamID = library.material[i].param[j].name;

                    entry.Attributes[j].DataObject = library.material[i].param[j].Value;
                }

                matl.Entries[i] = entry;
            }

            return matl;
        }

        public static material_library MATLtoLibrary(MATL matlFile)
        {
            material_library library = new material_library();
            library.material = new material[matlFile.Entries.Length];

            int entryIndex = 0;
            foreach (var entry in matlFile.Entries)
            {
                material mat = new material();
                mat.name = entry.MaterialName;
                mat.label = entry.MaterialLabel;

                mat.param = new attribute[entry.Attributes.Length];

                int attribIndex = 0;
                foreach (var attr in entry.Attributes)
                {
                    attribute attrib = new attribute();
                    attrib.name = attr.ParamID;
                    attrib.Value = attr.DataObject;
                    mat.param[attribIndex++] = attrib;
                }

                library.material[entryIndex] = mat;
                entryIndex++;
            }

            return library;
        }

        public class material_library
        {
            [XmlElement]
            public material[] material;
        }

        public class material
        {
            [XmlAttribute]
            public string name;

            [XmlAttribute]
            public string label;

            [XmlElement]
            public attribute[] param;
        }

        [XmlInclude(typeof(MatlAttribute.MatlBlendState)), XmlInclude(typeof(MatlAttribute.MatlRasterizerState))
            , XmlInclude(typeof(MatlAttribute.MatlVector4)), XmlInclude(typeof(MatlAttribute.MatlSampler))
            , XmlInclude(typeof(MatlAttribute.MatlString)), XmlInclude(typeof(MatlAttribute.MatlUVTransform))]
        public class attribute
        {
            [XmlAttribute]
            public MatlEnums.ParamId name;

            [XmlElement("blend_state", typeof(MatlAttribute.MatlBlendState))]
            [XmlElement("rasterizer_state", typeof(MatlAttribute.MatlRasterizerState))]
            [XmlElement("vector4", typeof(MatlAttribute.MatlVector4))]
            [XmlElement("sampler", typeof(MatlAttribute.MatlSampler))]
            [XmlElement("string", typeof(MatlAttribute.MatlString))]
            [XmlElement("UVtransform", typeof(MatlAttribute.MatlUVTransform))]
            [XmlElement("float", typeof(float))]
            [XmlElement("bool", typeof(bool))]
            public object Value;
        }
    }
}
