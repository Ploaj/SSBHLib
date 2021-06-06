using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossMod.MaterialValidation.Test
{
    [TestClass]
    public class IsValidAttributeList
    {
        [TestMethod]
        public void InvalidShaderLabel()
        {
            Assert.IsFalse(ShaderValidation.IsValidAttributeList("SFX_PBS_0000000000000000",
                new string[] {
                    "Position0",
                    "Normal0",
                    "Tangent0",
                    "map1",
                    "colorSet1",
                    "colorSet2"
                })
            );
        }

        [TestMethod]
        public void ValidLabelAndAttributeList()
        {
            Assert.IsTrue(ShaderValidation.IsValidAttributeList("SFX_PBS_0100000008008269_opaque",
                new string[] {
                    "Position0",
                    "Normal0",
                    "Tangent0",
                    "map1",
                })
            );
        }

        [TestMethod]
        public void ValidLabelAndAttributeListInkMesh()
        {
            Assert.IsTrue(ShaderValidation.IsValidAttributeList("SFX_PBS_3500000008900280_opaque",
                new string[] {
                    "Position0",
                    "Normal0",
                    "Tangent0",
                    "colorSet2",
                    "colorSet4",
                    "map1",
                })
            );
        }

        [TestMethod]
        public void ValidLabelAndAttributeListDifferentOrder()
        {
            Assert.IsTrue(ShaderValidation.IsValidAttributeList("SFX_PBS_0100000008008269_opaque",
                new string[] {
                    "Normal0",
                    "Position0",
                    "map1",
                    "Tangent0",
                })
            );
        }

        [TestMethod]
        public void AdditionalAttribute()
        {
            Assert.IsTrue(ShaderValidation.IsValidAttributeList("SFX_PBS_1b01000008008a68_opaque",
                new string[] {
                    "Position0",
                    "Normal0",
                    "Tangent0",
                    "map1",
                    "colorSet3",
                    "colorSet1",
                    "colorSet2"
                })
            );
        }

        [TestMethod]
        public void MissingAttribute()
        {
            Assert.IsFalse(ShaderValidation.IsValidAttributeList("SFX_PBS_1b01000008008a68_opaque",
                new string[] {
                    "Position0",
                    "Normal0",
                    "Tangent0",
                    "colorSet1",
                    "colorSet2"
                })
            );
        }
    }
}
