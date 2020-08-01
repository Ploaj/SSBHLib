using SSBHLib;
using SSBHLib.Formats.Meshes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSBHBatchProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: SSBHBatchProcess.exe <SSBH source folder>");
                return;
            }
            var sourceFolder = args[0];

            foreach (var file in Directory.EnumerateFiles(sourceFolder, $"*.nuanmb", SearchOption.AllDirectories))
            {
                try
                {
                    Ssbh.TryParseSsbhFile(file, out SsbhFile ssbhFile);
                }
                catch (Exception)
                {
                    //Console.WriteLine(file);
                    //Console.WriteLine(e.Message);
                }
            }
        }
    }
}
