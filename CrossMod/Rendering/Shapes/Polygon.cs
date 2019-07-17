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
        private static Shader Shader;

        static Polygon()
        {
            Shader = ShaderContainer.GetShader("Polygon");
        }

        public Polygon(List<Vector3> shape) : base(shape, PrimitiveType.Polygon) { }

        public override List<VertexAttribute> GetVertexAttributes()
        {
            return new List<VertexAttribute>()
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float, false)
            };
        }

        public void Render(Matrix4 bone, Matrix4 mvp, Vector4 color)
        {
            Shader.UseProgram();
            
            Shader.SetMatrix4x4("bone", ref bone);
            Shader.SetMatrix4x4("mvp", ref mvp);
            Shader.SetVector4("color", color);

            Draw(Shader);
        }
    }
}
