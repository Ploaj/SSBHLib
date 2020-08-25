using System.Collections.Generic;

namespace XMBLib
{
    // XMB parsing code adapted from the following python script 
    // MIT License Copyright (c) 2018 Sammi Husky
    // https://github.com/Sammi-Husky/SSBU-TOOLS/blob/master/XMBDec.py
    // https://github.com/Sammi-Husky/SSBU-TOOLS/blob/master/LICENSE
    public class XmbEntry
    {
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public List<XmbEntry> Children { get; set; } = new List<XmbEntry>();
        public XmbEntry Parent { get; set; }
        public string Name { get; set; }
        public short Index { get; set; }

        public EntryData Data { get; set; }
    }
}
