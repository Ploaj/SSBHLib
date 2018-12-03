using System;
using System.Collections.Generic;
using OpenTK;

namespace CrossMod.IO
{
    public struct IOVertex
    {
        Vector3 Position;
        Vector3 Normal;
        Vector3 Tangent;
        Vector2 UV0;
        Vector2 UV1;
        Vector3 UV2;
        Vector3 UV3;
        Vector4 BoneIndices;
        Vector4 BoneWeights;
    }

    public class IOMesh
    {
        public string Name;
        public List<IOVertex> Vertices = new List<IOVertex>();
        public List<uint> Indicies = new List<uint>();

        public bool HasPositions { get; set; } = false;
        public bool HasNormals { get; set; } = false;
        public bool HasUV0 { get; set; } = false;
        public bool HasUV1 { get; set; } = false;
        public bool HasUV2 { get; set; } = false;
        public bool HasUV3 { get; set; } = false;
        public bool HasWeights { get; set; } = false;
    }
}
