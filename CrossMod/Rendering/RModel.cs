using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.BufferObjects;
using SFGraphics.GLObjects.Shaders;
using System;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public class RModel : IRenderable
    {
        // Shaders
        private static Shader shader;

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
            if (shader == null)
            {
                shader = new Shader();
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.vert"), ShaderType.VertexShader);
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.frag"), ShaderType.FragmentShader);
                Console.WriteLine(shader.GetErrorLog());
            }
            shader.UseProgram();

            shader.EnableVertexAttributes();

            // Camera
            Matrix4 View = Camera.MvpMatrix;
            shader.SetMatrix4x4("mvp", ref View);

            // Bind Buffers
            IndexBuffer.Bind();
            VertexBuffer.Bind();

            // Draw Mesh
            foreach (RMesh m in Mesh)
            {
                m.Draw(shader, Camera, Skeleton);
            }

            shader.DisableVertexAttributes();
        }
    }
}
