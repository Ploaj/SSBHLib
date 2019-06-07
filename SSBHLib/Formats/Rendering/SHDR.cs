namespace SSBHLib.Formats.Rendering
{
    [SSBHFileAttribute("RDHS")]
    public class SHDR : ISSBH_File
    {
        public uint Magic { get; set; } 

        public ushort MajorVersion { get; set; }

        public ushort MinorVersion { get; set; }

        public ShdrShader[] Shaders { get; set; }
    }
}
