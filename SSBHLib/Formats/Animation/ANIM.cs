namespace SSBHLib.Formats.Animation
{
    [SsbhFile("MINA")]
    public class Anim : SsbhFile
    {
        public uint Magic { get; set; } = 0x414E494D;

        public ushort VersionMajor { get; set; } = 0x0002;

        public ushort VersionMinor { get; set; } = 0x0000;

        public float FrameCount { get; set; }

        public ushort Unk1 { get; set; } = 1;

        public ushort Unk2 { get; set; } = 3;

        public string Name { get; set; }

        public AnimGroup[] Animations { get; set; }

        public byte[] Buffer { get; set; }
    }
}
