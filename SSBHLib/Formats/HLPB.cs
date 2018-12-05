using SSBHLib.IO;

namespace SSBHLib.Formats
{
    [SSBHFileAttribute("BPLH")]
    public class HLPB : ISSBH_File
    {
        public uint Magic { get; set; }

        public ushort VersionMajor { get; set; } // 0x0001

        public ushort VersionMinor { get; set; } // 0x0001

        public long UnkOffset { get; set; }

        public long UnkCount { get; set; }

        public HLPB_Entry[] Entries { get; set; }

        public int[] List1 { get; set; }

        public int[] List2 { get; set; }
    }

    public class HLPB_Entry : ISSBH_File
    {
        public string Name { get; set; }

        public string BoneName { get; set; }

        public string RootBoneName { get; set; }

        public string ParentBoneName { get; set; }

        public string DriverBoneName { get; set; }

        public uint Type { get; set; } // 1 and 2 not sure what for

        public float AoIX { get; set; }
        public float AoIY { get; set; }
        public float AoIZ { get; set; }

        public float Quat1X { get; set; }
        public float Quat1Y { get; set; }
        public float Quat1Z { get; set; }
        public float Quat1W { get; set; }

        public float Quat2X { get; set; }
        public float Quat2Y { get; set; }
        public float Quat2Z { get; set; }
        public float Quat2W { get; set; }

        public float MinRangeX { get; set; }
        public float MinRangeY { get; set; }
        public float MinRangeZ { get; set; }

        public float MaxRangeX { get; set; }
        public float MaxRangeY { get; set; }
        public float MaxRangeZ { get; set; }
    }
}
