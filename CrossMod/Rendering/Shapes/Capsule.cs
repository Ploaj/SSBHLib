using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using SFGenericModel.VertexAttributes;
using SFGraphics.GLObjects.Shaders;
using SFShapes;
using System.Collections.Generic;
using System.IO;

namespace CrossMod.Rendering.Shapes
{
    public class Capsule : GenericMesh<Vector4>
    {
        private static List<Vector4> UnitCapsule;
        private static Shader Shader;

        static Capsule()
        {
            List<Vector3> baseSphere = ShapeGenerator.GetSpherePositions(Vector3.Zero, 1, 30).Item1;
            var capsule = new List<Vector4>();
            foreach (var v in baseSphere)
            {
                Vector4 value = new Vector4();
                value.Xyz = v;
                if (value.Y > 0)
                    value.W = 1;
                capsule.Add(value);
            }
            UnitCapsule = capsule;

            Shader = new Shader();
            Shader.LoadShader(File.ReadAllText("Shaders/SolidColor.frag"), ShaderType.FragmentShader);
            Shader.LoadShader(File.ReadAllText("Shaders/Capsule.vert"), ShaderType.VertexShader);
        }

        public Capsule() : base (UnitCapsule, PrimitiveType.TriangleStrip) { }

        public override List<VertexAttribute> GetVertexAttributes()
        {
            return new List<VertexAttribute>()
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float)
            };
        }

        public void Render(float size, Vector3 offset, Vector3 offset2, Matrix4 boneTransform, Matrix4 mvp, Vector4 color)
        {
            Shader.UseProgram();

            Shader.SetFloat("size", size);
            Shader.SetMatrix4x4("mvp", ref mvp);
            Shader.SetVector4("color", color);

            Vector3 position1 = Vector3.TransformPosition(offset, boneTransform);
            Vector3 position2 = Vector3.TransformPosition(offset2, boneTransform);
            Vector3 to = position2 - position1;
            to.NormalizeFast();

            Vector3 axis = Vector3.Cross(Vector3.UnitY, to);
            float omega = (float)System.Math.Acos(Vector3.Dot(Vector3.UnitY, to));
            Matrix4 rotation = Matrix4.CreateFromAxisAngle(axis, omega);

            Matrix4 transform1 = rotation * Matrix4.CreateTranslation(position1);
            Matrix4 transform2 = rotation * Matrix4.CreateTranslation(position2);

            Shader.SetMatrix4x4("transform1", ref transform1);
            Shader.SetMatrix4x4("transform2", ref transform2);

            Draw(Shader, null);
        }
    }
}
