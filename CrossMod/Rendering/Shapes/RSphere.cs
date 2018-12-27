using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using SFGenericModel;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.VertexAttributes;
using SFGraphics.GLObjects.Shaders;
using SFGraphics.Cameras;

namespace CrossMod.Rendering
{
    public class RSphere : GenericMesh<Vector3>
    {
        private static Shader sphereShader;

        public RSphere() : base(SFShapes.ShapeGenerator.GetSpherePositions(Vector3.Zero, 1, 20).Item1, PrimitiveType.TriangleStrip)
        {

        }


        public override List<VertexAttribute> GetVertexAttributes()
        {
            return new List<VertexAttribute>()
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float),
            };
        }

        public void RenderSphere(Camera Camera, float Size, Vector3 Position, Matrix4 Bone)
        {
            if(sphereShader == null)
            {
                sphereShader = new Shader();
                sphereShader.LoadShader(System.IO.File.ReadAllText("Shaders/Sphere.frag"), ShaderType.FragmentShader);
                sphereShader.LoadShader(System.IO.File.ReadAllText("Shaders/Sphere.vert"), ShaderType.VertexShader);
            }

            sphereShader.UseProgram();

            sphereShader.SetVector4("sphereColor", new Vector4(1, 0, 0, 0.5f));


            Matrix4 mvp = Camera.MvpMatrix;
            sphereShader.SetMatrix4x4("mvp", ref mvp);

            sphereShader.SetVector3("offset", Position);

            Bone = Bone.ClearScale();
            sphereShader.SetMatrix4x4("bone", ref Bone);

            sphereShader.SetFloat("Size", Size);


            Draw(sphereShader, null);
        }
    }
}