namespace SSBHLib.Formats.Rendering
{
    [SsbhFile("RDHS")]
    public class Shdr : SsbhFile
    {
        public uint Magic { get; set; }

        public ushort MajorVersion { get; set; }

        public ushort MinorVersion { get; set; }

        public ShdrShader[] Shaders { get; set; }
    }
}
