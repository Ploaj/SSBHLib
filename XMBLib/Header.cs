namespace XMBLib
{
    // XMB parsing code adapted from the following python script 
    // https://github.com/Sammi-Husky/SSBU-TOOLS/blob/master/XMBDec.py
    // MIT License Copyright (c) 2018 Sammi Husky
    // https://github.com/Sammi-Husky/SSBU-TOOLS/blob/master/LICENSE
    public class Header
    {
        public char[] Magic { get; set; }
        public int NumNodes { get; set; }
        public int NumValues { get; set; }
        public int NumProperties { get; set; }
        public int NumMappedNodes { get; set; }
        public uint StringsOffset { get; set; }
        public uint PNodesTable { get; set; }
        public uint PPropertiesTable { get; set; }
        public uint PNodeMap { get; set; }
        public uint PStrNames { get; set; }
        public uint PStrValues { get; set; }
        public uint Padding { get; set; }
    }
}
