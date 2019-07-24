namespace SSBHLib.Formats.Materials
{
    public partial class MatlAttribute
    {
        public class MatlBlendState : SsbhFile
        {
            public int Unk1 { get; set; }

            public int Unk2 { get; set; }

            public int BlendFactor1 { get; set; }

            public int Unk4 { get; set; }

            public int Unk5 { get; set; }

            public int BlendFactor2 { get; set; }

            public int Unk7 { get; set; }

            public int Unk8 { get; set; }

            public int Unk9 { get; set; }

            public int Unk10 { get; set; }

            public int Unk11 { get; set; }

            public int Unk12 { get; set; }

            public override string ToString()
            {
                return GetPropertyValues(GetType(), this);
            }
        }
    }
}