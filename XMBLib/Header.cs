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
        public uint Magic { get; set; }
        public uint NodeCount{ get; set; }
        public uint ValueCount{ get; set; }
        public uint PropertyCount{ get; set; }
        public uint MappedNodesCount{ get; set; }
        public uint StringsOffset{ get; set; }
        public uint NodesTableOffset{ get; set; }
        public uint PropertiesTableOffset{ get; set; }
        public uint NodeMapOffset{ get; set; }
        public uint NamesOffset{ get; set; }
        public uint ValuesOffset{ get; set; }
        public uint Padding{ get; set; }
    }
}
