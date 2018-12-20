namespace SSBHLib.Formats.Materials
{
    public partial class MtalAttribute
    {
        public class MtalBlendState : ISSBH_File
        {
            public int Unk1 { get; set; }

            public int Unk2 { get; set; }

            public int Unk3 { get; set; }

            public int Unk4 { get; set; }

            public int Unk5 { get; set; }

            public int Unk6 { get; set; }

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