namespace SSBHLib.Formats.Materials
{
    public partial class MatlAttribute
    {
        public class MatlSampler : SsbhFile
        {
            public int WrapS { get; set; }
            public int WrapT { get; set; }
            public int WrapR { get; set; }
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