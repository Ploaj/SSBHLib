using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class HasAttribute
    {
        [TestMethod]
        public void ValidAttributeAndLabel()
        {
            Assert.IsTrue(ShaderValidation.HasAttribute("colorSet1", "SFX_PBS_1b01000008008a68_opaque"));
        }

        [TestMethod]
        public void InvalidLabel()
        {
            Assert.IsFalse(ShaderValidation.HasAttribute("colorSet1", "SFX_PBS_1b01000008008a68_near"));
        }

        [TestMethod]
        public void MissingTag()
        {
            Assert.IsFalse(ShaderValidation.HasAttribute("colorSet1", "SFX_PBS_1b01000008008a68"));
        }

        [TestMethod]
        public void InvalidAttribute()
        {
            Assert.IsFalse(ShaderValidation.HasAttribute("colorSet10", "SFX_PBS_1b01000008008a68_opaque"));
        }
    }
}
