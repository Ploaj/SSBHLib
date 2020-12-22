using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class HasAttribute
    {
        [TestMethod]
        public void ValidAttributeAndLabel()
        {
            Assert.IsTrue(ShaderValidation.HasAttribute("SFX_PBS_1b01000008008a68_opaque", "colorSet1"));
        }

        [TestMethod]
        public void InvalidLabel()
        {
            Assert.IsFalse(ShaderValidation.HasAttribute("SFX_PBS_1b01000008008a68_near", "colorSet1"));
        }

        [TestMethod]
        public void MissingTag()
        {
            Assert.IsFalse(ShaderValidation.HasAttribute("SFX_PBS_1b01000008008a68", "colorSet1"));
        }

        [TestMethod]
        public void InvalidAttribute()
        {
            Assert.IsFalse(ShaderValidation.HasAttribute("SFX_PBS_1b01000008008a68_opaque", "colorSet10"));
        }
    }
}
