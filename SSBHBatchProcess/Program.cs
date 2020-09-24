using SSBHLib;
using SSBHLib.Formats.Materials;
using SSBHLib.Formats.Meshes;
using SSBHLib.Formats.Animation;
using SSBHLib.Formats;
using System;
using System.IO;
using System.Linq;
using SSBHLib.IO;

namespace SSBHBatchProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test exporting to make sure it's still 1:1. 
            foreach (var file in Directory.EnumerateFiles(args[0], "*.*"))
            {
                if (file.EndsWith("_out"))
                    continue;
                var extension = Path.GetExtension(file);

                switch (extension)
                {
                    case ".numatb":
                        TestFileExport<Matl>(file);
                        break;
                    case ".numshb":
                        TestFileExport<Mesh>(file);
                        break;
                    case ".numdlb":
                        TestFileExport<Modl>(file);
                        break;
                    case ".nusktb":
                        TestFileExport<Skel>(file);
                        break;
                    case ".nuanmb":
                        TestFileExport<Anim>(file);
                        break;
                    case ".nuhlpb":
                        TestFileExport<Hlpb>(file);
                        break;
                    default:
                        break;
                }
            }

        }

        private static void TestFileExport<T>(string input) where T : SsbhFile
        {
            Ssbh.TryParseSsbhFile(input, out T file);

            var stream = new MemoryStream();
            using (var exporter = new SsbhExporter(stream))
            {
                exporter.WriteSsbhFile(file);
            }

            if (!Enumerable.SequenceEqual(File.ReadAllBytes(input), stream.ToArray()))
            {
                Console.WriteLine($"{input} did not export 1:1");
            }
            else
            {
                Console.WriteLine($"1:1 Export for type {typeof(T).Name}");
            }
        }
    }
}
