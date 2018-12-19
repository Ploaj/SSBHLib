using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.VertexAttributes;

namespace CrossMod.Rendering.Models
{
    public struct IVec4
    {
        public int X { get; set; } 
        public int Y { get; set; }
        public int Z { get; set; }
        public int W { get; set; }
    }

    public struct CustomVertex
    {
        [VertexFloat("Position0", ValueCount.Three, VertexAttribPointerType.Float)]
        public Vector3 Position0 { get; }

        [VertexFloat("Normal0", ValueCount.Three, VertexAttribPointerType.Float)]
        public Vector3 Normal0 { get; }

        [VertexFloat("Tangent0", ValueCount.Three, VertexAttribPointerType.Float)]
        public Vector3 Tangent0 { get; }

        // Generated value.
        [VertexFloat("Bitangent0", ValueCount.Three, VertexAttribPointerType.Float)]
        public Vector3 Bitangent0 { get; }

        [VertexFloat("map1", ValueCount.Two, VertexAttribPointerType.Float)]
        public Vector2 Map1 { get; }

        [VertexInt("boneIndices", ValueCount.Four, VertexAttribIntegerType.UnsignedInt)]
        public IVec4 BoneIndices { get; }

        [VertexFloat("boneWeights", ValueCount.Four, VertexAttribPointerType.Float)]
        public Vector4 BoneWeights { get; }

        [VertexFloat("bake1", ValueCount.Two, VertexAttribPointerType.Float)]
        public Vector2 Bake1 { get; }

        [VertexFloat("colorSet1", ValueCount.Four, VertexAttribPointerType.Float)]
        public Vector4 ColorSet1 { get; }

        [VertexFloat("colorSet5", ValueCount.Four, VertexAttribPointerType.Float)]
        public Vector4 ColorSet5 { get; }

        public CustomVertex(Vector3 position0, Vector3 normal0, Vector3 tangent0, Vector3 bitangent0, Vector2 map1, IVec4 boneIndices, Vector4 boneWeights, Vector2 bake1, Vector4 colorSet1, Vector4 colorSet5)
        {
            Position0 = position0;
            Normal0 = normal0;
            Tangent0 = tangent0;
            Bitangent0 = bitangent0;
            Map1 = map1;
            BoneIndices = boneIndices;
            BoneWeights = boneWeights;
            Bake1 = bake1;
            ColorSet1 = colorSet1;
            ColorSet5 = colorSet5;
        }
    }
}
