using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSBHLib.Formats.Materials;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class IsValidParameterList
    {
        [TestMethod]
        public void InvalidShaderLabel()
        {
            Assert.IsFalse(ShaderValidation.IsValidParameterList("SFX_PBS_0000000000000080_invalid",
                new MatlEnums.ParamId[] {
                    MatlEnums.ParamId.RasterizerState0,
                    MatlEnums.ParamId.BlendState0,
                    MatlEnums.ParamId.CustomVector13,
                    MatlEnums.ParamId.CustomVector8,
                    MatlEnums.ParamId.Texture0,
                    MatlEnums.ParamId.Sampler0,
                    MatlEnums.ParamId.Texture9,
                    MatlEnums.ParamId.Sampler9,
                })
            );
        }

        [TestMethod]
        public void ValidLabelAndParameterList()
        {
            Assert.IsTrue(ShaderValidation.IsValidParameterList("SFX_PBS_0000000000000080_opaque",
                new MatlEnums.ParamId[] {
                    MatlEnums.ParamId.RasterizerState0,
                    MatlEnums.ParamId.BlendState0,
                    MatlEnums.ParamId.CustomVector13,
                    MatlEnums.ParamId.CustomVector8,
                    MatlEnums.ParamId.Texture0,
                    MatlEnums.ParamId.Sampler0,
                    MatlEnums.ParamId.Texture9,
                    MatlEnums.ParamId.Sampler9,
                })
            );
        }

        [TestMethod]
        public void ValidLabelAndParameterListDifferentOrder()
        {
            Assert.IsTrue(ShaderValidation.IsValidParameterList("SFX_PBS_0000000000000080_opaque",
                new MatlEnums.ParamId[] {
                    MatlEnums.ParamId.CustomVector8,
                    MatlEnums.ParamId.BlendState0,
                    MatlEnums.ParamId.Sampler0,
                    MatlEnums.ParamId.RasterizerState0,
                    MatlEnums.ParamId.Texture9,
                    MatlEnums.ParamId.Texture0,
                    MatlEnums.ParamId.CustomVector13,
                    MatlEnums.ParamId.Sampler9,
                })
            );
        }

        [TestMethod]
        public void AdditionalParameter()
        {
            Assert.IsFalse(ShaderValidation.IsValidParameterList("SFX_PBS_0000000000000080_opaque",
                new MatlEnums.ParamId[] {
                    MatlEnums.ParamId.RasterizerState0,
                    MatlEnums.ParamId.BlendState0,
                    MatlEnums.ParamId.CustomVector13,
                    MatlEnums.ParamId.CustomVector8,
                    MatlEnums.ParamId.CustomVector9,
                    MatlEnums.ParamId.Texture0,
                    MatlEnums.ParamId.Sampler0,
                    MatlEnums.ParamId.Texture9,
                    MatlEnums.ParamId.Sampler9,
                })
            );
        }

        [TestMethod]
        public void MissingParameter()
        {
            Assert.IsFalse(ShaderValidation.IsValidParameterList("SFX_PBS_0000000000000080_opaque",
                new MatlEnums.ParamId[] {
                    MatlEnums.ParamId.RasterizerState0,
                    MatlEnums.ParamId.CustomVector13,
                    MatlEnums.ParamId.CustomVector8,
                    MatlEnums.ParamId.Texture0,
                    MatlEnums.ParamId.Sampler0,
                    MatlEnums.ParamId.Texture9,
                    MatlEnums.ParamId.Sampler9,
                })
            );
        }
    }
}
