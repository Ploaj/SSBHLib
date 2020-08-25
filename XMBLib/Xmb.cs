using SSBHLib.IO;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace XMBLib
{
    // XMB parsing code adapted from the following python script 
    // MIT License Copyright (c) 2018 Sammi Husky
    // https://github.com/Sammi-Husky/SSBU-TOOLS/blob/master/XMBDec.py
    // https://github.com/Sammi-Husky/SSBU-TOOLS/blob/master/LICENSE
    public class Xmb
    {
        public Header Header { get; set; }
        public XmbEntry Root { get; set; }
        public List<XmbEntry> Nodes { get; } = new List<XmbEntry>();
        public Dictionary<string, int> NodeDict { get; } = new Dictionary<string, int>();

        public Xmb(string path)
        {
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                Header = reader.ReadStruct<Header>();

                for (int x = 0; x < Header.NodeCount; x++)
                {
                    reader.BaseStream.Position = Header.NodesTableOffset + x * Marshal.SizeOf(typeof(EntryData));
                    var entry = new XmbEntry
                    {
                        Data = reader.ReadStruct<EntryData>()
                    };

                    entry.Name = reader.ReadAscii(Header.NamesOffset + entry.Data.NameOffset);
                    entry.Index = (sbyte)x;

                    for (int y = 0; y < entry.Data.PropertyCount; y++)
                    {
                        // TODO: This is an array of structs.
                        reader.BaseStream.Position = Header.PropertiesTableOffset + (entry.Data.FirstProp + y) * sizeof(int) * 2;
                        var strOff1 = reader.ReadUInt32();
                        var strOff2 = reader.ReadUInt32();
                        var prop = reader.ReadAscii(Header.NamesOffset + strOff1);
                        entry.Properties[prop] = reader.ReadAscii(Header.ValuesOffset + strOff2);
                    }

                    Nodes.Add(entry);
                }

                for (int x = 0; x < Header.MappedNodesCount; x++)
                {
                    // TODO: This is an array of structs.
                    reader.BaseStream.Position = Header.NodeMapOffset + x * sizeof(int) * 2;
                    var strOff1 = reader.ReadUInt32();
                    var nodeIndex = reader.ReadInt32();
                    var nodeId = reader.ReadAscii(Header.ValuesOffset + strOff1);
                    NodeDict[nodeId] = nodeIndex;
                }

                for (int x = 0; x < Nodes.Count; x++)
                {
                    var entry = Nodes[x];
                    if (entry.Data.ParentIndex != -1)
                    {
                        entry.Parent = Nodes[entry.Data.ParentIndex];
                        Nodes[entry.Data.ParentIndex].Children.Add(entry);
                    }
                    else
                    {
                        Root = entry;
                    }
                }
            }
        }
    }
}
