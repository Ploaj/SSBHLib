using System;

namespace SSBHLib.Formats.Materials
{
    [SSBHFileAttribute("LTAM")]
    public class MTAL : ISSBH_File
    {
        public uint Magic { get; set; }

        public short MajorVersion { get; set; }

        public short MinorVersion { get; set; }

        public MtalEntry[] Entries { get; set; }
    }
}