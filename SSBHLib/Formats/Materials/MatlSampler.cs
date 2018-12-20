namespace SSBHLib.Formats.Materials
{
    public partial class MatlAttribute
    {
        public class MtalSampler : ISSBH_File
        {
            public int WrapS { get; set; }
            public int WrapT { get; set; }
            public int WrapR { get; set; }
            public int Unk4 { get; set; }
            public int Unk5 { get; set; }
            public int Unk6 { get; set; }
            public int Unk7 { get; set; }
            public int Unk8 { get; set; }
            public int Unk9 { get; set; }
            public int Unk10 { get; set; }
            public int Unk11 { get; set; }
            public int Unk12 { get; set; }
            public float Unk13 { get; set; }
            public int Unk14 { get; set; }
            public int Unk15 { get; set; }

            public override string ToString()
            {
                return GetPropertyValues(GetType(), this);
            }
        }
    }
}