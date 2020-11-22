namespace SSBHLib.Formats.Materials
{
    public enum MatlWrapMode : int
    {
        Repeat = 0,
        ClampToEdge = 1,
        MirroredRepeat = 2,
        ClampToBorder = 3
    }

    public enum MatlMinFilter : int
    {
        Nearest = 0,
        LinearMipmapLinear = 1,
        LinearMipmapLinear2 = 2,
    }

    public enum MatlMagFilter : int
    {
        Nearest = 0,
        Linear = 1,
        Linear2 = 2,
    }

    public enum FilteringType : int
    {
        Default = 0,
        Default2 = 1,
        AnisotropicFiltering = 2
    }

    public partial class MatlAttribute
    {
        public class MatlSampler : SsbhFile
        {
            public MatlWrapMode WrapS { get; set; }
            public MatlWrapMode WrapT { get; set; }
            public MatlWrapMode WrapR { get; set; }
            public MatlMinFilter MinFilter { get; set; }
            public MatlMagFilter MagFilter { get; set; }
            public FilteringType TextureFilteringType { get; set; }
            public int Unk7 { get; set; }
            public int Unk8 { get; set; }
            public int Unk9 { get; set; }
            public int Unk10 { get; set; }
            public int Unk11 { get; set; }
            public int Unk12 { get; set; }
            public float LodBias { get; set; }
            public int MaxAnisotropy { get; set; }

            public override string ToString()
            {
                return GetPropertyValues(GetType(), this);
            }
        }
    }
}