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

    public class NrpdFrameBufferContainer : ISSBH_File
    {
        public NrpdFrameBuffer FrameBuffer { get; set; }

        public ulong Type { get; set; }
    }

    public class NrpdFrameBuffer : ISSBH_File
    {
        public string Name { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public ulong Unk1 { get; set; }

        public uint Unk2 { get; set; }

        public uint Unk3 { get; set; }
    }


    public class NrpdStateContainer : ISSBH_File
    {
        public NrpdSampler StateObject { get; set; } // TODO: not always sampler

        public ulong Type { get; set; } // 0 - sampler 3 - state 2 - ZKeepAlways?
    }

    public class NrpdSampler : ISSBH_File
    {
        public string Name { get; set; }

        public int WrapS { get; set; }
        public int WrapT { get; set; }
        public int WrapR { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public int Unk6 { get; set; }
        public int Unk7 { get; set; }
        public int Unk8 { get; set; }
        public int Unk9 { get; set; }
        public int Unk10 { get; set; }
        public int Unk11 { get; set; }
        public int Unk12 { get; set; }
        public float Unk13 { get; set; }
        public int Unk14 { get; set; }
        public int Unk15 { get; set; }
        public int Unk16 { get; set; }
    }


    public class NrpdRenderPass : ISSBH_File
    {
        public string Name { get; set; }
        public ulong Offset2 { get; set; } // TODO: 
        public ulong Type2 { get; set; }
        public ulong Offset3 { get; set; }
        public ulong Type3 { get; set; }
        public string UnkString { get; set; }
        public ulong Type4 { get; set; }
        public ulong Padding { get; set; }
    }
}
