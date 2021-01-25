using CrossMod.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace CrossMod.Test.ParamIdExtensionTests
{
    [TestClass]
    public class ToSamplerIndex
    {
        [TestMethod]
        public void Sampler0()
        {
            Assert.AreEqual(0, ParamId.Sampler0.ToSamplerIndex());
        }

        [TestMethod]
        public void Sampler19()
        {
            Assert.AreEqual(19, ParamId.Sampler19.ToSamplerIndex());
        }

        [TestMethod]
        public void Sampler15()
        {
            Assert.AreEqual(15, ParamId.Sampler15.ToSamplerIndex());
        }

        [TestMethod]
        public void Sampler17()
        {
            Assert.AreEqual(17, ParamId.Sampler17.ToSamplerIndex());
        }

        [TestMethod]
        public void BelowSamplerRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.Sampler0 - 1).ToSamplerIndex());
        }

        [TestMethod]
        public void AboveSamplerRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.Sampler19 + 1).ToSamplerIndex());
        }
    }
}
