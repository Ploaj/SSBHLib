using System;
using System.Collections.Generic;
using SSBHLib.Formats.Meshes;
using System.Linq;

namespace SSBHLib.Tools
{
    public class SSBHRiggingCompiler
    {

        public static MeshRiggingGroup CreateRiggingGroup(string MeshName, int MeshIndex, SSBHVertexInfluence[] influences)
        {
            var group = new MeshRiggingGroup();
            group.Name = MeshName;
            group.SubMeshIndex = MeshIndex;

            Dictionary<string, List<byte>> boneNameToData = new Dictionary<string, List<byte>>();
            Dictionary<ushort, int> vertexToWeightCount = new Dictionary<ushort, int>();
            int MaxInfluenceCount = 0;

            foreach(var influence in influences)
            {
                // get byte list
                if (!boneNameToData.ContainsKey(influence.BoneName))
                {
                    boneNameToData.Add(influence.BoneName, new List<byte>());
                }

                var bytes = boneNameToData[influence.BoneName];
                bytes.AddRange(BitConverter.GetBytes(influence.VertexIndex));
                bytes.AddRange(BitConverter.GetBytes(influence.Weight));

                if (!vertexToWeightCount.ContainsKey(influence.VertexIndex))
                {
                    vertexToWeightCount.Add(influence.VertexIndex, 0);
                }
                vertexToWeightCount[influence.VertexIndex]++;
                MaxInfluenceCount = Math.Max(MaxInfluenceCount, vertexToWeightCount[influence.VertexIndex]);
            }

            // create bone groups
            group.Flags = 0x0100 | MaxInfluenceCount;
            List<MeshBoneBuffer> bonebuffers = new List<MeshBoneBuffer>();
            foreach(var pair in boneNameToData)
            {
                bonebuffers.Add(new MeshBoneBuffer() { BoneName = pair.Key, Data = pair.Value.ToArray() });
            }
            group.Buffers = bonebuffers.ToArray().OrderBy(o => o.BoneName, StringComparer.Ordinal).ToArray();


            return group;
        }

    }
}
