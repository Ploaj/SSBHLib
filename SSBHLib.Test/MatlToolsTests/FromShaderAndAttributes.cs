using Microsoft.VisualStudio.TestTools.UnitTesting;
using SsbhLib.Tools;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

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
        public void DefaultTextures()
        {
            var currentEntry = new MatlEntry
            {
                ShaderLabel = "currentShader",
                MaterialLabel = "currentMaterial",
                Attributes = new MatlAttribute[0]
            };

            var newEntry = new MatlEntry
            {
                ShaderLabel = "newShader",
                MaterialLabel = "newMaterial",
                Attributes = new MatlAttribute[]
                {
                    CreateTexture(MatlEnums.ParamId.Texture0, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture1, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture2, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture3, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture4, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture5, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture6, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture7, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture8, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture9, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture10, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture11, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture12, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture13, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture14, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture15, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture16, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture17, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture18, "texture"),
                    CreateTexture(MatlEnums.ParamId.Texture19, "texture"),

                }
            };


            var result = MatlTools.FromShaderAndAttributes(currentEntry, newEntry, true);
            // Missing textures should use a default value.
            Assert.AreEqual("/common/shader/sfxPBS/default_Gray", ((MatlAttribute.MatlString)result.Attributes[0].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Gray", ((MatlAttribute.MatlString)result.Attributes[1].DataObject).Text);
            Assert.AreEqual("#replace_cubemap", ((MatlAttribute.MatlString)result.Attributes[2].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_White", ((MatlAttribute.MatlString)result.Attributes[3].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Normal", ((MatlAttribute.MatlString)result.Attributes[4].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Black", ((MatlAttribute.MatlString)result.Attributes[5].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Params", ((MatlAttribute.MatlString)result.Attributes[6].DataObject).Text);
            Assert.AreEqual("#replace_cubemap", ((MatlAttribute.MatlString)result.Attributes[7].DataObject).Text);
            Assert.AreEqual("#replace_cubemap", ((MatlAttribute.MatlString)result.Attributes[8].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Black", ((MatlAttribute.MatlString)result.Attributes[9].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Gray", ((MatlAttribute.MatlString)result.Attributes[10].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Gray", ((MatlAttribute.MatlString)result.Attributes[11].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Gray", ((MatlAttribute.MatlString)result.Attributes[12].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Black", ((MatlAttribute.MatlString)result.Attributes[13].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Black", ((MatlAttribute.MatlString)result.Attributes[14].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Black", ((MatlAttribute.MatlString)result.Attributes[15].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Normal", ((MatlAttribute.MatlString)result.Attributes[16].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Black", ((MatlAttribute.MatlString)result.Attributes[17].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Black", ((MatlAttribute.MatlString)result.Attributes[18].DataObject).Text);
            Assert.AreEqual("/common/shader/sfxPBS/default_Black", ((MatlAttribute.MatlString)result.Attributes[19].DataObject).Text);
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
                    CreateFloat(MatlEnums.ParamId.CustomFloat0, 0.3f)
                }
            };

            var result = MatlTools.FromShaderAndAttributes(currentEntry, newEntry, true);

            // Preserve texture names but not material values.
            Assert.AreEqual("texture0_current", ((MatlAttribute.MatlString)result.Attributes[0].DataObject).Text);
            Assert.AreEqual("texture1_current", ((MatlAttribute.MatlString)result.Attributes[1].DataObject).Text);
            Assert.AreEqual("texture2_current", ((MatlAttribute.MatlString)result.Attributes[2].DataObject).Text);
            Assert.AreEqual(0.3f, (float)result.Attributes[3].DataObject);
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
