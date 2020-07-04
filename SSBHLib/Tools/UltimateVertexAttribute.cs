using SSBHLib.Formats.Meshes;
using System;

namespace SSBHLib
{
    /// <summary>
    /// Stores the state for the different values of <see cref="MeshAttribute"/> used in Smash Ultimate.
    /// </summary>
    public class UltimateVertexAttribute
    {
        /// <summary>
        /// The default configuration for the Position0 attribute.
        /// </summary>
        public static UltimateVertexAttribute Position0 { get; } = new UltimateVertexAttribute("Position0", 3, MeshAttribute.AttributeDataType.Float, 0, 0);

        /// <summary>
        /// The default configuration for the Normal0 attribute.
        /// </summary>
        public static UltimateVertexAttribute Normal0 { get; } = new UltimateVertexAttribute("Normal0", 4, MeshAttribute.AttributeDataType.HalfFloat, 1, 0);

        /// <summary>
        /// The default configuration for the Tangent0 attribute.
        /// </summary>
        public static UltimateVertexAttribute Tangent0 { get; } = new UltimateVertexAttribute("Tangent0", 4, MeshAttribute.AttributeDataType.HalfFloat, 3, 0);

        /// <summary>
        /// The default configuration for the map1 attribute.
        /// </summary>
        public static UltimateVertexAttribute Map1 { get; } = new UltimateVertexAttribute("map1", 2, MeshAttribute.AttributeDataType.HalfFloat2, 4, 1);

        /// <summary>
        /// The default configuration for the uvSet attribute.
        /// </summary>
        public static UltimateVertexAttribute UvSet { get; } = new UltimateVertexAttribute("uvSet", 2, MeshAttribute.AttributeDataType.HalfFloat2, 4, 1);

        /// <summary>
        /// The default configuration for the uvSet1 attribute.
        /// </summary>
        public static UltimateVertexAttribute UvSet1 { get; } = new UltimateVertexAttribute("uvSet1", 2, MeshAttribute.AttributeDataType.HalfFloat2, 4, 1);

        /// <summary>
        /// The default configuration for the uvSet2 attribute.
        /// </summary>
        public static UltimateVertexAttribute UvSet2 { get; } = new UltimateVertexAttribute("uvSet2", 2, MeshAttribute.AttributeDataType.HalfFloat2, 4, 1);

        /// <summary>
        /// The default configuration for the colorSet1 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet1 { get; } = new UltimateVertexAttribute("colorSet1", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the colorSet2 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet2 { get; } = new UltimateVertexAttribute("colorSet2", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the colorSet2_1 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet21 { get; } = new UltimateVertexAttribute("colorSet2_1", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the colorSet22 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet22 { get; } = new UltimateVertexAttribute("colorSet2_2", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the colorSet23 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet23 { get; } = new UltimateVertexAttribute("colorSet2_3", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the colorSet3 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet3 { get; } = new UltimateVertexAttribute("colorSet3", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the colorSet4 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet4 { get; } = new UltimateVertexAttribute("colorSet4", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the colorSet5 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet5 { get; } = new UltimateVertexAttribute("colorSet5", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the colorSet6 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet6 { get; } = new UltimateVertexAttribute("colorSet6", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the colorSet7 attribute.
        /// </summary>
        public static UltimateVertexAttribute ColorSet7 { get; } = new UltimateVertexAttribute("colorSet7", 4, MeshAttribute.AttributeDataType.Byte, 5, 1);

        /// <summary>
        /// The default configuration for the bake1 attribute.
        /// </summary>
        public static UltimateVertexAttribute Bake1 { get; } = new UltimateVertexAttribute("bake1", 2, MeshAttribute.AttributeDataType.HalfFloat2, 4, 1);

        /// <summary>
        /// The in game name used in the MESH format.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The number of vector components. Color sets have 4 components, for example.
        /// </summary>
        public int ComponentCount { get; }

        /// <summary>
        /// The data type of each attribute component.
        /// </summary>
        public MeshAttribute.AttributeDataType DataType { get; }

        /// <summary>
        /// TODO: ???
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The buffer where the attribute values are stored.
        /// </summary>
        public int BufferIndex { get; }

        private UltimateVertexAttribute(string name, int componentCount, MeshAttribute.AttributeDataType dataType, int index, int bufferIndex)
        {
            Name = name;
            ComponentCount = componentCount;
            DataType = dataType;
            Index = index;
            BufferIndex = bufferIndex;
        }

        /// <summary>
        /// Finds a default attribute with in game name <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The in game name used in the mesh format</param>
        /// <returns>The default attribute with the given name</returns>
        public static UltimateVertexAttribute GetAttributeFromName(string name)
        {
            switch (name)
            {
                case "Position0":
                    return Position0;
                case "Normal0":
                    return Normal0;
                case "Tangent0":
                    return Tangent0;
                case "map1":
                    return Map1;
                case "uvSet":
                    return UvSet;
                case "uvSet1":
                    return UvSet1;
                case "uvSet2":
                    return UvSet2;
                case "bake1":
                    return Bake1;
                case "colorSet1":
                    return ColorSet1;
                case "colorSet2":
                    return ColorSet2;
                case "colorSet2_1":
                    return ColorSet21;
                case "colorSet2_2":
                    return ColorSet22;
                case "colorSet2_3":
                    return ColorSet23;
                case "colorSet3":
                    return ColorSet3;
                case "colorSet4":
                    return ColorSet4;
                case "colorSet5":
                    return ColorSet5;
                case "colorSet6":
                    return ColorSet6;
                case "colorSet7":
                    return ColorSet7;
                default:
                    throw new NotImplementedException($"Unrecognized attribute {name}");
            }
        }
    }
}
