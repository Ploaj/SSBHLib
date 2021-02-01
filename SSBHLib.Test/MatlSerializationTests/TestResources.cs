using System.IO;
using System.Reflection;

namespace SsbhLib.Test.MatlSerializationTests
{
    internal static class TestResources
    {
        public static string ReadResourceTextFile(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
