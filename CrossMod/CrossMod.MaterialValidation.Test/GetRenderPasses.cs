using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class GetRenderPasses
    {
        [TestMethod]
        public void InvalidShaderLabel()
        {
            CollectionAssert.AreEqual(new List<string>(), ShaderValidation.GetRenderPasses("SFX_PBS_0000000000000000"));
        }

        [TestMethod]
        public void ValidShaderLabelWithTag()
        {
            CollectionAssert.AreEqual(
                new List<string> {
                    "opaque",
                    "sort",
                    "far",
                    "near"
                },
                ShaderValidation.GetRenderPasses("SFX_PBS_010002000800824f_opaque")
            );
        }

        [TestMethod]
        public void ValidShaderLabelNoTag()
        {
            CollectionAssert.AreEqual(
                new List<string> {
                    "opaque",
                    "sort",
                    "far",
                    "near"
                },
                ShaderValidation.GetRenderPasses("SFX_PBS_010002000800824f")
            );
        }
    }
}
