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
    }
}
