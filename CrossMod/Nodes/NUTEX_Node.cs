using CrossMod.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;
using CrossMod.Tools;
using OpenTK;

namespace CrossMod.Nodes
{
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

    [FileTypeAttribute(".nutexb")]
    public class NUTEX_Node : FileNode, IRenderableNode
    {
        public List<byte[]> Mipmaps;
        public int Width;
        public int Height;
        public NUTEX_FORMAT Format;
        public string TexName;

        // TODO: Fix these formats.
        public readonly Dictionary<NUTEX_FORMAT, InternalFormat> glFormatByNuTexFormat = new Dictionary<NUTEX_FORMAT, InternalFormat>()
        {
            { NUTEX_FORMAT.R8G8B8A8_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.R8G8B8A8_SRGB, InternalFormat.Rgba },
            { NUTEX_FORMAT.R32G32B32A32_FLOAT, InternalFormat.Rgba },
            { NUTEX_FORMAT.B8G8R8A8_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.B8G8R8A8_SRGB, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC1_UNORM, InternalFormat.CompressedRgbaS3tcDxt1Ext },
            { NUTEX_FORMAT.BC1_SRGB, InternalFormat.CompressedSrgbAlphaS3tcDxt1Ext },
            { NUTEX_FORMAT.BC2_UNORM, InternalFormat.CompressedRgbaS3tcDxt3Ext },
            { NUTEX_FORMAT.BC2_SRGB, InternalFormat.CompressedSrgbAlphaS3tcDxt3Ext },
            { NUTEX_FORMAT.BC3_UNORM, InternalFormat.CompressedRgbaS3tcDxt5Ext },
            { NUTEX_FORMAT.BC3_SRGB, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC4_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC4_SNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC5_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC5_SNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC6_UFLOAT, InternalFormat.CompressedRgbBptcUnsignedFloat },
            { NUTEX_FORMAT.BC7_UNORM, InternalFormat.CompressedRgbaBptcUnorm },
            { NUTEX_FORMAT.BC7_SRGB, InternalFormat.CompressedSrgbAlphaBptcUnorm }
        };

        public NUTEX_Node()
        {
            ImageKey = "texture";
            SelectedImageKey = "texture";
        }

