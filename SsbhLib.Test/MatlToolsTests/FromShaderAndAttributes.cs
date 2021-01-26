using Microsoft.VisualStudio.TestTools.UnitTesting;
using SsbhLib.Tools;
using SSBHLib.Formats.Materials;

namespace SsbhLib.Test.MatlToolsTests
{
    [TestClass]
    public class FromShaderAndAttributes
    {
        // TODO: This should be a part of SSBHLib
        private MatlAttribute CreateTexture(MatlEnums.ParamId paramId, string value)
        {
            return new MatlAttribute
            {
                ParamId = paramId,
                DataType = MatlEnums.ParamDataType.String,
                DataObject = new MatlAttribute.MatlString { Text = value }
            };
        }

        private MatlAttribute CreateFloat(MatlEnums.ParamId paramId, float value)
        {
            return new MatlAttribute
            {
                ParamId = paramId,
                DataType = MatlEnums.ParamDataType.Float,
                DataObject = value
            };
        }

        [TestMethod]
        public void PreserveLabels()
        {
            var currentEntry = new MatlEntry
            {
                ShaderLabel = "currentShader",
                MaterialLabel = "currentMaterial"
            };

            var newEntry = new MatlEntry
            {
                ShaderLabel = "newShader",
                MaterialLabel = "newMaterial"
            };

            var result = MatlTools.FromShaderAndAttributes(currentEntry, newEntry, false);

            // Make sure existing material assignments to meshes aren't effected.
            Assert.AreEqual("newShader", result.ShaderLabel);
            Assert.AreEqual("currentMaterial", result.MaterialLabel);
        }

        [TestMethod]
        public void PreserveTextures()
        {
            var currentEntry = new MatlEntry
            {
                ShaderLabel = "currentShader",
                MaterialLabel = "currentMaterial",
                Attributes = new MatlAttribute[]
                {
                    CreateTexture(MatlEnums.ParamId.Texture0, "texture0_current"),
                    CreateTexture(MatlEnums.ParamId.Texture1, "texture1_current"),
                    CreateTexture(MatlEnums.ParamId.Texture2, "texture2_current"),
                    CreateFloat(MatlEnums.ParamId.CustomFloat0, 0.1f)
                }
            };

            var newEntry = new MatlEntry
            {
                ShaderLabel = "newShader",
                MaterialLabel = "newMaterial",
                Attributes = new MatlAttribute[]
                {
                    CreateTexture(MatlEnums.ParamId.Texture0, "texture0_new"),
                    CreateTexture(MatlEnums.ParamId.Texture1, "texture1_new"),
                    CreateTexture(MatlEnums.ParamId.Texture2, "texture2_new"),
                    CreateTexture(MatlEnums.ParamId.Texture3, "texture3_new"),
                    CreateFloat(MatlEnums.ParamId.CustomFloat0, 0.3f)
                }
            };

            var result = MatlTools.FromShaderAndAttributes(currentEntry, newEntry, true);

            // Preserve texture names but not material values.
            Assert.AreEqual("texture0_current", ((MatlAttribute.MatlString)result.Attributes[0].DataObject).Text);
            Assert.AreEqual("texture1_current", ((MatlAttribute.MatlString)result.Attributes[1].DataObject).Text);
            Assert.AreEqual("texture2_current", ((MatlAttribute.MatlString)result.Attributes[2].DataObject).Text);
            Assert.AreEqual("texture3_new", ((MatlAttribute.MatlString)result.Attributes[3].DataObject).Text);
            Assert.AreEqual(0.3f, (float)result.Attributes[4].DataObject);
        }

        [TestMethod]
        public void DoNotPreserveTextures()
        {
            var currentEntry = new MatlEntry
            {
                ShaderLabel = "currentShader",
                MaterialLabel = "currentMaterial",
                Attributes = new MatlAttribute[]
                {
                    CreateTexture(MatlEnums.ParamId.Texture0, "texture0_current"),
                    CreateTexture(MatlEnums.ParamId.Texture1, "texture1_current"),
                    CreateTexture(MatlEnums.ParamId.Texture2, "texture2_current"),
                    CreateFloat(MatlEnums.ParamId.CustomFloat0, 0.1f)
                }
            };

            var newEntry = new MatlEntry
            {
                ShaderLabel = "newShader",
                MaterialLabel = "newMaterial",
                Attributes = new MatlAttribute[]
                {
                    CreateTexture(MatlEnums.ParamId.Texture0, "texture0_new"),
                    CreateTexture(MatlEnums.ParamId.Texture1, "texture1_new"),
                    CreateTexture(MatlEnums.ParamId.Texture2, "texture2_new"),
                    CreateTexture(MatlEnums.ParamId.Texture3, "texture3_new"),
                    CreateFloat(MatlEnums.ParamId.CustomFloat0, 0.3f)
                }
            };

            var result = MatlTools.FromShaderAndAttributes(currentEntry, newEntry, false);

            // Don't preserve any material values.
            Assert.AreEqual("texture0_new", ((MatlAttribute.MatlString)result.Attributes[0].DataObject).Text);
            Assert.AreEqual("texture1_new", ((MatlAttribute.MatlString)result.Attributes[1].DataObject).Text);
            Assert.AreEqual("texture2_new", ((MatlAttribute.MatlString)result.Attributes[2].DataObject).Text);
            Assert.AreEqual("texture3_new", ((MatlAttribute.MatlString)result.Attributes[3].DataObject).Text);
            Assert.AreEqual(0.3f, (float)result.Attributes[4].DataObject);
        }
    }
}
