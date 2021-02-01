using Microsoft.VisualStudio.TestTools.UnitTesting;
using SsbhLib.MatlXml;
using SSBHLib.Formats.Materials;
using System.IO;

namespace SsbhLib.Test.MatlSerializationTests
{
    [TestClass]
    public class SerializeMatl
    {
        [TestMethod]
        public void EmptyMatl()
        {
            var matl = new Matl();
            var writer = new StringWriter();
            MatlSerialization.SerializeMatl(writer, matl);
            var expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<MaterialLibrary xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />";
            Assert.AreEqual(expected, writer.ToString()); 
        }

        [TestMethod]
        public void SerializeMarioC00()
        {
            var source = TestResources.ReadResourceTextFile("SsbhLib.Test.MatlSerializationTests.marioc00.xml");

            var reader = new StringReader(source);
            var matl = MatlSerialization.DeserializeMatl(reader);

            var writer = new StringWriter();
            MatlSerialization.SerializeMatl(writer, matl);

            // Test 1:1 deserialize/serialize
            Assert.AreEqual(source, writer.ToString());
        }
    }
}
