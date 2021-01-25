using CrossMod.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace CrossMod.Test.ParamIdExtensionTests
{
    [TestClass]
    public class ToTextureIndex
    {
        [TestMethod]
        public void Texture0()
        {
            Assert.AreEqual(0, ParamId.Texture0.ToTextureIndex());
        }

        [TestMethod]
        public void Texture15()
        {
            Assert.AreEqual(15, ParamId.Texture15.ToTextureIndex());
        }

        [TestMethod]
        public void Texture17()
        {
            Assert.AreEqual(17, ParamId.Texture17.ToTextureIndex());
        }

        [TestMethod]
        public void Texture19()
        {
            Assert.AreEqual(19, ParamId.Texture19.ToTextureIndex());
        }

        [TestMethod]
        public void BelowTextureRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.Texture0 - 1).ToTextureIndex());
        }

        [TestMethod]
        public void AboveTextureRange()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => (ParamId)(ParamId.Texture19 + 1).ToTextureIndex());
        }
    }
}
