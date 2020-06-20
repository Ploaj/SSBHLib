using System.IO;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".shpcanim")]
    public class ShanNode : FileNode
    {

        public ShanNode(string path) : base(path)
        {
            ImageKey = "animation";
            SelectedImageKey = "animation";
        }

        public override void Open()
        {
            using (BinaryReader r = new BinaryReader(new FileStream(AbsolutePath, FileMode.Open)))
            {
                char[] magic = r.ReadChars(4);
                uint defaultId = r.ReadUInt32();
                uint tpcbCount = r.ReadUInt32();

                // TODO: anything else in this header?

                r.BaseStream.Position = 0x80;
                uint[] tpcbIDs = new uint[tpcbCount];
                uint[] tpcbOffsets = new uint[tpcbCount];

                for (int i = 0; i < tpcbCount; i++)
                    tpcbIDs[i] = r.ReadUInt32();
                for (int i = 0; i < tpcbCount; i++)
                    tpcbOffsets[i] = r.ReadUInt32();

                for (int i = 0; i < tpcbCount; i++)
                {
                    r.BaseStream.Position = tpcbOffsets[i];
                    TpcbNode node = new TpcbNode();

                    node.Magic = r.ReadChars(4);
                    if (!new string(node.Magic).Equals("TPCB")) continue;

                    uint sectionOffset1 = (uint)r.BaseStream.Position + r.ReadUInt32();
                    uint sectionOffset2 = (uint)r.BaseStream.Position + r.ReadUInt32();
                    uint sectionOffset3 = (uint)r.BaseStream.Position + r.ReadUInt32();
                    ushort unk1 = r.ReadUInt16();
                    ushort unk2 = r.ReadUInt16();
                    ushort unk3 = r.ReadUInt16();
                    ushort unk4 = r.ReadUInt16();
                    uint unk5 = r.ReadUInt32();
                    uint unk6 = r.ReadUInt32();
                    float[] floats = new float[12];
                    for (int j = 0; j < floats.Length; j++)
                        floats[j] = r.ReadSingle();
                    uint stride = r.ReadUInt32(); //?
                    float float1 = r.ReadSingle();
                    float float2 = r.ReadSingle();
                    uint entryCount = r.ReadUInt32();
                    
                    // TODO: look into in the future
                }

            }
        }
    }

    public class TpcbNode
    {
        public char[] Magic;

    }
}
