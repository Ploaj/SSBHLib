using System;

namespace SSBHLib.Formats.Materials
{
    [SSBHFileAttribute("LTAM")]
    public class MATL : ISSBH_File
    {
        public uint Magic { get; set; } = 0x4D41544C;

        public short MajorVersion { get; set; } = 1;

        public short MinorVersion { get; set; } = 6;

        public MatlEntry[] Entries { get; set; }
    }
}