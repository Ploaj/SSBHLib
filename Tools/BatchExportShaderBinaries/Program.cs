using System;
using System.IO;
using SSBHLib.Formats.Rendering;

namespace BatchExportShaderBinaries
{
    static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: BatchExportShaderBinaries.exe <Source Folder> <Destination Folder>");
                return;
            }

            var sourceFolder = args[0];
            var destinationFolder = args[1];

            foreach (var file in Directory.EnumerateFiles(sourceFolder, "*.nushdb*", SearchOption.AllDirectories))
            {
                if (!SSBHLib.Ssbh.TryParseSsbhFile<Shdr>(File.ReadAllBytes(file), out Shdr shdr))
                    continue;

                foreach (var shader in shdr.Shaders)
                {
                    var outputPath = Path.Combine(destinationFolder, $"{shader.Name}.bin");
                    File.WriteAllBytes(outputPath, shader.ShaderBinary);
                    Console.WriteLine(outputPath);
                }
            }
        }
    }
}
