namespace SSBHLib.Formats.Materials
{
    public enum MatlBlendFactor : int
    {
        Zero = 0,
        One = 1,
        SourceAlpha = 2,
        DestinationAlpha = 3,
        SourceColor = 4,
        DestinationColor = 5,
        OneMinusSourceAlpha = 6,
        OneMinusDestinationAlpha = 7,
        OneMinusSourceColor = 8,
        OneMinusDestinationColor = 9,
        SourceAlphaSaturate = 10
    }

    public partial class MatlAttribute
    {
        public class MatlBlendState : SsbhFile
        {
            public MatlBlendFactor SourceColor { get; set; }

            public int Unk2 { get; set; }

            public MatlBlendFactor DestinationColor { get; set; }

            public int Unk4 { get; set; }

            public int Unk5 { get; set; }

            public int Unk6 { get; set; }

            public int EnableAlphaSampleToCoverage { get; set; }

            public int Unk8 { get; set; }

            public int Unk9 { get; set; }

            public int Unk10 { get; set; }

            public ulong Padding { get; }

            public override string ToString()
            {
                return GetPropertyValues(GetType(), this);
            }
        }
    }
}