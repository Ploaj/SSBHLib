using System;

namespace SSBHLib.Formats.Materials
{
    [SsbhFile("LTAM")]
    public class Matl : SsbhFile
    {
        public uint Magic { get; set; } = 0x4D41544C;

        public short MajorVersion { get; set; } = 1;

        public short MinorVersion { get; set; } = 6;

        public MatlEntry[] Entries { get; set; }
    }
}