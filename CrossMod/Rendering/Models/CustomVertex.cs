using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.VertexAttributes;
using SSBHLib.Tools;
using System.Runtime.InteropServices;

namespace CrossMod.Rendering.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct IVec4
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int W { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UintColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public UintColor(SsbhVertexAttribute vals)
        {
            R = (byte)vals.X;
            G = (byte)vals.Y;
            B = (byte)vals.Z;
            A = (byte)vals.W;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Packed4Colors
    {
        public UintColor Color1 { get; set; }
        public UintColor Color2 { get; set; }
        public UintColor Color3 { get; set; }
        public UintColor Color4 { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CustomVertex
    {
        [VertexFloat("Position0", ValueCount.Four, VertexAttribPointerType.Float, false, AttributeUsage.Position, false, false)]
        public SsbhVertexAttribute Position0 { get; }

        [VertexFloat("Normal0", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public SsbhVertexAttribute Normal0 { get; }

        [VertexFloat("Tangent0", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public SsbhVertexAttribute Tangent0 { get; }

        [VertexFloat("map1", ValueCount.Two, VertexAttribPointerType.Float, false)]
        public Vector2 Map1 { get; }

        [VertexFloat("uvSetUvSet1", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 UvSetUvSet1 { get; }

        [VertexFloat("uvSet2Bake1", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 UvSet2Bake1 { get; }

        [VertexInt("boneIndices", ValueCount.Four, VertexAttribIntegerType.Int)]
        public IVec4 BoneIndices { get; }

        [VertexFloat("boneWeights", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 BoneWeights { get; }

        [VertexFloat("colorSet1", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public SsbhVertexAttribute ColorSet1 { get; }

        [VertexInt("colorSet2", ValueCount.Four, VertexAttribIntegerType.UnsignedInt)]
        public Packed4Colors ColorSet2Packed { get; }

        [VertexInt("colorSet3456Packed", ValueCount.Four, VertexAttribIntegerType.UnsignedInt)]
        public Packed4Colors ColorSet3456Packed { get; }

        [VertexFloat("colorSet7", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public SsbhVertexAttribute ColorSet7 { get; }

        private static Packed4Colors GetPacked4ByteColor(SsbhVertexAttribute x, SsbhVertexAttribute y, SsbhVertexAttribute z, SsbhVertexAttribute w)
        {
            var color1 = new UintColor(x);
            var color2 = new UintColor(x);
            var color3 = new UintColor(x);
            var color4 = new UintColor(x);
            return new Packed4Colors { Color1 = color1, Color2 = color2, Color3 = color3, Color4 = color4 };
        }

        public CustomVertex(
            SsbhVertexAttribute position0, SsbhVertexAttribute normal0, SsbhVertexAttribute tangent0,
            SsbhVertexAttribute map1, SsbhVertexAttribute uvSet, SsbhVertexAttribute uvSet1, SsbhVertexAttribute uvSet2,
            IVec4 boneIndices, Vector4 boneWeights, SsbhVertexAttribute bake1,
            SsbhVertexAttribute colorSet1, SsbhVertexAttribute colorSet2, SsbhVertexAttribute colorSet21, SsbhVertexAttribute colorSet22, SsbhVertexAttribute colorSet23,
            SsbhVertexAttribute colorSet3, SsbhVertexAttribute colorSet4, SsbhVertexAttribute colorSet5, SsbhVertexAttribute colorSet6, SsbhVertexAttribute colorSet7)
        {
            Position0 = position0;
            Normal0 = normal0;
            Tangent0 = tangent0;
            Map1 = map1.ToVector2();

            // Intel/Nvidia have a max of 16 vertex attributes, so some attributes have to be packed together.
            UvSetUvSet1 = new Vector4(uvSet.X, uvSet.Y, uvSet1.X, uvSet1.Y);
            UvSet2Bake1 = new Vector4(uvSet2.X, uvSet2.Y, bake1.X, bake1.Y);

            BoneIndices = boneIndices;
            BoneWeights = boneWeights;

            // RGBA colors are 4 bytes, so 4 colors fit in a vec4 attribute.
            // TODO: These should be read from file as uints directly to avoid the byte -> float -> byte conversion.
            ColorSet1 = colorSet1;
            ColorSet2Packed = GetPacked4ByteColor(colorSet2, colorSet21, colorSet22, colorSet23);
            ColorSet3456Packed = GetPacked4ByteColor(colorSet3, colorSet4, colorSet5, colorSet6);
            ColorSet7 = colorSet7;
        }
    }
}