        public override void Open(string Path)
        {
            //hingadingadurgan
            using (BinaryReader R  = new BinaryReader(new FileStream(Path, FileMode.Open)))
            {
                Mipmaps = new List<byte[]>();
                R.BaseStream.Position = R.BaseStream.Length - 0xB0;
                int[] MipSizes = new int[16];
                for (int i = 0; i < MipSizes.Length; i++)
                    MipSizes[i] = R.ReadInt32();
                R.ReadChars(4); // TNX magic
                TexName = "";
                for(int i = 0; i < 0x40; i++)
                {
                    byte b = R.ReadByte();
                    if (b != 0)
                        TexName += (char)b;
                }
                Width = R.ReadInt32();
                Height = R.ReadInt32();
                int Unk = R.ReadInt32();
                Format = (NUTEX_FORMAT)R.ReadByte();
                R.ReadByte();
                ushort Padding = R.ReadUInt16();
                R.ReadUInt32();
                int MipCount = R.ReadInt32();
                int Alignment = R.ReadInt32();
                int ArrayCount = R.ReadInt32();
                int ImageSize = R.ReadInt32();
                char[] Magic = R.ReadChars(4);
                int MajorVersion = R.ReadInt16();
                int MinorVersion = R.ReadInt16();


                if (Format != NUTEX_FORMAT.BC7_UNORM && Format != NUTEX_FORMAT.BC7_SRGB)
                    return;

                uint blkWidth = (uint)blkdims[Format].X;
                uint blkHeight = (uint)blkdims[Format].Y;

                uint blockHeight = SwitchSwizzler.GetBlockHeight(SwitchSwizzler.DIV_ROUND_UP((uint)Height, blkHeight));
                uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;
                uint tileMode = 0;

                //int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

                uint bpp = bpps(Format);

                R.BaseStream.Position = 0;
                int blockHeightShift = 0;
                for (int i = 0; i < 1; i++)
                {
                    int size = MipSizes[i];

                    if (i == 0)
                        if (size % Alignment != 0)
                            size = size + (Alignment - (size % Alignment));

                    //if (SwitchSwizzler.pow2_round_up(SwitchSwizzler.DIV_ROUND_UP((uint)Height, blkWidth)) < linesPerBlockHeight)
                    //    blockHeightShift += 1;
                    //Console.WriteLine($"{Path} {Width} {Height} {bpp} {Format} {size} {MipSizes[0]} {blkWidth} {blkHeight}");
                    try
                    {
                        //Mipmaps.Add(SwitchSwizzler.deswizzle((uint)(Width >> i), (uint)(Height >> i), blkWidth, blkHeight, 1, bpp, tileMode, (int)Math.Max(0, BlockHeightLog2 - blockHeightShift), R.ReadBytes(MipSizes[i])));
                        byte[] deswiz = SwitchSwizzler.deswizzle((uint)Width, (uint)Height, blkWidth, blkHeight, 0, bpp, tileMode, (int)Math.Max(0, BlockHeightLog2 - blockHeightShift), R.ReadBytes(ImageSize));
                        byte[] trimmed = new byte[MipSizes[0]];
                        Array.Copy(deswiz, 0, trimmed, 0, trimmed.Length);

                        Mipmaps.Add(trimmed);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public static Dictionary<NUTEX_FORMAT, Vector2> blkdims = new Dictionary<NUTEX_FORMAT, Vector2>()
        {
            { NUTEX_FORMAT.B8G8R8A8_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.B8G8R8A8_SRGB, new Vector2(1, 1) },
            { NUTEX_FORMAT.R8G8B8A8_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.R8G8B8A8_SRGB, new Vector2(1, 1) },
            { NUTEX_FORMAT.R32G32B32A32_FLOAT, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC1_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC1_SRGB, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC2_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC2_SRGB, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC3_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC3_SRGB, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC4_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC4_SNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC5_UNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC5_SNORM, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC6_UFLOAT, new Vector2(1, 1) },
            { NUTEX_FORMAT.BC7_SRGB, new Vector2(4, 4) },
            { NUTEX_FORMAT.BC7_UNORM, new Vector2(4, 4) },
        };

        public static uint bpps(NUTEX_FORMAT format)
        {
            switch (format)
            {
                case NUTEX_FORMAT.R8G8B8A8_UNORM:
                case NUTEX_FORMAT.R8G8B8A8_SRGB:
                    return 4;

                case NUTEX_FORMAT.BC1_UNORM:
                case NUTEX_FORMAT.BC1_SRGB:
                case NUTEX_FORMAT.BC4_UNORM:
                case NUTEX_FORMAT.BC4_SNORM:
                    return 8;

                case NUTEX_FORMAT.R32G32B32A32_FLOAT:
                case NUTEX_FORMAT.BC2_UNORM:
                case NUTEX_FORMAT.BC2_SRGB:
                case NUTEX_FORMAT.BC3_UNORM:
                case NUTEX_FORMAT.BC3_SRGB:
                case NUTEX_FORMAT.BC5_UNORM:
                case NUTEX_FORMAT.BC5_SNORM:
                case NUTEX_FORMAT.BC6_UFLOAT:
                case NUTEX_FORMAT.BC7_UNORM:
                case NUTEX_FORMAT.BC7_SRGB:
                    return 16;
                default: return 0x00;
            }
        }

        public IRenderable GetRenderableNode()
        {
            var texture = new RTexture();
            if (glFormatByNuTexFormat.ContainsKey(Format))
            {
                // TODO: This may require a higher OpenGL version for BC7.
                var sfTex = new SFGraphics.GLObjects.Textures.Texture2D();
                if(glFormatByNuTexFormat[Format] != InternalFormat.Rgba)
                    sfTex.LoadImageData(Width, Height, Mipmaps, glFormatByNuTexFormat[Format]);
                texture.texture = sfTex;
                sfTex.TextureWrapS = TextureWrapMode.Repeat;
                sfTex.TextureWrapT = TextureWrapMode.Repeat;
            }

            return texture;
        }

    }
}
