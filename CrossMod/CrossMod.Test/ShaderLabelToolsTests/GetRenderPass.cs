using CrossMod.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossMod.Test.ShaderLabelToolsTests
{
    [TestClass]
    public class GetRenderPass
    {
        [TestMethod]
        public void HasRenderPass()
        {
            Assert.AreEqual("opaque", ShaderLabelTools.GetRenderPass("SFX_PBS_0000000000000088_opaque"));
        }

        [TestMethod]
        public void NoRenderPass()
        {
            Assert.AreEqual("", ShaderLabelTools.GetRenderPass("SFX_PBS_0000000000000088"));
        }

        [TestMethod]
        public void Empty()
        {
            Assert.AreEqual("", ShaderLabelTools.GetRenderPass(""));
            Assert.AreEqual("", ShaderLabelTools.GetRenderPass(null));
        }
    }
}
