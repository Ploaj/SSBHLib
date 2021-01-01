using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class IsValidShaderLabel
    {
        [TestMethod]
        public void ValidLabel()
        {
            Assert.IsTrue(ShaderValidation.IsValidShaderLabel("SFX_PBS_1b01000008008a68_opaque"));
        }

        [TestMethod]
        public void InvalidTag()
        {
            Assert.IsFalse(ShaderValidation.IsValidShaderLabel("SFX_PBS_1b01000008008a68_near"));
        }

        [TestMethod]
        public void InvalidLabel()
        {
            Assert.IsFalse(ShaderValidation.IsValidShaderLabel("SFX_PBS_0000000000000000_opaque"));
        }

        [TestMethod]
        public void MissingTag()
        {
            Assert.IsFalse(ShaderValidation.IsValidShaderLabel("SFX_PBS_1b01000008008a68"));
        }

        [TestMethod]
        public void TerryShaders()
        {
            // Test DLC characters.
            Assert.IsTrue(ShaderValidation.IsValidShaderLabel("SFX_PBS_010000000808866b_opaque"));
            Assert.IsTrue(ShaderValidation.IsValidShaderLabel("SFX_PBS_0100000008018669_sort"));  
        }
    }
}
