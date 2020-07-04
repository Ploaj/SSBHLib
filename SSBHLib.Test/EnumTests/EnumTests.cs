using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSBHLib.Test.EnumTests
{
    [TestClass]
    public class EnumTests
    {
        [TestMethod]
        public void TestUltimateVertexAttributeTwoWayConversion()
        {
            // Make sure the naming is consistent.
            foreach (UltimateVertexAttribute value in Enum.GetValues(typeof(UltimateVertexAttribute)))
            {
                Assert.AreEqual(value, EnumHelpers.GetAttributeFromInGameString(value.ToInGameString()));
            }
        }
    }
}
