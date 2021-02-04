using CrossMod.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossMod.Test.ShaderLabelToolsTests
{
    [TestClass]
    public class WithRenderPass
    {
        [TestMethod]
        public void ReplaceRenderPass()
        {
            Assert.AreEqual("SFX_PBS_0000000000000088_sort", 
                ShaderLabelTools.WithRenderPass("SFX_PBS_0000000000000088_opaque", "sort"));
        }

        [TestMethod]
        public void RemoveRenderPass()
        {
            Assert.AreEqual("SFX_PBS_0000000000000088_",
                ShaderLabelTools.WithRenderPass("SFX_PBS_0000000000000088_opaque", ""));
        }

        [TestMethod]
        public void AddRenderPass()
        {
            Assert.AreEqual("SFX_PBS_0000000000000088_near",
                ShaderLabelTools.WithRenderPass("SFX_PBS_0000000000000088", "near"));
        }

        [TestMethod]
        public void Empty()
        {
            Assert.AreEqual("_opaque", ShaderLabelTools.WithRenderPass("", "opaque"));
            Assert.AreEqual("_opaque", ShaderLabelTools.WithRenderPass(null, "opaque"));
        }
    }
}
