namespace SSBHLib.Formats.Rendering
{
    public class NrpdFrameBuffer : ISSBH_File
    {
        public string Name { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public ulong Unk1 { get; set; }

        public uint Unk2 { get; set; }

        public uint Unk3 { get; set; }
    }
}
