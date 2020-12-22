using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class HasColorSets
    {
        [TestMethod]
        public void ValidLabel()
        {
            Assert.IsTrue(ShaderValidation.HasColorSets("SFX_PBS_0000000000080089"));
        }

        [TestMethod]
        public void IncorrectlyIncludesTag()
        {
            Assert.IsFalse(ShaderValidation.HasColorSets("SFX_PBS_0000000000080089_opaque"));
        }

        [TestMethod]
        public void InvalidLabel()
        {
            Assert.IsFalse(ShaderValidation.HasColorSets("SFX_PBS_010140000a008a6b"));
        }
    }
}
