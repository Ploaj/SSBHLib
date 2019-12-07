using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using SFGenericModel.VertexAttributes;
using SFGraphics.GLObjects.Shaders;
using SFShapes;
using System;
using System.Collections.Generic;

namespace CrossMod.Rendering.Shapes
{
    public class Capsule : GenericMesh<Vector4>
    {
        private static readonly List<Vector4> unitCapsule;

        static Capsule()
        {
            vertexAttributes = new List<VertexAttribute>
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float, false)
            };

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
            unitCapsule = capsule;
        }

        public Capsule() : base (unitCapsule.ToArray(), PrimitiveType.TriangleStrip) { }

        public void Render(Shader shader, float size, Vector3 offset1, Vector3 offset2, Matrix4 bone, Matrix4 mvp, Vector4 color)
        {
            shader.UseProgram();

            var dir = Vector3.Normalize(offset2 - offset1);
            Matrix4 orientation;
            if (dir.X == 0 && dir.Z == 0)
            {
                orientation = new Matrix4(Vector4.UnitX, new Vector4(dir), Vector4.UnitZ, Vector4.UnitW);
            }
            else
            {
                var axis = Vector3.Cross(Vector3.UnitY, dir);
                var angle = (float)Math.Acos(Vector3.Dot(Vector3.UnitY, dir));
                orientation = Matrix4.CreateFromAxisAngle(axis, angle);
            }

            shader.SetFloat("size", size);
            shader.SetMatrix4x4("orientation", ref orientation);
            shader.SetVector3("offset1", offset1);
            shader.SetVector3("offset2", offset2);
            shader.SetMatrix4x4("bone", ref bone);
            shader.SetMatrix4x4("mvp", ref mvp);
            shader.SetVector4("color", color);

            Draw(shader);
        }
    }
}
