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

            if (Skeleton != null)
            {
                Matrix4[] WorldTransforms = Skeleton.GetWorldTransforms();
                //RModelShader.SetMatrix4x4("Transforms", WorldTransforms); // temp until ubo
                shader.SetMatrix4x4("Transforms", WorldTransforms);
            }

            // Bind Buffers
            IndexBuffer.Bind();
            VertexBuffer.Bind();

            // Draw Mesh
            foreach (RMesh m in Mesh)
            {
                m.Draw(shader, Camera);
            }

            shader.DisableVertexAttributes();
        }
    }

    public class RMesh : IRenderable
    {
        public string Name;

        public List<CustomVertexAttribute> VertexAttributes = new List<CustomVertexAttribute>();

        public int IndexOffset;
        public int IndexCount;

        public string SingleBindName = "";
        public int SingleBindIndex = -1;

        public Material Material;

        public void Draw(Shader s, Camera c)
        {
            s.SetInt("SingleBindIndex", SingleBindIndex);
            if(Material != null)
            {
                if(Material.COL != null)
                    s.SetTexture("_col", Material.COL, 0);
            }
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
