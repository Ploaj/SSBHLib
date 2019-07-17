using CrossMod.Rendering;
using CrossMod.Tools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using SFGraphics.GLObjects.Textures;
using SFGraphics.GLObjects.Textures.TextureFormats;

namespace CrossMod.Nodes
{
    public enum NutexFormat
    {
        R8G8B8A8Unorm = 0,
        R8G8B8A8Srgb = 0x05,
        R32G32B32A32Float = 0x34,
        B8G8R8A8Unorm = 0x50,
        B8G8R8A8Srgb = 0x55,
        Bc1Unorm = 0x80,
        Bc1Srgb = 0x85,
        Bc2Unorm = 0x90,
        Bc2Srgb = 0x95,
        Bc3Unorm = 0xa0,
        Bc3Srgb = 0xa5,
        Bc4Unorm = 0xb0,
        Bc4Snorm = 0xb5,
        Bc5Unorm = 0xc0,
        Bc5Snorm = 0xc5,
        Bc6Ufloat = 0xd7,
        Bc7Unorm = 0xe0,
        Bc7Srgb = 0xe5
    }

    [FileTypeAttribute(".nutexb")]
    public class NutexNode : FileNode, IRenderableNode, IExportableTextureNode
    {
        public List<byte[]> Mipmaps;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        public NutexFormat Format;
        public string TexName { get; private set; }

        // Don't generate redundant textures.
        private RTexture renderableTexture = null;

        public readonly Dictionary<NutexFormat, InternalFormat> internalFormatByNuTexFormat = new Dictionary<NutexFormat, InternalFormat>()
        {
            { NutexFormat.R8G8B8A8Srgb, InternalFormat.SrgbAlpha },
            { NutexFormat.R8G8B8A8Unorm, InternalFormat.Rgba },
            { NutexFormat.R32G32B32A32Float, InternalFormat.Rgba },
            { NutexFormat.B8G8R8A8Unorm, InternalFormat.Rgba },
            { NutexFormat.B8G8R8A8Srgb, InternalFormat.Srgb },
            { NutexFormat.Bc1Unorm, InternalFormat.CompressedRgbaS3tcDxt1Ext },
            { NutexFormat.Bc1Srgb, InternalFormat.CompressedSrgbAlphaS3tcDxt1Ext },
            { NutexFormat.Bc2Unorm, InternalFormat.CompressedRgbaS3tcDxt3Ext },
            { NutexFormat.Bc2Srgb, InternalFormat.CompressedSrgbAlphaS3tcDxt3Ext },
            { NutexFormat.Bc3Unorm, InternalFormat.CompressedRgbaS3tcDxt5Ext },
            { NutexFormat.Bc3Srgb, InternalFormat.CompressedSrgbAlphaS3tcDxt5Ext },
            { NutexFormat.Bc4Unorm, InternalFormat.Rgba },
            { NutexFormat.Bc4Snorm, InternalFormat.Rgba },
            { NutexFormat.Bc5Unorm, InternalFormat.Rgba },
            { NutexFormat.Bc5Snorm, InternalFormat.Rgba },
            { NutexFormat.Bc6Ufloat, InternalFormat.CompressedRgbBptcUnsignedFloat },
            { NutexFormat.Bc7Unorm, InternalFormat.CompressedRgbaBptcUnorm },
            { NutexFormat.Bc7Srgb, InternalFormat.CompressedSrgbAlphaBptcUnorm }
        };

        /// <summary>
        /// Channel information for uncompressed formats.
        /// </summary>
        public readonly Dictionary<NutexFormat, PixelFormat> pixelFormatByNuTexFormat = new Dictionary<NutexFormat, PixelFormat>()
        {
            { NutexFormat.R8G8B8A8Srgb, PixelFormat.Rgba },
            { NutexFormat.R8G8B8A8Unorm, PixelFormat.Rgba },
            { NutexFormat.B8G8R8A8Unorm, PixelFormat.Bgra },
            { NutexFormat.B8G8R8A8Srgb, PixelFormat.Bgra },
        };

        public NutexNode(string path): base(path)
        {
            ImageKey = "texture";
            SelectedImageKey = "texture";
        }

        public override string ToString()
        {
            return Text.Contains(".") ? Text.Substring(0, Text.IndexOf(".")) : Text;
        }

        public override void Open()
        {
            using (BinaryReader reader  = new BinaryReader(new FileStream(AbsolutePath, FileMode.Open)))
            {
                Mipmaps = new List<byte[]>();
                // TODO: Why are there empty streams?
                if (reader.BaseStream.Length == 0)
                    return;

                reader.BaseStream.Position = reader.BaseStream.Length - 0xB0;


                int[] mipmapSizes = new int[16];
                for (int i = 0; i < mipmapSizes.Length; i++)
                    mipmapSizes[i] = reader.ReadInt32();

                reader.ReadChars(4); // TNX magic

                TexName = ReadTexName(reader);

                Width = reader.ReadInt32();
                Height = reader.ReadInt32();
                Depth = reader.ReadInt32();

                Format = (NutexFormat)reader.ReadByte();

                reader.ReadByte();

                ushort padding = reader.ReadUInt16();
                reader.ReadUInt32();

                int mipCount = reader.ReadInt32();
                int alignment = reader.ReadInt32();
                int arrayCount = reader.ReadInt32();
                int imageSize = reader.ReadInt32();
                char[] magic = reader.ReadChars(4);
                int majorVersion = reader.ReadInt16();
                int minorVersion = reader.ReadInt16();

                uint blkWidth = (uint)BlkDims[Format].X;
                uint blkHeight = (uint)BlkDims[Format].Y;

                uint blockHeight = SwitchSwizzler.GetBlockHeight(SwitchSwizzler.DivRoundUp((uint)Height, blkHeight));
                uint blockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;
                uint tileMode = 0;

                uint bpp = GetBpps(Format);

                reader.BaseStream.Position = 0;
                int blockHeightShift = 0;
                for (int i = 0; i < 1; i++)
                {
                    int size = mipmapSizes[i];

                    if (i == 0 && size % alignment != 0)
                        size += alignment - (size % alignment);

                    byte[] deswiz = SwitchSwizzler.Deswizzle((uint)Width, (uint)Height, blkWidth, blkHeight, 0, bpp, tileMode, (int)Math.Max(0, blockHeightLog2 - blockHeightShift), reader.ReadBytes(imageSize));
                    byte[] trimmed = new byte[mipmapSizes[0]];
                    Array.Copy(deswiz, 0, trimmed, 0, trimmed.Length);

                    Mipmaps.Add(trimmed);
                }
            }
        }

