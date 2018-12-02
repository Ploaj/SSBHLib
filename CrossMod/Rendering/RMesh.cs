using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics;
using SFGenericModel;
using SFGraphics.GLObjects.Shaders;
using SFGraphics.Cameras;
using SFGenericModel.VertexAttributes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.BufferObjects;

namespace CrossMod.Rendering
{
    public class RModel : IRenderable
    {
        // Shaders
        public static Shader RModelShader;

        // Buffers
        public BufferObject VertexBuffer;
        public BufferObject IndexBuffer;

        // Sub Meshes
        public List<RMesh> Mesh = new List<RMesh>();

        public void Render(Camera Camera)
        {
            Render(Camera, null);
        }

        public void Render(Camera Camera, RSkeleton Skeleton = null)
        {
            if (RModelShader == null)
            {
                RModelShader = new Shader();
                RModelShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.vert"), OpenTK.Graphics.OpenGL.ShaderType.VertexShader);
                RModelShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.frag"), OpenTK.Graphics.OpenGL.ShaderType.FragmentShader);
                Console.WriteLine(RModelShader.GetErrorLog());
            }
            RModelShader.UseProgram();

            RModelShader.EnableVertexAttributes();

            // Camera
            Matrix4 View = Camera.MvpMatrix;
            RModelShader.SetMatrix4x4("mvp", ref View);

            // Bind Buffers
            IndexBuffer.Bind();
            VertexBuffer.Bind();

            // Draw Mesh
            foreach (RMesh m in Mesh)
            {
                m.Draw(RModelShader, Camera);
            }

            RModelShader.DisableVertexAttributes();
        }
    }

    public class RMesh : IRenderable
    {
        public string Name;

        public List<CustomVertexAttribute> VertexAttributes = new List<CustomVertexAttribute>();

        public int IndexOffset;
        public int IndexCount;

        public int SingleBindIndex;

        public void Draw(Shader s, Camera c)
        {

            foreach (CustomVertexAttribute a in VertexAttributes)
            {
                a.Bind(s);
            }

            GL.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedShort, IndexOffset);
        }

        public void Render(Camera Camera)
        {

        }
    }
}
