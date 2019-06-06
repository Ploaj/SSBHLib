namespace SSBHLib.Formats
{
    //TODO: incomplete documentation
    // Is this even worth looking into?
    [SSBHFileAttribute("DPRN")]
    public class NRPD : ISSBH_File
    {
        public uint Magic { get; set; } = 0x4E525044;

        public ushort MajorVersion { get; set; } = 1;

        public ushort MinorVersion { get; set; } = 6;

        public NrpdFrameBufferContainer[] FrameBufferContainers { get; set; }

        public NrpdStateContainer[] StateContainers { get; set; }

        public NrpdRenderPass[] RenderPasses { get; set; }

        // enum list or something

        // then finally some other structure
    }
}
