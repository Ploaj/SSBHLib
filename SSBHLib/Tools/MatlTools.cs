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

                        // TODO: use an appropriate default texture if not found.
                        var textureName = currentValue?.Text ?? newTexture.Text ?? "";

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
