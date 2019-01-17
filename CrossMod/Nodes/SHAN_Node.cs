using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".shpcanim")]
    public class SHAN_Node : FileNode
    {

        public SHAN_Node(string path) : base(path)
        {
            ImageKey = "animation";
            SelectedImageKey = "animation";
        }

        public override void Open()
        {
            using (BinaryReader R = new BinaryReader(new FileStream(AbsolutePath, FileMode.Open)))
            {
                char[] Magic = R.ReadChars(4);
                uint DefaultID = R.ReadUInt32();
                uint TPCBCount = R.ReadUInt32();

                // TODO: anything else in this header?

                R.BaseStream.Position = 0x80;
                uint[] TPCB_IDs = new uint[TPCBCount];
                uint[] TPCB_Offsets = new uint[TPCBCount];

                for (int i = 0; i < TPCBCount; i++)
                    TPCB_IDs[i] = R.ReadUInt32();
                for (int i = 0; i < TPCBCount; i++)
                    TPCB_Offsets[i] = R.ReadUInt32();

                for (int i = 0; i < TPCBCount; i++)
                {
                    R.BaseStream.Position = TPCB_Offsets[i];
                    TPCB_Node Node = new TPCB_Node();

                    Node.Magic = R.ReadChars(4);
                    if (!new string(Node.Magic).Equals("TPCB")) continue;

                    uint SectionOffset1 = (uint)R.BaseStream.Position + R.ReadUInt32();
                    uint SectionOffset2 = (uint)R.BaseStream.Position + R.ReadUInt32();
                    uint SectionOffset3 = (uint)R.BaseStream.Position + R.ReadUInt32();
                    ushort Unk1 = R.ReadUInt16();
                    ushort Unk2 = R.ReadUInt16();
                    ushort Unk3 = R.ReadUInt16();
                    ushort Unk4 = R.ReadUInt16();
                    uint Unk5 = R.ReadUInt32();
                    uint Unk6 = R.ReadUInt32();
                    float[] floats = new float[12];
                    for (int j = 0; j < 12; j++)
                        floats[j] = R.ReadSingle();
                    uint Stride = R.ReadUInt32(); //?
                    float Float1 = R.ReadSingle();
                    float Float2 = R.ReadSingle();
                    uint EntryCount = R.ReadUInt32();
                    
                    // look into in the future
                }

            }
        }
    }

    public class TPCB_Node
    {
        public char[] Magic;

    }
}
