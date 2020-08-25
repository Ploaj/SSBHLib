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
        private struct PropertyData
        {
            public uint NameOffset { get; set; }
            public uint ValueOffset { get; set; }
        }

        public Header Header { get; set; }
        public XmbEntry Root { get; set; }
        public List<XmbEntry> Nodes { get; } = new List<XmbEntry>();
        public Dictionary<string, int> NodeDict { get; } = new Dictionary<string, int>();

        public Xmb(string path)
        {
            InitializeFromFile(path);
        }

        private void InitializeFromFile(string path)
        {
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                Header = reader.ReadStruct<Header>();

                reader.BaseStream.Position = Header.NodesTableOffset;
                var entryData = reader.ReadStructs<EntryData>((int)Header.NodeCount);
                CreateEntries(reader, entryData);

                CreateMappedNodes(reader);

                CreateParentChildRelationships();
            }
        }

        private void CreateMappedNodes(BinaryReader reader)
        {
            reader.BaseStream.Position = Header.NodeMapOffset;
            for (int mappedNodeIndex = 0; mappedNodeIndex < Header.MappedNodesCount; mappedNodeIndex++)
            {
                var idOffset = reader.ReadUInt32();
                var nodeIndex = reader.ReadInt32();
                var nodeId = reader.ReadAscii(Header.ValuesOffset + idOffset);
                NodeDict[nodeId] = nodeIndex;
            }
        }

        private void CreateParentChildRelationships()
        {
            foreach (var entry in Nodes)
            {
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

        private void CreateEntries(BinaryReader reader, EntryData[] entryData)
        {
            for (int entryIndex = 0; entryIndex < Header.NodeCount; entryIndex++)
            {
                var entry = new XmbEntry
                {
                    Data = entryData[entryIndex],
                    Name = reader.ReadAscii(Header.NamesOffset + entryData[entryIndex].NameOffset),
                    Index = (sbyte)entryIndex
                };

                reader.BaseStream.Position = Header.PropertiesTableOffset + entry.Data.PropertyStartIndex * Marshal.SizeOf(typeof(PropertyData));
                var propertyData = reader.ReadStructs<PropertyData>(entry.Data.PropertyCount);

                CreateProperties(reader, entry, propertyData);

                Nodes.Add(entry);
            }
        }

        private void CreateProperties(BinaryReader reader, XmbEntry entry, PropertyData[] propertyData)
        {
            foreach (var property in propertyData)
            {
                var propertyName = reader.ReadAscii(Header.NamesOffset + property.NameOffset);
                var propertyValue = reader.ReadAscii(Header.ValuesOffset + property.ValueOffset);
                entry.Properties[propertyName] = propertyValue;
            }
        }
    }
}
