namespace SSBHLib.Formats.Materials
{
    public enum MatlWrapMode : int
    {
        Repeat = 0,
        ClampToEdge = 1,
        MirroredRepeat = 2,
        ClampToBorder = 3
    }

    public partial class MatlAttribute
    {
        public class MatlSampler : SsbhFile
        {
            public MatlWrapMode WrapS { get; set; }
            public MatlWrapMode WrapT { get; set; }
            public MatlWrapMode WrapR { get; set; }
            public int MinFilter { get; set; }
            public int MagFilter { get; set; }
            public int Unk6 { get; set; }
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