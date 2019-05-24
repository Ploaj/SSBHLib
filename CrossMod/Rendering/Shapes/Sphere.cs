using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using SFGenericModel.VertexAttributes;
using SFGraphics.GLObjects.Shaders;
using SFShapes;
using System.Collections.Generic;

namespace CrossMod.Rendering.Shapes
{
    public class Sphere : GenericMesh<Vector3>
    {
        private static List<Vector3> UnitSphere;
        private static Shader Shader;
        
        static Sphere()
        {
            UnitSphere = ShapeGenerator.GetSpherePositions(Vector3.Zero, 1, 30).Item1;
            Shader = ShaderContainer.GetShader("Sphere");
        }

        public Sphere() : base(UnitSphere, PrimitiveType.TriangleStrip) { }

        public override List<VertexAttribute> GetVertexAttributes()
        {
            return new List<VertexAttribute>()
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float, false)
            };
        }

        public void Render(float size, Vector3 offset, Matrix4 bone, Matrix4 mvp, Vector4 color)
        {
            Shader.UseProgram();

            Shader.SetFloat("size", size);
            Shader.SetVector3("offset", offset);
            Shader.SetMatrix4x4("bone", ref bone);
            Shader.SetMatrix4x4("mvp", ref mvp);
            Shader.SetVector4("color", color);

            Draw(Shader);
        }
    }
}
