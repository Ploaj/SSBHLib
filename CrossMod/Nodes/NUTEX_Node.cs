using CrossMod.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;

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
                R.ReadInt32(); // padding?
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
                foreach(int size in MipSizes)
                {
                    Mipmaps.Add(R.ReadBytes(size));
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            RTexture Texture = new RTexture();

            return Texture;
        }

    }
}
