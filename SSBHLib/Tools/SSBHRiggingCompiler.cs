using System;
using System.Collections.Generic;
using SSBHLib.Formats.Meshes;

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
            group.Buffers = new MeshBoneBuffer[boneNameToData.Count];
            int bufindex = 0;
            foreach(var pair in boneNameToData)
            {
                group.Buffers[bufindex++] = new MeshBoneBuffer() { BoneName = pair.Key, Data = pair.Value.ToArray() };
            }

            return group;
        }

    }
}
