using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSBHLib.Formats;
using System.IO;

namespace SSBHLib.Tools
{
    public struct SSBHVertexInfluence
    {
        public string BoneName;
        public ushort VertexIndex;
        public float Weight;
    }

    public class SSBHRiggingAccessor
    {
        private MESH MeshFile;

        public SSBHRiggingAccessor(MESH MeshFile)
        {
            this.MeshFile = MeshFile;
        }

        public SSBHVertexInfluence[] ReadRiggingBuffer(string MeshName, int SubIndex)
        {
            MESH_RiggingGroup riggingGroup = null;

            foreach(MESH_RiggingGroup g in MeshFile.RiggingBuffers)
            {
                if(g.Name.Equals(MeshName) && g.SubMeshIndex == SubIndex)
                {
                    riggingGroup = g;
                    break;
                }
            }

            if (riggingGroup == null) return new SSBHVertexInfluence[0];

            List<SSBHVertexInfluence> Influences = new List<SSBHVertexInfluence>();

            foreach (MESH_BoneBuffer boneBuffer in riggingGroup.Buffers)
            {
                using (BinaryReader R = new BinaryReader(new MemoryStream(boneBuffer.Data)))
                {
                    for (int i = 0; i < boneBuffer.Data.Length / 6; i++)
                    {
                        Influences.Add(new SSBHVertexInfluence()
                        {
                            BoneName = boneBuffer.BoneName,
                            VertexIndex = R.ReadUInt16(),
                            Weight = R.ReadSingle()
                        });
                    }
                }
            }

            return Influences.ToArray();
        }

    }
}
