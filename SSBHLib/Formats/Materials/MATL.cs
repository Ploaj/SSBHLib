using System;

namespace SSBHLib.Formats.Materials
{
    [SSBHFileAttribute("LTAM")]
    public class MATL : ISSBH_File
    {
        public uint Magic { get; set; }

        public short MajorVersion { get; set; }

        public short MinorVersion { get; set; }

        public MatlEntry[] Entries { get; set; }
    }
}