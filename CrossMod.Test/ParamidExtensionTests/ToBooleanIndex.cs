using CrossMod.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace CrossMod.Test.ParamIdExtensionTests
{
    [TestClass]
    public class ToBooleanIndex
    {
        [TestMethod]
        public void CustomBoolean0()
        {
            Assert.AreEqual(0, ParamId.CustomBoolean0.ToBooleanIndex());
        }

        [TestMethod]
        public void CustomBoolean19()
        {
            Assert.AreEqual(19, ParamId.CustomBoolean19.ToBooleanIndex());
        }

        [TestMethod]
        public void BelowBooleanRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.CustomBoolean0 - 1).ToBooleanIndex());
        }

        [TestMethod]
        public void AboveBooleanRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.CustomBoolean19 + 1).ToFloatIndex());
        }
    }
}
