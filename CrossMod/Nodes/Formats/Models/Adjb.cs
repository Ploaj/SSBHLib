using System.Collections.Generic;
using System.IO;

namespace CrossMod.Nodes
{
    // extra node not specified in the SSBH
    // seems to be triangle adjacency for certain meshes
    public class Adjb
    {
        public Dictionary<int, uint[]> MeshToIndexBuffer = new Dictionary<int, uint[]>();

        public void Read(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                int count = reader.ReadInt32();

                int prevId = -1;
                int prevOffset = -1;
                for (int i = 0; i < count; i++)
                {
                    int id = reader.ReadInt32();
                    int offset = reader.ReadInt32();

                    if (prevOffset != -1)
                    {
                        long temp = reader.BaseStream.Position;
                        reader.BaseStream.Position = 4 + 8 * count + prevOffset;
                        uint[] buffer = new uint[(offset - prevOffset) / 2];
                        for (int j = 0; j < buffer.Length; j++)
                            buffer[j] = (uint)reader.ReadInt16();
                        reader.BaseStream.Position = temp;
                        MeshToIndexBuffer.Add(prevId, buffer);
                    }

                    prevOffset = offset;
                    prevId = id;
                }

                if (prevOffset != -1)
                {
                    long temp = reader.BaseStream.Position;
                    reader.BaseStream.Position = 4 + 8 * count + prevOffset;
                    uint[] buffer = new uint[((int)(reader.BaseStream.Length - 4 - count * 8) - prevOffset) / 2];
                    for (int j = 0; j < buffer.Length; j++)
                        buffer[j] = (uint)reader.ReadInt16();
                    reader.BaseStream.Position = temp;
                    MeshToIndexBuffer.Add(prevId, buffer);
                }
            }
        }
    }
}
