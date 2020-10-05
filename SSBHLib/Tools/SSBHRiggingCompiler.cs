using SSBHLib.Formats.Meshes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSBHLib.Tools
{
    /// <summary>
    /// Tools for compiling rigging information into a Mesh Rigging Group
    /// </summary>
    public class SsbhRiggingCompiler
    {
        /// <summary>
        /// Creates a mesh rigging group
        /// </summary>
        /// <param name="meshName"></param>
        /// <param name="meshIndex"></param>
        /// <param name="influences"></param>
        /// <returns></returns>
        public static MeshRiggingGroup CreateRiggingGroup(string meshName, int meshIndex, SsbhVertexInfluence[] influences)
        {
            var group = new MeshRiggingGroup
            {
                MeshName = meshName,
                MeshSubIndex = meshIndex
            };

            Dictionary<string, List<byte>> boneNameToData = new Dictionary<string, List<byte>>();
            Dictionary<ushort, int> vertexToWeightCount = new Dictionary<ushort, int>();
            int maxInfluenceCount = 0;

            foreach (var influence in influences)
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
                maxInfluenceCount = Math.Max(maxInfluenceCount, vertexToWeightCount[influence.VertexIndex]);
            }

            // create bone groups
            group.Flags = 0x0100 | maxInfluenceCount;
            List<MeshBoneBuffer> bonebuffers = new List<MeshBoneBuffer>();
            foreach (var pair in boneNameToData)
            {
                bonebuffers.Add(new MeshBoneBuffer() { BoneName = pair.Key, Data = pair.Value.ToArray() });
            }
            group.Buffers = bonebuffers.ToArray().OrderBy(o => o.BoneName, StringComparer.Ordinal).ToArray();


            return group;
        }

    }
}
