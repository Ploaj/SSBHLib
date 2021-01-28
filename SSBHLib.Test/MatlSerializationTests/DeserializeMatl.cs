using Microsoft.VisualStudio.TestTools.UnitTesting;
using SsbhLib.MatlXml;
using SSBHLib.Formats.Materials;
using System.IO;

namespace SsbhLib.Test.MatlSerializationTests
{
    [TestClass]
    public class DeserializeMatl
    {
        [TestMethod]
        public void EmptyMatl()
        {
            var source = @"<?xml version=""1.0"" encoding=""utf-16""?>
<MaterialLibrary xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />";

            var reader = new StringReader(source);
            var matl = MatlSerialization.DeserializeMatl(reader);

            Assert.AreEqual(0, matl.Entries.Length);
        }
    }
}
