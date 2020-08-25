using System;
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

        // TODO: Move this somewhere.
        // TODO: This code can be shared with SSBHLib
        // TODO: Have a second option for inline strings?
        public static string ReadAscii(BinaryReader reader)
        {
            var stringValue = new System.Text.StringBuilder();

            byte b = reader.ReadByte();
            while (b != 0)
            {
                stringValue.Append((char)b);
                b = reader.ReadByte();
            }

            return stringValue.ToString();
        }

        public Xmb(string path)
        {
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                Header = new Header
                {
                    // TODO: This could be a struct.
                    Magic = reader.ReadChars(4),
                    NumNodes = reader.ReadInt32(),
                    NumValues = reader.ReadInt32(),
                    NumProperties = reader.ReadInt32(),
                    NumMappedNodes = reader.ReadInt32(),
                    StringsOffset = reader.ReadUInt32(),
                    PNodesTable = reader.ReadUInt32(),
                    PPropertiesTable = reader.ReadUInt32(),
                    PNodeMap = reader.ReadUInt32(),
                    PStrNames = reader.ReadUInt32(),
                    PStrValues = reader.ReadUInt32(),
                    Padding = reader.ReadUInt32()
                };

                for (int x = 0; x < Header.NumNodes; x++)
                {
                    // TODO: What is this stride?
                    reader.BaseStream.Position = Header.PNodesTable + x * 0x10;
                    var entry = new XmbEntry();

                    // TODO: This could be a struct.
                    // 0x10 is the size in bytes.
                    entry.NameOffset = reader.ReadUInt32();
                    entry.NumProps = reader.ReadUInt16();
                    entry.NumChildren = reader.ReadUInt16();
                    entry.FirstProp = reader.ReadUInt16();
                    entry.Unk1 = reader.ReadUInt16();
                    entry.ParentIndex = reader.ReadInt16();
                    entry.Unk2 = reader.ReadUInt16();

                    reader.BaseStream.Position = Header.PStrNames + entry.NameOffset;
                    entry.Name = ReadAscii(reader);
                    entry.Index = (sbyte)x;

                    for (int y = 0; y < entry.NumProps; y++)
                    {
                        // TODO: What is this stride?
                        reader.BaseStream.Position = Header.PPropertiesTable + (entry.FirstProp + y) * 8;
                        var strOff1 = reader.ReadUInt32();
                        var strOff2 = reader.ReadUInt32();
                        reader.BaseStream.Position = Header.PStrNames + strOff1;
                        var prop = ReadAscii(reader);
                        reader.BaseStream.Position = Header.PStrValues + strOff2;
                        entry.Properties[prop] = ReadAscii(reader);
                    }

                    Nodes.Add(entry);
                }

                for (int x = 0; x < Header.NumMappedNodes; x++)
                {
                    // TODO: What is this stride?
                    reader.BaseStream.Position = Header.PNodeMap + x * 8;
                    var strOff1 = reader.ReadInt32();
                    var nodeIndex = reader.ReadInt32();
                    reader.BaseStream.Position = Header.PStrValues + strOff1;
                    var nodeId = ReadAscii(reader);
                    NodeDict[nodeId] = nodeIndex;
                }

                for (int x = 0; x < Nodes.Count; x++)
                {
                    var entry = Nodes[x];
                    if (entry.ParentIndex != -1)
                    {
                        entry.Parent = Nodes[entry.ParentIndex];
                        Nodes[entry.ParentIndex].Children.Add(entry);
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
