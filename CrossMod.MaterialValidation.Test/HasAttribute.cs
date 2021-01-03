using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class HasAttribute
    {
        [TestMethod]
        public void ValidAttributeAndLabel()
        {
            Assert.IsTrue(ShaderValidation.HasAttribute("SFX_PBS_0100000008008269_opaque", "map1"));
        }

        [TestMethod]
        public void InvalidLabel()
        {
            Assert.IsFalse(ShaderValidation.HasAttribute("SFX_PBS_1b01000008008a68_invalid", "colorSet1"));
        }

        [TestMethod]
        public void MissingTag()
        {
            Assert.IsTrue(ShaderValidation.HasAttribute("SFX_PBS_0100000008008269", "map1"));
        }

        [TestMethod]
        public void InvalidAttribute()
        {
            Assert.IsFalse(ShaderValidation.HasAttribute("SFX_PBS_0100000008008269_opaque", "colorSet1"));
        }
    }
}
