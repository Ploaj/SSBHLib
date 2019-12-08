using Newtonsoft.Json;
using System;
using System.IO;

namespace NuanmbToJson
{
    class Program
    {
        private static string GetFullPathWithoutExtension(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        }

        private static void SerializeNuanim(string input, string output)
        {
            var nuanim = new NuanimData(input);
            var jsonText = JsonConvert.SerializeObject(nuanim, Formatting.Indented);
            File.WriteAllText(output, jsonText);
        }

        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("Usage: NuanmbToJson.exe <input> [output]");
                return;
            }

            if (args.Length == 2)
            {
                SerializeNuanim(args[0], args[1]);
            }
            else
            {
                var output = $"{GetFullPathWithoutExtension(args[0])}.json";
                SerializeNuanim(args[0], output);
            }
        }
    }
}
