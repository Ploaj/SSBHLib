using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

// Classes ported from StudioSB
// https://github.com/Ploaj/StudioSB/blob/master/LICENSE
namespace CrossMod.Tools
{
    public class TextureFormatInfo
    {
        public static uint GetBPP(InternalFormat format)
        {
            return FormatTable[format].BytesPerPixel;
        }
        public static uint GetBlockWidth(InternalFormat format)
        {
            return FormatTable[format].BlockWidth;
        }
        public static uint GetBlockHeight(InternalFormat format)
        {
            return FormatTable[format].BlockHeight;
        }
        public static uint GetBlockDepth(InternalFormat format)
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

        private static readonly Dictionary<InternalFormat, FormatInfo> FormatTable =
                            new Dictionary<InternalFormat, FormatInfo>()
           {
            { InternalFormat.Rgba32f,                        new FormatInfo(16, 1, 1, 1) },
            { InternalFormat.Rgba32i,                        new FormatInfo(16, 1, 1, 1) },
            { InternalFormat.Rgba32ui,                       new FormatInfo(16, 1, 1, 1) },
            { InternalFormat.Rgba16f,                        new FormatInfo(8,  1, 1, 1) },
            { InternalFormat.Rgba16i,                        new FormatInfo(8,  1, 1, 1) },
            { InternalFormat.Rgba16ui,                       new FormatInfo(8,  1, 1, 1) }, 
            { InternalFormat.Rg32f,                          new FormatInfo(8,  1, 1, 1) },
            { InternalFormat.Rg32i,                          new FormatInfo(8,  1, 1, 1) },
            { InternalFormat.Rg32ui,                         new FormatInfo(8,  1, 1, 1) },
            { InternalFormat.Rgba8i,                         new FormatInfo(4,  1, 1, 1) },
            { InternalFormat.Srgb8Alpha8,                    new FormatInfo(4,  1, 1, 1) },
            { InternalFormat.Rgba8Snorm,                     new FormatInfo(4,  1, 1, 1) },
            { InternalFormat.Rgba8ui,                        new FormatInfo(4,  1, 1, 1) },
            { InternalFormat.Rgba8,                          new FormatInfo(4,  1, 1, 1) }, 
            { InternalFormat.SrgbAlpha,                      new FormatInfo(4,  1, 1, 1) }, 
            { InternalFormat.R32f,                           new FormatInfo(4,  1, 1, 1) }, 
            { InternalFormat.Rgb5A1,                         new FormatInfo(2,  1, 1, 1) }, 
            { InternalFormat.CompressedRgbaS3tcDxt1Ext,      new FormatInfo(8,  4, 4, 1) }, 
            { InternalFormat.CompressedSrgbAlphaS3tcDxt1Ext, new FormatInfo(8,  4, 4, 1) }, 
            { InternalFormat.CompressedRgbaS3tcDxt3Ext,      new FormatInfo(16, 4, 4, 1) },
            { InternalFormat.CompressedSrgbAlphaS3tcDxt3Ext, new FormatInfo(16, 4, 4, 1) },
            { InternalFormat.CompressedRgbaS3tcDxt5Ext,      new FormatInfo(16, 4, 4, 1) },
            { InternalFormat.CompressedSrgbAlphaS3tcDxt5Ext, new FormatInfo(16, 4, 4, 1) },
            { InternalFormat.CompressedRgbBptcUnsignedFloat, new FormatInfo(16, 4, 4, 1) },
            { InternalFormat.CompressedRgbaBptcUnorm,        new FormatInfo(16, 4, 4, 1) },
            { InternalFormat.CompressedSrgbAlphaBptcUnorm,   new FormatInfo(16, 4, 4, 1) },
        };
    }
}
