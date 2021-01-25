using OpenTK;
using SSBHLib.Tools;

namespace CrossMod.Rendering
{
    public static class SsbhVertexAttributeExtensions
    {
        public static Vector4 ToVector4(this SsbhVertexAttribute attribute)
        {
            return new Vector4(attribute.X, attribute.Y, attribute.Z, attribute.W);
        }

        public static Vector3 ToVector3(this SsbhVertexAttribute attribute)
        {
            return new Vector3(attribute.X, attribute.Y, attribute.Z);
        }

        public static Vector2 ToVector2(this SsbhVertexAttribute attribute)
        {
            return new Vector2(attribute.X, attribute.Y);
        }
    }
}
