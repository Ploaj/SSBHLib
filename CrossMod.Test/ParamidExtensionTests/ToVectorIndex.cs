using CrossMod.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace CrossMod.Test.ParamIdExtensionTests
{
    [TestClass]
    public class ToVectorIndex
    {
        [TestMethod]
        public void CustomVector0()
        {
            Assert.AreEqual(0, ParamId.CustomVector0.ToVectorIndex());
        }

        [TestMethod]
        public void CustomVector19()
        {
            Assert.AreEqual(19, ParamId.CustomVector19.ToVectorIndex());
        }

        [TestMethod]
        public void BelowVectorRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.CustomVector0 - 1).ToVectorIndex());
        }

        [TestMethod]
        public void AboveVectorRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.CustomVector63 + 1).ToVectorIndex());
        }
    }
}
