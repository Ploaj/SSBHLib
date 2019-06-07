namespace SSBHLib.Formats
{
    public class ShdrShader : ISSBH_File
    {
        public string Name { get; set; }

        public uint Unk1 { get; set; }

        public uint Unk2 { get; set; }

        public byte[] ShaderBinary { get; set; }

        public long ShaderFileSize { get { return ShaderBinary.Length; } set { } }

        public long Padding1 { get; set; }

        public long Padding2 { get; set; }
    }
}
