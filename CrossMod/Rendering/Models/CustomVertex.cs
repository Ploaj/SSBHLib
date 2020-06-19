using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.VertexAttributes;
using SSBHLib.Tools;

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
        [VertexFloat("Position0", ValueCount.Three, VertexAttribPointerType.Float, false, AttributeUsage.Position, false, false)]
        public Vector3 Position0 { get; }

        [VertexFloat("Normal0", ValueCount.Three, VertexAttribPointerType.Float, false)]
        public Vector3 Normal0 { get; }

        [VertexFloat("Tangent0", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 Tangent0 { get; }

        [VertexFloat("map1", ValueCount.Two, VertexAttribPointerType.Float, false)]
        public Vector2 Map1 { get; }

        [VertexFloat("uvSet", ValueCount.Two, VertexAttribPointerType.Float, false)]
        public Vector2 UvSet { get; }

        [VertexFloat("uvSet1", ValueCount.Two, VertexAttribPointerType.Float, false)]
        public Vector2 UvSet1 { get; }

        [VertexFloat("uvSet2", ValueCount.Two, VertexAttribPointerType.Float, false)]
        public Vector2 UvSet2 { get; }

        [VertexInt("boneIndices", ValueCount.Four, VertexAttribIntegerType.UnsignedInt)]
        public IVec4 BoneIndices { get; }

        [VertexFloat("boneWeights", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 BoneWeights { get; }

        [VertexFloat("bake1", ValueCount.Two, VertexAttribPointerType.Float, false)]
        public Vector2 Bake1 { get; }

        [VertexFloat("colorSet1", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet1 { get; }

        [VertexFloat("colorSet2", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet2 { get; }

        [VertexFloat("colorSet2_1", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet21 { get; }

        [VertexFloat("colorSet2_2", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet22 { get; }

        [VertexFloat("colorSet2_3", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet23 { get; }

        [VertexFloat("colorSet3", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet3 { get; }

        [VertexFloat("colorSet4", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet4 { get; }

        [VertexFloat("colorSet5", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet5 { get; }

        [VertexFloat("colorSet6", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet6 { get; }

        [VertexFloat("colorSet7", ValueCount.Four, VertexAttribPointerType.Float, false)]
        public Vector4 ColorSet7 { get; }

        public CustomVertex(
            SsbhVertexAttribute position0, SsbhVertexAttribute normal0, SsbhVertexAttribute tangent0, 
            SsbhVertexAttribute map1, SsbhVertexAttribute uvSet, SsbhVertexAttribute uvSet1, SsbhVertexAttribute uvSet2, 
            IVec4 boneIndices, Vector4 boneWeights, SsbhVertexAttribute bake1, 
            SsbhVertexAttribute colorSet1, SsbhVertexAttribute colorSet2, SsbhVertexAttribute colorSet21, SsbhVertexAttribute colorSet22, SsbhVertexAttribute colorSet23, 
            SsbhVertexAttribute colorSet3, SsbhVertexAttribute colorSet4, SsbhVertexAttribute colorSet5, SsbhVertexAttribute colorSet6, SsbhVertexAttribute colorSet7)
        {
            // TODO: Attributes could use vec4 in the shaders and avoid the conversion.
            // vec2 attributes can be packed together to save vertex attributes.
            // Intel/Nvidia have a max of 16 vertex attributes.
            Position0 = position0.ToVector3();
            Normal0 = normal0.ToVector3();
            Tangent0 = tangent0.ToVector4();
            Map1 = map1.ToVector2();
            UvSet = uvSet.ToVector2();
            UvSet1 = uvSet1.ToVector2();
            UvSet2 = uvSet2.ToVector2();
            BoneIndices = boneIndices;
            BoneWeights = boneWeights;
            Bake1 = bake1.ToVector2();

            // TODO: This operation could be done in OpenGL to improve performance.
            ColorSet1 = colorSet1.ToVector4() / 128.0f;
            ColorSet2 = colorSet2.ToVector4() / 128.0f;
            ColorSet21 = colorSet21.ToVector4() / 128.0f;
            ColorSet22 = colorSet22.ToVector4() / 128.0f;
            ColorSet23 = colorSet23.ToVector4() / 128.0f;
            ColorSet3 = colorSet3.ToVector4() / 128.0f;
            ColorSet4 = colorSet4.ToVector4() / 128.0f;
            ColorSet5 = colorSet5.ToVector4() / 128.0f;
            ColorSet6 = colorSet6.ToVector4() / 128.0f;
            ColorSet7 = colorSet7.ToVector4() / 128.0f;
        }
    }
}
