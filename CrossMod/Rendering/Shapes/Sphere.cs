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
        private static readonly List<Vector3> UnitSphere;

        static Sphere()
        {
            vertexAttributes = new List<VertexAttribute>
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float, false)
            };
            UnitSphere = ShapeGenerator.GetSpherePositions(Vector3.Zero, 1, 30).Item1;
        }

        public Sphere() : base(UnitSphere.ToArray(), PrimitiveType.TriangleStrip) { }

        public void Render(Shader shader, float size, Vector3 offset, Matrix4 bone, Matrix4 mvp, Vector4 color)
        {
            shader.UseProgram();

            shader.SetFloat("size", size);
            shader.SetVector3("offset", offset);
            shader.SetMatrix4x4("bone", ref bone);
            shader.SetMatrix4x4("mvp", ref mvp);
            shader.SetVector4("color", color);

            Draw(shader);
        }
    }
}
