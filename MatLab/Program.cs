using SsbhLib.MatlXml;
using SSBHLib;
using SSBHLib.Formats.Materials;
using System;
using System.IO;

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
            switch (Path.GetExtension(inputPath))
            {
                case ".numatb":
                    if (hasSpecifiedOutPath)
                        SerializeMatl(inputPath, outputPath);
                    else
                        SerializeMatl(inputPath, outputPath + "_out.xml");
                    break;
                case ".xml":
                    if (hasSpecifiedOutPath)
                        DeserializeXml(inputPath, outputPath);
                    else
                        DeserializeXml(inputPath, outputPath + "_out.numatb");
                    break;
            }
        }

        private static void DeserializeXml(string inputPath, string outputPath)
        {
            Console.WriteLine($"Converting {Path.GetFileName(inputPath)} to {outputPath}.numatb...");
            using (var reader = new StringReader(File.ReadAllText(inputPath)))
            {
                var matl = MatlSerialization.DeserializeMatl(reader);
                Ssbh.TrySaveSsbhFile(outputPath, matl);
            }
        }

        private static void SerializeMatl(string inputPath, string outputPath)
        {
            Console.WriteLine($"Converting {Path.GetFileName(inputPath)} to {outputPath}...");
            if (Ssbh.TryParseSsbhFile(inputPath, out Matl matlFile))
            {
                using (var writer = new StringWriter())
                {
                    MatlSerialization.SerializeMatl(writer, matlFile);
                    File.WriteAllText(outputPath, writer.ToString());
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
    }
}
