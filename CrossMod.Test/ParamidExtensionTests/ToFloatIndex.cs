using CrossMod.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace CrossMod.Test.ParamIdExtensionTests
{
    [TestClass]
    public class ToFloatIndex
    {
        [TestMethod]
        public void CustomFloat0()
        {
            Assert.AreEqual(0, ParamId.CustomFloat0.ToFloatIndex());
        }

        [TestMethod]
        public void CustomFloat19()
        {
            Assert.AreEqual(19, ParamId.CustomFloat19.ToFloatIndex());
        }

        [TestMethod]
        public void BelowFloatRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.CustomFloat0 - 1).ToFloatIndex());
        }

        [TestMethod]
        public void AboveFloatRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.CustomFloat19 + 1).ToFloatIndex());
        }
    }
}
