using System;

namespace SSBHLib
{
    public enum UltimateVertexAttribute
    {
        Position0,
        Normal0,
        Tangent0,
        Map1,
        UvSet,
        UvSet1,
        UvSet2,
        ColorSet1,
        ColorSet2,
        ColorSet21,
        ColorSet22,
        ColorSet23,
        ColorSet3,
        ColorSet4,
        ColorSet5,
        ColorSet6,
        ColorSet7,
        Bake1,
    }

    public static class EnumHelpers
    {
        public static UltimateVertexAttribute GetAttributeFromInGameString(string str)
        {
            switch (str)
            {
                case "Position0":
                    return UltimateVertexAttribute.Position0;
                case "Normal0":
                    return UltimateVertexAttribute.Normal0;
                case "Tangent0":
                    return UltimateVertexAttribute.Tangent0;
                case "map1":
                    return UltimateVertexAttribute.Map1;
                case "uvSet":
                    return UltimateVertexAttribute.UvSet;
                case "uvSet1":
                    return UltimateVertexAttribute.UvSet1;
                case "uvSet2":
                    return UltimateVertexAttribute.UvSet2;
                case "bake1":
                    return UltimateVertexAttribute.Bake1;
                case "colorSet1":
                    return UltimateVertexAttribute.ColorSet1;
                case "colorSet2":
                    return UltimateVertexAttribute.ColorSet2;
                case "colorSet2_1":
                    return UltimateVertexAttribute.ColorSet21;
                case "colorSet2_2":
                    return UltimateVertexAttribute.ColorSet22;
                case "colorSet2_3":
                    return UltimateVertexAttribute.ColorSet23;
                case "colorSet3":
                    return UltimateVertexAttribute.ColorSet3;
                case "colorSet4":
                    return UltimateVertexAttribute.ColorSet4;
                case "colorSet5":
                    return UltimateVertexAttribute.ColorSet5;
                case "colorSet6":
                    return UltimateVertexAttribute.ColorSet6;
                case "colorSet7":
                    return UltimateVertexAttribute.ColorSet7;
                default:
                    throw new NotImplementedException($"Unrecognized attribute {str}");
            }
        }

        /// <summary>
        /// Converts the attribute enum to the name used for Smash Ultimate's MESH format.
        /// </summary>
        /// <param name="attribute">The attribute to convert</param>
        /// <returns>The attribute name as used in the MESH format.</returns>
        public static string ToInGameString(this UltimateVertexAttribute attribute)
        {
            switch (attribute)
            {
                case UltimateVertexAttribute.Position0:
                    return "Position0";
                case UltimateVertexAttribute.Normal0:
                    return "Normal0";
                case UltimateVertexAttribute.Tangent0:
                    return "Tangent0";
                case UltimateVertexAttribute.Map1:
                    return "map1";
                case UltimateVertexAttribute.UvSet:
                    return "uvSet";
                case UltimateVertexAttribute.UvSet1:
                    return "uvSet1";
                case UltimateVertexAttribute.UvSet2:
                    return "uvSet2";
                case UltimateVertexAttribute.Bake1:
                    return "bake1";
                case UltimateVertexAttribute.ColorSet1:
                    return "colorSet1";
                case UltimateVertexAttribute.ColorSet2:
                    return "colorSet2";
                case UltimateVertexAttribute.ColorSet21:
                    return "colorSet2_1";
                case UltimateVertexAttribute.ColorSet22:
                    return "colorSet2_2";
                case UltimateVertexAttribute.ColorSet23:
                    return "colorSet2_3";
                case UltimateVertexAttribute.ColorSet3:
                    return "colorSet3";
                case UltimateVertexAttribute.ColorSet4:
                    return "colorSet4";
                case UltimateVertexAttribute.ColorSet5:
                    return "colorSet5";
                case UltimateVertexAttribute.ColorSet6:
                    return "colorSet6";
                case UltimateVertexAttribute.ColorSet7:
                    return "colorSet7";
                default:
                    throw new NotImplementedException($"Unrecognized attribute {attribute}");
            }
        }
    }
}
