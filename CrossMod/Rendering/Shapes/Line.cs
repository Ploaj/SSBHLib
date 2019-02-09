using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using SFGenericModel.VertexAttributes;
using SFGraphics.GLObjects.Shaders;
using SFShapes;
using System.Collections.Generic;


namespace CrossMod.Rendering.Shapes
{
    public class Line : GenericMesh<Vector3>
    {
        private static List<Vector3> UnitLine;
        private static Shader Shader;

        static Line()
        {
            UnitLine = new List<Vector3>()
            {
                Vector3.Zero,
                Vector3.UnitZ
            };
            Shader = ShaderContainer.GetShader("Line");
        }

        public Line() : base(UnitLine, PrimitiveType.Lines) { }

        public override List<VertexAttribute> GetVertexAttributes()
        {
            return new List<VertexAttribute>
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float)
            };
        }

        public void Render(float radians, float length, Vector3 offset, Matrix4 bone, Matrix4 mvp, Vector4 color)
        {
            Shader.UseProgram();

            Matrix4 rotation = Matrix4.CreateFromAxisAngle(-Vector3.UnitX, radians);

            Shader.SetFloat("size", length);
            Shader.SetVector3("offset", offset);
            Shader.SetMatrix4x4("bone", ref bone);
            Shader.SetMatrix4x4("rotation", ref rotation);
            Shader.SetMatrix4x4("mvp", ref mvp);
            Shader.SetVector4("color", color);

            Draw(Shader, null);
        }
    }
}
