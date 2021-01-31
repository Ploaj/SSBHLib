using Microsoft.VisualStudio.TestTools.UnitTesting;
using SsbhLib.MatlXml;
using System.IO;
using System.Reflection;

namespace SsbhLib.Test.MatlSerializationTests
{
    [TestClass]
    public class DeserializeMatl
    {
        private static string ReadResourceTextFile(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        [TestMethod]
        public void EmptyMatl()
        {
            var source = @"<?xml version=""1.0"" encoding=""utf-16""?>
<MaterialLibrary xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />";

            var reader = new StringReader(source);
            var matl = MatlSerialization.DeserializeMatl(reader);

            Assert.AreEqual(0, matl.Entries.Length);
        }

        [TestMethod]
        public void DeserializeMarioC00()
        {
            var source = ReadResourceTextFile("SsbhLib.Test.MatlSerializationTests.marioc00.xml");

            var reader = new StringReader(source);
            var matl = MatlSerialization.DeserializeMatl(reader);

            Assert.AreEqual(12, matl.Entries.Length);
            var entry = matl.Entries[5];
            Assert.AreEqual("SFX_PBS_010000001800830d_opaque", entry.ShaderLabel);
            Assert.AreEqual("EyeRD", entry.MaterialLabel);

            Assert.AreEqual(20, entry.Attributes.Length);
            Assert.AreEqual(false, entry.Attributes[3].DataObject);
        }
    }
}
