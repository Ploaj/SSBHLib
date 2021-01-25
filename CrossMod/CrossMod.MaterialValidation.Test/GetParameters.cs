using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class GetParameters
    {
        [TestMethod]
        public void InvalidShaderLabel()
        {
            CollectionAssert.AreEqual(new List<MatlEnums.ParamId>(), ShaderValidation.GetParameters("SFX_PBS_0000000000000000"));
        }

        [TestMethod]
        public void ValidShaderLabel()
        {
            CollectionAssert.AreEqual(
                new List<MatlEnums.ParamId> {
                    MatlEnums.ParamId.RasterizerState0,
                    MatlEnums.ParamId.BlendState0,
                    MatlEnums.ParamId.CustomVector13,
                    MatlEnums.ParamId.CustomVector8,
                    MatlEnums.ParamId.Texture0,
                    MatlEnums.ParamId.Sampler0,
                    MatlEnums.ParamId.Texture9,
                    MatlEnums.ParamId.Sampler9,
                },
                ShaderValidation.GetParameters("SFX_PBS_0000000000000080_opaque")
            );
        }
    }
}
