﻿using OpenTK;
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
                new VertexFloatAttribute("point", ValueCount.Three, VertexAttribPointerType.Float, false)
            };
        }

        public void Render(float radians, float length, Vector3 trans, Matrix4 bone, Matrix4 mvp, Vector4 color)
        {
            Shader.UseProgram();

            Vector3 endpoint = new Vector3(0, (float)Math.Sin(radians) * length, (float)Math.Cos(radians) * length);

            Shader.SetMatrix4x4("bone1", ref bone);
            Shader.SetMatrix4x4("bone2", ref bone);
            Shader.SetVector3("trans1", trans);
            Shader.SetVector3("trans2", trans);
            Shader.SetVector3("off1", Vector3.Zero);
            Shader.SetVector3("off2", endpoint);

            Shader.SetMatrix4x4("mvp", ref mvp);

            Shader.SetVector4("color", color);

            Draw(Shader);
        }

        public void Render(Matrix4 bone1, Matrix4 bone2, Vector3 trans1, Vector3 trans2, Matrix4 mvp, Vector4 color)
        {
            Shader.UseProgram();

            Shader.SetMatrix4x4("bone1", ref bone1);
            Shader.SetMatrix4x4("bone2", ref bone2);
            Shader.SetVector3("trans1", trans1);
            Shader.SetVector3("trans2", trans2);
            Shader.SetVector3("off1", Vector3.Zero);
            Shader.SetVector3("off2", Vector3.Zero);

            Shader.SetMatrix4x4("mvp", ref mvp);

            Shader.SetVector4("color", color);

            Draw(Shader);
        }

        public void Render(Matrix4 bone1, Matrix4 bone2, Vector3 trans1, Vector3 trans2, Vector3 off1, Vector3 off2, Matrix4 mvp, Vector4 color)
        {
            Shader.UseProgram();

            Shader.SetMatrix4x4("bone1", ref bone1);
            Shader.SetMatrix4x4("bone2", ref bone2);
            Shader.SetVector3("trans1", trans1);
            Shader.SetVector3("trans2", trans2);
            Shader.SetVector3("off1", off1);
            Shader.SetVector3("off2", off2);

            Shader.SetMatrix4x4("mvp", ref mvp);

            Shader.SetVector4("color", color);

            Draw(Shader);
        }
    }
}