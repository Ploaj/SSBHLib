using SSBHLib.Formats.Materials;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SsbhLib.Tools
{
    // TODO: Make this part of MatlEntry?
    public static class MatlTools
    {
        public static MatlEntry FromShaderAndAttributes(MatlEntry currentEntry, MatlEntry newEntry, bool preserveTextureNames)
        {
            // TODO: Avoid conversion to array?
            var entry = new MatlEntry
            {
                MaterialLabel = currentEntry.MaterialLabel,
                ShaderLabel = newEntry.ShaderLabel,
                Attributes = GetAttributeList(currentEntry, newEntry, preserveTextureNames).ToArray()
            };

            return entry;
        }

        private static string GetDefaultTexture(MatlEnums.ParamId texture)
        {
            switch (texture)
            {
                case MatlEnums.ParamId.Texture0:
                case MatlEnums.ParamId.Texture1:
                case MatlEnums.ParamId.Texture10:
                case MatlEnums.ParamId.Texture11:
                case MatlEnums.ParamId.Texture12:
                    return "/common/shader/sfxPBS/default_Gray";
                case MatlEnums.ParamId.Texture2:
                case MatlEnums.ParamId.Texture7:
                case MatlEnums.ParamId.Texture8:
                    return "#replace_cubemap";
                case MatlEnums.ParamId.Texture4:
                case MatlEnums.ParamId.Texture16:
                    return "/common/shader/sfxPBS/default_Normal";
                case MatlEnums.ParamId.Texture6:
                    return "/common/shader/sfxPBS/default_Params";
                case MatlEnums.ParamId.Texture3:
                    return "/common/shader/sfxPBS/default_White";
                default:
                    return "/common/shader/sfxPBS/default_Black";
            }
        }

        private static List<MatlAttribute> GetAttributeList(MatlEntry currentEntry, MatlEntry newEntry, bool preserveTextureNames)
        {
            var attributes = new List<MatlAttribute>();

            if (newEntry.Attributes != null)
            {
                foreach (var newAttribute in newEntry.Attributes)
                {
                    if (newAttribute.DataObject is MatlAttribute.MatlString newTexture && preserveTextureNames)
                    {
                        var currentValue = currentEntry.Attributes.FirstOrDefault(a => a.ParamId == newAttribute.ParamId)?.DataObject as MatlAttribute.MatlString;

                        // Use an appropriate default texture if not found.
                        // This preserves the current appearance when applying a preset (ex: black for an emissive map).
                        var textureName = currentValue?.Text ?? GetDefaultTexture(newAttribute.ParamId);

                        attributes.Add(new MatlAttribute
                        {
                            ParamId = newAttribute.ParamId,
                            DataType = newAttribute.DataType,
                            DataObject = new MatlAttribute.MatlString { Text = textureName }
                        });
                    }
                    else
                    {
                        attributes.Add(new MatlAttribute
                        {
                            ParamId = newAttribute.ParamId,
                            DataType = newAttribute.DataType,
                            DataObject = newAttribute.DataObject
                        });
                    }
                }
            }

            return attributes;
        }
    }
}
