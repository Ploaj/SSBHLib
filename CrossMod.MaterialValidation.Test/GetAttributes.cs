using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class GetAttributes
    {
        [TestMethod]
        public void InvalidShaderLabel()
        {
            CollectionAssert.AreEqual(new List<string>(), ShaderValidation.GetAttributes("SFX_PBS_0000000000000000"));
        }

        [TestMethod]
        public void ValidShaderLabel()
        {
            CollectionAssert.AreEqual(
                new List<string> {
                    "colorSet2",
                    "map1",
                }, 
                ShaderValidation.GetAttributes("SFX_PBS_1b01000008008a68_opaque")
            );
        }
    }
}
