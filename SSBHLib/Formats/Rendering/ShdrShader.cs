namespace SSBHLib.Formats.Rendering
{
    public class ShdrShader : SsbhFile
    {
        public string Name { get; set; }

        public uint Unk1 { get; set; }

        public uint Unk2 { get; set; }

        public byte[] ShaderBinary { get; set; }

        // TODO: Why does this property have a setter?
        public long ShaderFileSize { get { return ShaderBinary.Length; } set { } }

        public long Padding1 { get; set; }

        public long Padding2 { get; set; }
    }
}
