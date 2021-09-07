using CrossMod.Tools;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Textures;
using SFGraphics.GLObjects.Textures.TextureFormats;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

// Classes ported from StudioSB
// https://github.com/Ploaj/StudioSB/blob/master/LICENSE
namespace CrossMod.Nodes
{
    public class MipArray
    {
        public List<byte[]> Mipmaps = new List<byte[]>();
    }

    /// <summary>
    /// Descripts a texture surface object
    /// </summary>
    public class SBSurface
    {
        public string Name { get; set; }

        public List<MipArray> Arrays = new List<MipArray>();

        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }

        public TextureTarget TextureTarget { get; set; }
        public PixelFormat PixelFormat { get; set; }
        public PixelType PixelType { get; set; }
        public InternalFormat InternalFormat { get; set; }

        public int ArrayCount { get; set; } = 1;

        public bool IsCubeMap { get { return Arrays.Count == 6; } }

        public bool IsSRGB
        {
            get
            {
                return (InternalFormat.ToString().ToLower().Contains("srgb"));
            }
        }

        private Texture renderTexture = null;

        public SBSurface()
        {

        }

        public static SBSurface? FromNutexb(string FilePath)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                // TODO: Why are there empty streams?
                if (reader.BaseStream.Length == 0)
                    return null;

                SBSurface surface = new SBSurface();

                reader.BaseStream.Position = reader.BaseStream.Length - 0xB0;

                int[] mipmapSizes = new int[16];
                for (int i = 0; i < mipmapSizes.Length; i++)
                    mipmapSizes[i] = reader.ReadInt32();

                reader.ReadChars(4); // TNX magic

                string texName = ReadTexName(reader);
                surface.Name = texName;

                surface.Width = reader.ReadInt32();
                surface.Height = reader.ReadInt32();
                surface.Depth = reader.ReadInt32();

                var Format = (NUTEX_FORMAT)reader.ReadByte();

                reader.ReadByte();

                ushort Padding = reader.ReadUInt16();
                reader.ReadUInt32();

                int MipCount = reader.ReadInt32();
                int Alignment = reader.ReadInt32();
                surface.ArrayCount = reader.ReadInt32();
                int ImageSize = reader.ReadInt32();
                char[] Magic = reader.ReadChars(4);
                int MajorVersion = reader.ReadInt16();
                int MinorVersion = reader.ReadInt16();

                if (pixelFormatByNuTexFormat.ContainsKey(Format))
                    surface.PixelFormat = pixelFormatByNuTexFormat[Format];

                if (internalFormatByNuTexFormat.ContainsKey(Format))
                    surface.InternalFormat = internalFormatByNuTexFormat[Format];

                surface.PixelType = GetPixelType(Format);

                reader.BaseStream.Position = 0;
                byte[] ImageData = reader.ReadBytes(ImageSize);

                try
                {
                    surface.Arrays = SwitchSwizzler.GetImageData(surface, ImageData, MipCount);
                }
                catch (System.Exception)
                {
                    // TODO: Swizzling is really unstable.
                    return null;
                }


