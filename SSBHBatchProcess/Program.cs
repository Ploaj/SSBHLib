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
                var xmb = new Xmb(@"F:\Yuzu Games\01006A800016E000\romfs\root\fighter\mario\model\body\c00\model.xmb");
                try
                {
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
