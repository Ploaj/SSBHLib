using SSBHLib;
using SSBHLib.Formats.Meshes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMBLib;

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

            foreach (var file in Directory.EnumerateFiles(sourceFolder, $"*.xmb", SearchOption.AllDirectories))
            {
                try
                {
                    var xmb = new Xmb(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(file);
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
