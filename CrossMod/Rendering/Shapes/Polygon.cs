using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using SFGenericModel.VertexAttributes;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;

namespace CrossMod.Rendering.Shapes
{
    class Polygon : GenericMesh<Vector3>
    {
        static Polygon()
        {
            vertexAttributes = new List<VertexAttribute>
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float, false)
            };
        }

        public Polygon(List<Vector3> shape) : base(shape.ToArray(), PrimitiveType.Polygon) { }

        public void Render(Shader shader, Matrix4 bone, Matrix4 mvp, Vector4 color)
        {
            shader.UseProgram();

            shader.SetMatrix4x4("bone", ref bone);
            shader.SetMatrix4x4("mvp", ref mvp);
            shader.SetVector4("color", color);

            Draw(shader);
        }
    }
}
