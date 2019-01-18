using SSBHLib.Formats.Meshes;
using System.Collections.Generic;
using System.IO;

namespace SSBHLib.Tools
{
    public class SSBHRiggingAccessor
    {
        private MESH MeshFile;

        public SSBHRiggingAccessor(MESH meshFile)
        {
            MeshFile = meshFile;
        }

        public SSBHVertexInfluence[] ReadRiggingBuffer(string meshName, int subIndex)
        {
            MeshRiggingGroup riggingGroup = FindRiggingGroup(meshName, subIndex);

            if (riggingGroup == null)
                return new SSBHVertexInfluence[0];

            List<SSBHVertexInfluence> Influences = new List<SSBHVertexInfluence>();

            foreach (MeshBoneBuffer boneBuffer in riggingGroup.Buffers)
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

        private MeshRiggingGroup FindRiggingGroup(string meshName, int subIndex)
        {
            MeshRiggingGroup riggingGroup = null;

            foreach (MeshRiggingGroup g in MeshFile.RiggingBuffers)
            {
                if (g.Name.Equals(meshName) && g.SubMeshIndex == subIndex)
                {
                    riggingGroup = g;
                    break;
                }
            }

            return riggingGroup;
        }
    }
}
