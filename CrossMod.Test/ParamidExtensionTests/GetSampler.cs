using CrossMod.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SSBHLib.Formats.Materials.MatlEnums;

namespace CrossMod.Test.ParamIdExtensionTests
{
    [TestClass]
    public class GetSampler
    {
        [TestMethod]
        public void Sampler0To15()
        {
            for (int i = 0; i < 15; i++)
            {
                Assert.AreEqual((ParamId)((int)ParamId.Sampler0+i), ParamIdExtensions.GetSampler((ParamId)((int)ParamId.Texture0 + i)));
            }
        }

        [TestMethod]
        public void Sampler16To19()
        {
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual((ParamId)((int)ParamId.Sampler16 + i), ParamIdExtensions.GetSampler((ParamId)((int)ParamId.Texture16 + i)));
            }
        }
    }
}
