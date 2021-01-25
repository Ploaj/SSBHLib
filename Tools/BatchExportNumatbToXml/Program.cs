using System;
using System.IO;

namespace BatchExportNumatbToXml
{
    static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: BatchExportNumatbToXml.exe <Source Folder> <Destination Folder>");
                return;
            }

            var sourceFolder = args[0];
            var destinationFolder = args[1];

            foreach (var file in Directory.EnumerateFiles(sourceFolder, "*.numatb*", SearchOption.AllDirectories))
            {
                var inputPath = file;
                var outputPath = GetOutputPath(inputPath, sourceFolder, destinationFolder);
                MatLab.Program.ConvertFiles(inputPath, outputPath, true);
            }
        }

        private static string GetOutputPath(string inputPath, string sourceFolder, string destinationFolder)
        {
            // source/a/b/c/file.numatb -> destination/a_b_c_file.xml
            var outputFile = inputPath
                .Replace(sourceFolder, "")
                .Substring(1)
                .Replace(Path.DirectorySeparatorChar, '_')
                .Replace(".numatb", ".xml");
            return Path.Combine(destinationFolder, outputFile);
        }
    }
}
