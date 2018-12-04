using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSBHLib.Formats
{
    [Flags]
    public enum ANIM_TRACKFLAGS
    {
        None = 0,
        Has_Scale = 0x2,
        Has_Rotation = 0x4,
        Has_Position = 0x8
    }

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

    public class ANIM_Group : ISSBH_File
    {
        public long Type { get; set; }

        public ANIM_Node[] Nodes { get; set; }
    }

    public class ANIM_Node : ISSBH_File
    {
        public string Name { get; set; }

        public ANIM_Track[] Tracks { get; set; }
    }

    public class ANIM_Track : ISSBH_File
    {
        public string Name { get; set; }
        
        public ANIM_TRACKFLAGS Flags { get; set; }
        
        public uint FrameCount { get; set; }
        
        public uint Unk3_0 { get; set; }
        
        public uint DataOffset { get; set; }
        
        public long DataSize { get; set; }
    }
}
