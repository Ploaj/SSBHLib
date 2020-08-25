using System.Runtime.InteropServices;

namespace XMBLib
{
    // XMB parsing code adapted from the following python script 
    // MIT License Copyright (c) 2018 Sammi Husky
    // https://github.com/Sammi-Husky/SSBU-TOOLS/blob/master/XMBDec.py
    // https://github.com/Sammi-Husky/SSBU-TOOLS/blob/master/LICENSE
    [StructLayout(LayoutKind.Sequential)]
    public struct Header
    {
        public uint Magic;
        public uint NodeCount;
        public uint ValueCount;
        public uint PropertyCount;
        public uint MappedNodesCount;
        public uint StringsOffset;
        public uint NodesTableOffset;
        public uint PropertiesTableOffset;
        public uint NodeMapOffset;
        public uint NamesOffset;
        public uint ValuesOffset;
        public uint Padding;
    }
}