        private string ReadTexName(BinaryReader reader)
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

        public static readonly Dictionary<NutexFormat, Vector2> BlkDims = new Dictionary<NutexFormat, Vector2>()
        {
            { NutexFormat.B8G8R8A8Unorm, new Vector2(1, 1) },
            { NutexFormat.B8G8R8A8Srgb, new Vector2(1, 1) },
            { NutexFormat.R8G8B8A8Unorm, new Vector2(1, 1) },
            { NutexFormat.R8G8B8A8Srgb, new Vector2(1, 1) },
            { NutexFormat.R32G32B32A32Float, new Vector2(1, 1) },
            { NutexFormat.Bc1Unorm, new Vector2(4, 4) },
            { NutexFormat.Bc1Srgb, new Vector2(4, 4) },
            { NutexFormat.Bc2Unorm, new Vector2(4, 4) },
            { NutexFormat.Bc2Srgb, new Vector2(4, 4) },
            { NutexFormat.Bc3Unorm, new Vector2(4, 4) },
            { NutexFormat.Bc3Srgb, new Vector2(4, 4) },
            { NutexFormat.Bc4Unorm, new Vector2(1, 1) },
            { NutexFormat.Bc4Snorm, new Vector2(1, 1) },
            { NutexFormat.Bc5Unorm, new Vector2(1, 1) },
            { NutexFormat.Bc5Snorm, new Vector2(1, 1) },
            { NutexFormat.Bc6Ufloat, new Vector2(4, 4) },
            { NutexFormat.Bc7Srgb, new Vector2(4, 4) },
            { NutexFormat.Bc7Unorm, new Vector2(4, 4) },
        };

        public static uint GetBpps(NutexFormat format)
        {
            switch (format)
            {
                case NutexFormat.R8G8B8A8Unorm:
                case NutexFormat.R8G8B8A8Srgb:
                case NutexFormat.B8G8R8A8Unorm:
                    return 4;
                case NutexFormat.Bc1Unorm:
                    return 8;
                case NutexFormat.Bc1Srgb:
                    return 8;
                case NutexFormat.Bc4Unorm:
                    return 8;
                case NutexFormat.Bc4Snorm:
                    return 8;
                case NutexFormat.R32G32B32A32Float:
                case NutexFormat.Bc2Unorm:
                    return 8;
                case NutexFormat.Bc2Srgb:
                    return 8;
                case NutexFormat.Bc3Unorm:
                    return 16;
                case NutexFormat.Bc3Srgb:
                    return 16;
                case NutexFormat.Bc5Unorm:
                case NutexFormat.Bc5Snorm:
                case NutexFormat.Bc6Ufloat:
                case NutexFormat.Bc7Unorm:
                case NutexFormat.Bc7Srgb:
                    return 16;
                default:
                    return 0;
            }
        }

        public IRenderable GetRenderableNode()
        {
            // TODO: Some texture files are 0 bytes.
            if (Mipmaps.Count == 0)
                return null;

            // Don't initialize more than once.
            // We'll assume the context isn't destroyed.
            if (renderableTexture == null)
            {
                renderableTexture = new RTexture
                {
                    IsSrgb = Format.ToString().ToLower().Contains("srgb")
                };

                if (internalFormatByNuTexFormat.ContainsKey(Format))
                {
                    // This may require a higher OpenGL version for BC7.
                    if (!SFGraphics.GlUtils.OpenGLExtensions.IsAvailable("GL_ARB_texture_compression_bptc"))
                        throw new Rendering.Exceptions.MissingExtensionException("GL_ARB_texture_compression_bptc");

                    var sfTex = new Texture2D()
                    {
                        // Set defaults until all the sampler parameters are added.
                        TextureWrapS = TextureWrapMode.Repeat,
                        TextureWrapT = TextureWrapMode.Repeat
                    };

                    if (TextureFormatTools.IsCompressed(internalFormatByNuTexFormat[Format]))
                    {
                        sfTex.LoadImageData(Width, Height, Mipmaps, internalFormatByNuTexFormat[Format]);
                    }
                    else
                    {
                        var format = new TextureFormatUncompressed((PixelInternalFormat)internalFormatByNuTexFormat[Format], pixelFormatByNuTexFormat[Format], PixelType.UnsignedByte);
                        sfTex.LoadImageData(Width, Height, Mipmaps, format);
                    }

                    renderableTexture.renderTexture = sfTex;
                }
            }

            return renderableTexture;
        }

        public void SaveTexturePNG(string fileName)
        {
            System.Drawing.Bitmap texture = ((RTexture)GetRenderableNode()).renderTexture.GetBitmap(0);
            texture.Save(fileName);
            texture.Dispose();
        }
    }
}
