using CrossMod.Nodes;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace CrossMod.Tools
{
    public class TextureFormatInfo
    {
        public static uint GetBPP(NUTEX_FORMAT format)
        {
            return FormatTable[format].BytesPerPixel;
        }
        public static uint GetBlockWidth(NUTEX_FORMAT format)
        {
            return FormatTable[format].BlockWidth;
        }
        public static uint GetBlockHeight(NUTEX_FORMAT format)
        {
            return FormatTable[format].BlockHeight;
        }
        public static uint GetBlockDepth(NUTEX_FORMAT format)
        {
            return FormatTable[format].BlockDepth;
        }

        private class FormatInfo
        {
            public uint BytesPerPixel { get; private set; }
            public uint BlockWidth { get; private set; }
            public uint BlockHeight { get; private set; }
            public uint BlockDepth { get; private set; }

            public FormatInfo(uint bytesPerPixel, uint blockWidth, uint blockHeight, uint blockDepth)
            {
                BytesPerPixel = bytesPerPixel;
                BlockWidth = blockWidth;
                BlockHeight = blockHeight;
                BlockDepth = blockDepth;
            }
        }

        private static readonly Dictionary<NUTEX_FORMAT, FormatInfo> FormatTable =
                            new Dictionary<NUTEX_FORMAT, FormatInfo>()
        {
            { NUTEX_FORMAT.R8_UNORM, new FormatInfo(1, 1, 1, 1) },
            { NUTEX_FORMAT.R8G8B8A8_UNORM, new FormatInfo(4, 1, 1, 1) },
            { NUTEX_FORMAT.R8G8B8A8_SRGB, new FormatInfo(4, 1, 1, 1) },
            { NUTEX_FORMAT.R32G32B32A32_FLOAT, new FormatInfo(16, 1, 1, 1) },
            { NUTEX_FORMAT.B8G8R8A8_UNORM, new FormatInfo(4, 1, 1, 1) },
            { NUTEX_FORMAT.B8G8R8A8_SRGB, new FormatInfo(4, 1, 1, 1) },
            { NUTEX_FORMAT.BC1_UNORM, new FormatInfo(8, 4, 4, 1) },
            { NUTEX_FORMAT.BC1_SRGB, new FormatInfo(8, 4, 4, 1) },
            { NUTEX_FORMAT.BC2_UNORM, new FormatInfo(16, 4, 4, 1) },
            { NUTEX_FORMAT.BC2_SRGB, new FormatInfo(16, 4, 4, 1) },
            { NUTEX_FORMAT.BC3_UNORM, new FormatInfo(16, 4, 4, 1) },
            { NUTEX_FORMAT.BC3_SRGB, new FormatInfo(16, 4, 4, 1) },
            { NUTEX_FORMAT.BC4_UNORM, new FormatInfo(8, 4, 4, 1) },
            { NUTEX_FORMAT.BC4_SNORM, new FormatInfo(8, 4, 4, 1) },
            { NUTEX_FORMAT.BC5_UNORM, new FormatInfo(16, 4, 4, 1) },
            { NUTEX_FORMAT.BC5_SNORM, new FormatInfo(16, 4, 4, 1) },
            { NUTEX_FORMAT.BC6_UFLOAT, new FormatInfo(16, 4, 4, 1) },
            { NUTEX_FORMAT.BC6_SFLOAT, new FormatInfo(16, 4, 4, 1) },
            { NUTEX_FORMAT.BC7_UNORM, new FormatInfo(16, 4, 4, 1) },
            { NUTEX_FORMAT.BC7_SRGB, new FormatInfo(16, 4, 4, 1) },
        };
    }
}
