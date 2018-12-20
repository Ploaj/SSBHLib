namespace SSBHLib.Formats.Animation
{
    [SSBHFileAttribute("MINA")]
    public class ANIM : ISSBH_File
    {
        public uint Magic { get; set; }

        public ushort VersionMajor { get; set; } // 0x0002
        
        public ushort VersionMinor { get; set; } // 0x0001
        
        public float FrameCount { get; set; }
        
        public ushort Unk1 { get; set; }
        
        public ushort Unk2 { get; set; }
        
        public string Name { get; set; }

        public ANIM_Group[] Animations { get; set; }

        public byte[] Buffer { get; set; }
    }
}
