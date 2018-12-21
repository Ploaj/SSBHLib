namespace SSBHLib.Formats
{
    [SSBHFileAttribute("RDHS")]
    public class SHDR : ISSBH_File
    {
        public uint Magic { get; set; } 
        public ushort MajorVersion { get; set; }
        public ushort MinorVersion { get; set; }
        public ShdrShader[] Shaders { get; set; }
    }

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
