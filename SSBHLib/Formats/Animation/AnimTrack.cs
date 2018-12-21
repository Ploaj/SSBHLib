using System;

namespace SSBHLib.Formats.Animation
{
    public enum ANIM_TRACKFLAGS
    {
        Transform = 0x0001,
        Boolean = 0x0008,
        Vector4 = 0x0009,
        ConstTransform = 0x0200,
        Animated = 0x0400,
        Constant = 0x0500,
    }

    public class AnimTrack : ISSBH_File
    {
        public string Name { get; set; }

        public uint Flags { get; set; }

        public uint FrameCount { get; set; }

        public uint Unk3_0 { get; set; }

        public uint DataOffset { get; set; }

        public long DataSize { get; set; }
    }
}
