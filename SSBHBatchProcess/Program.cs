using SSBHLib;
using SSBHLib.Formats.Materials;
using SSBHLib.Formats.Meshes;
using SSBHLib.Formats.Animation;
using SSBHLib.Formats;
using System;
using System.IO;
using System.Linq;

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
                        TestFileExport<Matl>(file, file + "_out");
                        break;
                    case ".numshb":
                        TestFileExport<Mesh>(file, file + "_out");
                        break;
                    case ".numdlb":
                        TestFileExport<Modl>(file, file + "_out");
                        break;
                    case ".nusktb":
                        TestFileExport<Skel>(file, file + "_out");
                        break;
                    case ".nuanmb":
                        TestFileExport<Anim>(file, file + "_out");
                        break;
                    case ".nuhlpb":
                        TestFileExport<Hlpb>(file, file + "_out");
                        break;
                    default:
                        break;
                }
            }

        }

        private static void TestFileExport<T>(string input, string output) where T : SsbhFile
        {
            Ssbh.TryParseSsbhFile(input, out T file);
            Ssbh.TrySaveSsbhFile(output, file);
            if (!Enumerable.SequenceEqual(File.ReadAllBytes(input), File.ReadAllBytes(output)))
            {
                Console.WriteLine($"Files {input} and {output} do not match");
            }
            else
            {
                Console.WriteLine($"1:1 Export for type {typeof(T).Name}");
            }
        }
    }
}
