using System;

namespace SSBHLib.Formats.Animation
{
    [Flags]
    public enum ANIM_TRACKFLAGS
    {
        Transform = 0x1,
        Visibilty = 0x8,
        SingleTransform = 0x0200,
        HasTracks = 0x0400,
        SingleTrack = 0x0500,
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
