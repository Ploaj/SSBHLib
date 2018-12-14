using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.VertexAttributes;

namespace CrossMod.Rendering.Models
{
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

        public CustomVertex(Vector3 position0, Vector3 normal0, Vector3 tangent0, Vector3 bitangent0, Vector2 map1)
        {
            Position0 = position0;
            Normal0 = normal0;
            Tangent0 = tangent0;
            Bitangent0 = bitangent0;
            Map1 = map1;
        }
    }
}
