using CrossMod.Nodes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CrossMod.Tools
{
    public static class SwitchSwizzler
    {
        [DllImport("nutexb_swizzle", EntryPoint = "deswizzle_block_linear")]
        private static unsafe extern void DeswizzleBlockLinear(ulong width, ulong height, ulong depth, byte* source, ulong sourceLength, byte[] destination, ulong destinationLength, ulong blockHeight, ulong bytesPerPixel);

        [DllImport("nutexb_swizzle", EntryPoint = "block_height")]
        private static extern ulong GetBlockHeight(ulong height);

        [DllImport("nutexb_swizzle", EntryPoint = "swizzled_surface_size")]
        private static extern ulong GetSurfaceSize(ulong width, ulong height, ulong depth, ulong blockHeight, ulong bytesPerPixel);

        public static List<MipArray> GetImageData(SBSurface surface, byte[] imageData, int MipCount)
        {
            uint bpp = TextureFormatInfo.GetBPP(surface.InternalFormat);
            uint tileWidth = TextureFormatInfo.GetBlockWidth(surface.InternalFormat);
            uint tileHeight = TextureFormatInfo.GetBlockHeight(surface.InternalFormat);
            uint blkDepth = TextureFormatInfo.GetBlockDepth(surface.InternalFormat);

            int arrayOffset = 0;

            var arrays = new List<MipArray>();


            // TODO: Can this entire function be handled by Rust?
            for (int arrayLevel = 0; arrayLevel < surface.ArrayCount; arrayLevel++)
            {
                var mipmaps = new List<byte[]>();
                int surfaceSize = 0;

                for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                {
                    uint width = (uint)Math.Max(1, surface.Width >> mipLevel);
                    uint height = (uint)Math.Max(1, surface.Height >> mipLevel);
                    uint depth = (uint)Math.Max(1, surface.Depth >> mipLevel);

                    uint widthInTiles = DivRoundUp(width, tileWidth);
                    uint heightInTiles = DivRoundUp(height, tileHeight);

                    var blockHeight = GetBlockHeight(heightInTiles);
                    // TODO: Why do Pyra/Mythra have a strange block height for the first mip level?
                    if (mipLevel == 0 && height == 320 && bpp == 16)
                        blockHeight = 8;

                    // TODO: Poke Kalos Kamex (blastoise) also has a non standard block height?
                    if (mipLevel == 0 && height == 300 && bpp == 8)
                        blockHeight = 8;

                    var mipSize = GetSurfaceSize(widthInTiles, heightInTiles, depth, blockHeight, bpp);

                    // Use a span to avoid copying the memory each time.
                    var mipData = imageData.AsSpan()[(arrayOffset + surfaceSize)..];

                    // TODO: Handle errors?
                    var mipDataDeswizzled = Deswizzle(widthInTiles, heightInTiles, depth, bpp, blockHeight, mipData);
                    mipmaps.Add(mipDataDeswizzled);

                    surfaceSize += (int)mipSize;
                }

                var arr = new MipArray { Mipmaps = mipmaps };
                arrays.Add(arr);

                arrayOffset += imageData.Length / surface.ArrayCount;
            }

            return arrays;
        }

        public static readonly Dictionary<NUTEX_FORMAT, Vector2> TileDiminsions = new Dictionary<NUTEX_FORMAT, Vector2>()
        {
            { NUTEX_FORMAT.B8G8R8A8_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.B8G8R8A8_SRGB, new Vector2(1, 1) },
            { NUTEX_FORMAT.R8G8B8A8_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.R8G8B8A8_SRGB, new Vector2(1, 1) },
            { NUTEX_FORMAT.R32G32B32A32_FLOAT, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC1_UNORM, new Vector2(4, 4) },
            { NUTEX_FORMAT.BC1_SRGB, new Vector2(4, 4) },
            { NUTEX_FORMAT.BC2_UNORM, new Vector2(4, 4) },
            { NUTEX_FORMAT.BC2_SRGB, new Vector2(4, 4) },
            { NUTEX_FORMAT.BC3_UNORM, new Vector2(4, 4) },
            { NUTEX_FORMAT.BC3_SRGB, new Vector2(4, 4) },
            { NUTEX_FORMAT.BC4_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC4_SNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC5_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC5_SNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC6_UFLOAT, new Vector2(4, 4) },
            { NUTEX_FORMAT.BC7_SRGB, new Vector2(4, 4) },
            { NUTEX_FORMAT.BC7_UNORM, new Vector2(4, 4) },
        };

        public static uint GetBytesPerPixel(NUTEX_FORMAT format)
        {
            switch (format)
            {
                case NUTEX_FORMAT.R8G8B8A8_UNORM:
                case NUTEX_FORMAT.R8G8B8A8_SRGB:
                case NUTEX_FORMAT.B8G8R8A8_UNORM:
                    return 4;
                case NUTEX_FORMAT.BC1_UNORM:
                    return 8;
                case NUTEX_FORMAT.BC1_SRGB:
                    return 8;
                case NUTEX_FORMAT.BC4_UNORM:
                    return 8;
                case NUTEX_FORMAT.BC4_SNORM:
                    return 8;
                case NUTEX_FORMAT.R32G32B32A32_FLOAT:
                case NUTEX_FORMAT.BC2_UNORM:
                    return 8;
                case NUTEX_FORMAT.BC2_SRGB:
                    return 8;
                case NUTEX_FORMAT.BC3_UNORM:
                    return 16;
                case NUTEX_FORMAT.BC3_SRGB:
                    return 16;
                case NUTEX_FORMAT.BC5_UNORM:
                case NUTEX_FORMAT.BC5_SNORM:
                case NUTEX_FORMAT.BC6_UFLOAT:
                case NUTEX_FORMAT.BC7_UNORM:
                case NUTEX_FORMAT.BC7_SRGB:
                    return 16;
                default:
                    return 0;
            }
        }

        public static uint DivRoundUp(uint n, uint d)
        {
            return (n + d - 1) / d;
        }

        public static uint RoundUp(uint x, uint y)
        {
            return ((x - 1) | (y - 1)) + 1;
        }

        public static unsafe byte[] Deswizzle(uint width, uint height, uint depth, uint bpp, ulong blockHeight, Span<byte> data)
        {
            var output = new byte[width * height * bpp];

            fixed (byte* dataPtr = data)
            {
                DeswizzleBlockLinear(width, height, depth, dataPtr, (ulong)data.Length, output, (ulong)output.Length, blockHeight, bpp);
            }

            return output;
        }
    }
}
