namespace SSBHLib.Formats
{
    [SsbhFile("BPLH")]
    public class Hlpb : SsbhFile
    {
        public uint Magic { get; set; }

        public ushort VersionMajor { get; set; } // 0x0001

        public ushort VersionMinor { get; set; } // 0x0001

        public HlpbRotateAim[] AimEntries { get; set; }

        public HlpbRotateInterpolation[] InterpolationEntries { get; set; }

        public int[] List1 { get; set; }

        public int[] List2 { get; set; }
    }

    public class HlpbRotateAim : SsbhFile
    {
        public string Name { get; set; }

        public string AimBoneName1 { get; set; }

        public string AimBoneName2 { get; set; }

        public string AimType1 { get; set; }

        public string AimType2 { get; set; }

        public string TargetBoneName1 { get; set; }

        public string TargetBoneName2 { get; set; }

        public int Unknown1 { get; set; }

        public int Unknown2 { get; set; }

        public float Unknown3 { get; set; }

        public float Unknown4 { get; set; }

        public float Unknown5 { get; set; }

        public float Unknown6 { get; set; }

        public float Unknown7 { get; set; }

        public float Unknown8 { get; set; }

        public float Unknown9 { get; set; }

        public float Unknown10 { get; set; }

        public float Unknown11 { get; set; }

        public float Unknown12 { get; set; }

        public float Unknown13 { get; set; }

        public float Unknown14 { get; set; }

        public float Unknown15 { get; set; }

        public float Unknown16 { get; set; }

        public float Unknown17 { get; set; }

        public float Unknown18 { get; set; }

        public float Unknown19 { get; set; }

        public float Unknown20 { get; set; }

        public float Unknown21 { get; set; }

        public float Unknown22 { get; set; }
    }

    public class HlpbRotateInterpolation : SsbhFile
    {
        public string Name { get; set; }

        public string BoneName { get; set; }

        public string RootBoneName { get; set; }

        public string ParentBoneName { get; set; }

        public string DriverBoneName { get; set; }

        public uint Type { get; set; } // 1 and 2 not sure what for

        public float AoIx { get; set; }
        public float AoIy { get; set; }
        public float AoIz { get; set; }

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
