using System.Collections.Generic;
using OpenTK;

namespace CrossMod.IO
{
    public struct IOVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector2 UV0;
        public Vector2 UV1;
        public Vector2 UV2;
        public Vector2 UV3;
        public Vector4 Color;
        public Vector4 BoneIndices;
        public Vector4 BoneWeights;
    }

    public class IOMesh
    {
        public string Name { get; set; }
        public List<IOVertex> Vertices { get; } = new List<IOVertex>();
        public List<uint> Indices { get; } = new List<uint>();

        public int MaterialIndex = -1;

        public bool HasPositions { get; set; } = false;
        public bool HasNormals { get; set; } = false;
        public bool HasUV0 { get; set; } = false;
        public bool HasUV1 { get; set; } = false;
        public bool HasUV2 { get; set; } = false;
        public bool HasUV3 { get; set; } = false;
        public bool HasColor { get; set; } = false;
        public bool HasBoneWeights { get; set; } = false;
    }
}
