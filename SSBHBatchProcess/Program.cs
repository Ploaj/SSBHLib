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
            TestFileExport<Matl>(args[0], args[1]);
            TestFileExport<Mesh>(args[2], args[3]);
            TestFileExport<Anim>(args[4], args[5]);
            TestFileExport<Modl>(args[6], args[7]);
        }

        private static void TestFileExport<T>(string input, string output) where T : SsbhFile
        {
            Ssbh.TryParseSsbhFile(input, out T file);
            Ssbh.TrySaveSsbhFile(output, file);
            if (!Enumerable.SequenceEqual(File.ReadAllBytes(input), File.ReadAllBytes(output)))
            {
                Console.WriteLine($"Files {input} and {output} do not match");
            }
        }
    }
}
