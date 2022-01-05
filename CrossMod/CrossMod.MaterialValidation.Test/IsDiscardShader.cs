using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class IsDiscardShader
    {
        [TestMethod]
        public void ValidLabelDiscard()
        {
            Assert.IsTrue(ShaderValidation.IsDiscardShader("SFX_PBS_0100080008048269_opaque"));
        }

        [TestMethod]
        public void InvalidTag()
        {
            // Only valid tags are removed.
            Assert.IsFalse(ShaderValidation.IsDiscardShader("SFX_PBS_0100080008048269_invalid"));
        }

        [TestMethod]
        public void InvalidLabel()
        {
            Assert.IsFalse(ShaderValidation.IsDiscardShader("SFX_PBS_0000000000000000_opaque"));
        }

        [TestMethod]
        public void MissingTag()
        {
            Assert.IsTrue(ShaderValidation.IsDiscardShader("SFX_PBS_0100080008048269"));
        }

        [TestMethod]
        public void ValidLabelNoDiscard()
        {
            Assert.IsFalse(ShaderValidation.IsDiscardShader("SFX_PBS_0100000008008269_opaque"));
        }
    }
}
