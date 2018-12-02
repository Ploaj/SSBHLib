using CrossMod.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;
using CrossMod.Tools;

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
            { NUTEX_FORMAT.BC2_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC2_SRGB, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC3_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC3_SRGB, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC4_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC4_SNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC5_UNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC5_SNORM, InternalFormat.Rgba },
            { NUTEX_FORMAT.BC6_UFLOAT, InternalFormat.Rgba },
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
                for(int i = 0; i < 64; i++)
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
                R.ReadInt16();
                int MipCount = R.ReadInt32();
                int Alignment = R.ReadInt32();
                int Array = R.ReadInt32();
                int ImageSize = R.ReadInt32();
                char[] Magic = R.ReadChars(4);
                int MajorVersion = R.ReadInt16();
                int MinorVersion = R.ReadInt16();

                R.BaseStream.Position = 0;
                for (int i = 0; i < MipCount; i++)
                {
                    Mipmaps.Add(SwitchSwizzler.Deswizzle(Width >> i, Height >> i, 0x1E, R.ReadBytes(MipSizes[i])));
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            var texture = new RTexture();
            if (glFormatByNuTexFormat.ContainsKey(Format))
            {
                // TODO: This may require a higher OpenGL version for BC7.
                var sfTex = new SFGraphics.GLObjects.Textures.Texture2D();
                sfTex.LoadImageData(Width, Height, Mipmaps, glFormatByNuTexFormat[Format]);
                texture.texture = sfTex;
            }

            return texture;
        }

    }
}
