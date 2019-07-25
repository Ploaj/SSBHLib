using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using SFGenericModel.VertexAttributes;
using SFGraphics.GLObjects.Shaders;
using System;
using System.Collections.Generic;


namespace CrossMod.Rendering.Shapes
{
    public class Line : GenericMesh<Vector3>
    {
        private static readonly List<Vector3> unitLine = new List<Vector3>()
        {
            Vector3.Zero,
            Vector3.UnitZ
        };

        static Line()
        {
            vertexAttributes = new List<VertexAttribute>
            {
                new VertexFloatAttribute("point", ValueCount.Three, VertexAttribPointerType.Float, false)
            };
        }

        public Line() : base(unitLine, PrimitiveType.Lines) { }

        public void Render(Shader shader, float radians, float length, Vector3 trans, Matrix4 bone, Matrix4 mvp, Vector4 color)
        {
            shader.UseProgram();

            Vector3 endpoint = new Vector3(0, (float)Math.Sin(radians) * length, (float)Math.Cos(radians) * length);

            shader.SetMatrix4x4("bone1", ref bone);
            shader.SetMatrix4x4("bone2", ref bone);
            shader.SetVector3("trans1", trans);
            shader.SetVector3("trans2", trans);
            shader.SetVector3("off1", Vector3.Zero);
            shader.SetVector3("off2", endpoint);

            shader.SetMatrix4x4("mvp", ref mvp);

            shader.SetVector4("color", color);

            Draw(shader);
        }

        public void Render(Shader shader, Matrix4 bone1, Matrix4 bone2, Vector3 trans1, Vector3 trans2, Matrix4 mvp, Vector4 color)
        {
            shader.UseProgram();

            shader.SetMatrix4x4("bone1", ref bone1);
            shader.SetMatrix4x4("bone2", ref bone2);
            shader.SetVector3("trans1", trans1);
            shader.SetVector3("trans2", trans2);
            shader.SetVector3("off1", Vector3.Zero);
            shader.SetVector3("off2", Vector3.Zero);

            shader.SetMatrix4x4("mvp", ref mvp);

            shader.SetVector4("color", color);

            Draw(shader);
        }

        public void Render(Shader shader, Matrix4 bone1, Matrix4 bone2, Vector3 trans1, Vector3 trans2, Vector3 off1, Vector3 off2, Matrix4 mvp, Vector4 color)
        {
            shader.UseProgram();

            shader.SetMatrix4x4("bone1", ref bone1);
            shader.SetMatrix4x4("bone2", ref bone2);
            shader.SetVector3("trans1", trans1);
            shader.SetVector3("trans2", trans2);
            shader.SetVector3("off1", off1);
            shader.SetVector3("off2", off2);

            shader.SetMatrix4x4("mvp", ref mvp);

            shader.SetVector4("color", color);

            Draw(shader);
        }
    }
}
