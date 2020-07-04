using SSBHLib.Formats.Meshes;
using System;

namespace SSBHLib
{
    public static class EnumHelpers
    {
        /// <summary>
        /// Gets the size in bytes for this data type.
        /// </summary>
        /// <param name="dataType">The data type for each of the attribute's components</param>
        /// <returns>The number of bytes used to store <paramref name="dataType"/></returns>
        public static int GetSizeInBytes(this MeshAttribute.AttributeDataType dataType)
        {
            switch (dataType)
            {
                case MeshAttribute.AttributeDataType.Float:
                    return sizeof(float);
                case MeshAttribute.AttributeDataType.Byte:
                    return sizeof(byte);
                case MeshAttribute.AttributeDataType.HalfFloat:
                    return 2;
                case MeshAttribute.AttributeDataType.HalfFloat2:
                    return 2;
                default:
                    throw new NotImplementedException($"Size not implemented for {dataType}");
            }
        }
    }
}