                return surface;
            }
        }


        public void RefreshRendering()
        {
            // settings this to null with make the texture reload next time the GetRenderTexture is called
            renderTexture = null;
        }

        public static SBSurface FromBitmap(Bitmap bmp)
        {
            SBSurface surface = new SBSurface();

            surface.Name = "";
            surface.Width = bmp.Width;
            surface.Height = bmp.Height;
            surface.InternalFormat = InternalFormat.Rgba;
            surface.PixelFormat = PixelFormat.Bgra;
            surface.PixelType = PixelType.UnsignedByte;

            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var length = bitmapData.Stride * bitmapData.Height;

            byte[] bytes = new byte[length];

            Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
            bmp.UnlockBits(bitmapData);

            var mip = new MipArray();
            mip.Mipmaps.Add(bytes);

            surface.Arrays.Add(mip);

            return surface;
        }

        /// <summary>
        /// Gets the SFTexture of this surface
        /// </summary>
        /// <returns></returns>
        public Texture GetRenderTexture()
        {
            // TODO: This is a mess.
            if (renderTexture == null)
            {
                if (Arrays.Count == 6)
                {
                    var cube = new TextureCubeMap()
                    {
                        MinFilter = TextureMinFilter.Linear,
                        MagFilter = TextureMagFilter.Linear
                    };
                    if (TextureFormatTools.IsCompressed(InternalFormat))
                    {
                        cube.LoadImageData(Width, InternalFormat,
                            Arrays[0].Mipmaps, Arrays[1].Mipmaps, Arrays[2].Mipmaps,
                            Arrays[3].Mipmaps, Arrays[4].Mipmaps, Arrays[5].Mipmaps);
                    }
                    else
                    {
                        // TODO: The internal format should only be rgb, rgba, etc. A cast won't always work.
                        var format = new TextureFormatUncompressed((PixelInternalFormat)InternalFormat, PixelFormat, PixelType);
                        cube.LoadImageData(Width, format,
                            Arrays[0].Mipmaps[0], Arrays[1].Mipmaps[0], Arrays[2].Mipmaps[0],
                            Arrays[3].Mipmaps[0], Arrays[4].Mipmaps[0], Arrays[5].Mipmaps[0]);
                    }
                    renderTexture = cube;
                }
                else
                {
                    var sfTex = new Texture2D()
                    {
                        // Set defaults until all the sampler parameters are added.
                        TextureWrapS = TextureWrapMode.Repeat,
                        TextureWrapT = TextureWrapMode.Repeat,
                    };

                    if (TextureFormatTools.IsCompressed(InternalFormat))
                    {
                        // HACK: trying to load mipmaps with similar sizes seems to not work
                        var mipTest = new List<byte[]>();
                        int prevsize = 0;
                        foreach (var v in Arrays[0].Mipmaps)
                        {
                            if (v.Length == prevsize)
                                continue;
                            mipTest.Add(v);
                            prevsize = v.Length;
                        }
                        sfTex.LoadImageData(Width, Height, mipTest, InternalFormat);
                    }
                    else
                    {
                        // TODO: Uncompressed mipmaps
                        var format = new TextureFormatUncompressed((PixelInternalFormat)InternalFormat, PixelFormat, PixelType);
                        sfTex.LoadImageData(Width, Height, Arrays[0].Mipmaps, format);
                    }
                    renderTexture = sfTex;
                }
            }
            return renderTexture;
        }

        private static string ReadTexName(BinaryReader reader)
        {
            var result = "";
            for (int i = 0; i < 0x40; i++)
            {
                byte b = reader.ReadByte();
                if (b != 0)
                    result += (char)b;
            }

            return result;
        }

        private static readonly Dictionary<NUTEX_FORMAT, InternalFormat> internalFormatByNuTexFormat = new Dictionary<NUTEX_FORMAT, InternalFormat>()
        {
            { NUTEX_FORMAT.R8G8B8A8_SRGB, InternalFormat.Srgb8Alpha8 },
            { NUTEX_FORMAT.R8G8B8A8_UNORM, InternalFormat.Rgba8 },
            { NUTEX_FORMAT.R32G32B32A32_FLOAT, InternalFormat.Rgba32f },
            { NUTEX_FORMAT.B8G8R8A8_UNORM, InternalFormat.Rgba8 },
            { NUTEX_FORMAT.B8G8R8A8_SRGB, InternalFormat.Rgba8Snorm },
            { NUTEX_FORMAT.BC1_UNORM, InternalFormat.CompressedRgbaS3tcDxt1Ext },
            { NUTEX_FORMAT.BC1_SRGB, InternalFormat.CompressedSrgbAlphaS3tcDxt1Ext },
            { NUTEX_FORMAT.BC2_UNORM, InternalFormat.CompressedRgbaS3tcDxt3Ext },
            { NUTEX_FORMAT.BC2_SRGB, InternalFormat.CompressedSrgbAlphaS3tcDxt3Ext },
            { NUTEX_FORMAT.BC3_UNORM, InternalFormat.CompressedRgbaS3tcDxt5Ext },
            { NUTEX_FORMAT.BC3_SRGB, InternalFormat.CompressedSrgbAlphaS3tcDxt5Ext },
            { NUTEX_FORMAT.BC4_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC4_SNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC5_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC5_SNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC6_UFLOAT, InternalFormat.CompressedRgbBptcUnsignedFloat },
            { NUTEX_FORMAT.BC7_UNORM, InternalFormat.CompressedRgbaBptcUnorm },
            { NUTEX_FORMAT.BC7_SRGB, InternalFormat.CompressedSrgbAlphaBptcUnorm }
        };

        private static PixelType GetPixelType(NUTEX_FORMAT format)
        {
            return format switch
            {
                NUTEX_FORMAT.R32G32B32A32_FLOAT => PixelType.Float,
                _ => PixelType.UnsignedByte,
            };
        }

        /// <summary>
        /// Channel information for uncompressed formats.
        /// </summary>
        private static readonly Dictionary<NUTEX_FORMAT, PixelFormat> pixelFormatByNuTexFormat = new Dictionary<NUTEX_FORMAT, PixelFormat>()
        {
            { NUTEX_FORMAT.R32G32B32A32_FLOAT, PixelFormat.Rgba },
            { NUTEX_FORMAT.R8G8B8A8_SRGB, PixelFormat.Rgba },
            { NUTEX_FORMAT.R8G8B8A8_UNORM, PixelFormat.Rgba },
            { NUTEX_FORMAT.B8G8R8A8_UNORM, PixelFormat.Bgra },
            { NUTEX_FORMAT.B8G8R8A8_SRGB, PixelFormat.Bgra },
        };
    }

    public enum NUTEX_FORMAT
    {
        R8G8B8A8_UNORM = 0,
        R8G8B8A8_SRGB = 0x05,
        R32G32B32A32_FLOAT = 0x34,
        B8G8R8A8_UNORM = 0x50,
        B8G8R8A8_SRGB = 0x55,
        BC1_UNORM = 0x80,
        BC1_SRGB = 0x85,
        BC2_UNORM = 0x90,
        BC2_SRGB = 0x95,
        BC3_UNORM = 0xa0,
        BC3_SRGB = 0xa5,
        BC4_UNORM = 0xb0,
        BC4_SNORM = 0xb5,
        BC5_UNORM = 0xc0,
        BC5_SNORM = 0xc5,
        BC6_UFLOAT = 0xd7,
        BC7_UNORM = 0xe0,
        BC7_SRGB = 0xe5
    }
}
