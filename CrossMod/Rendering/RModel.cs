using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.BufferObjects;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public class RModel : IRenderable
    {
        public static Shader shader;
        public static Shader textureDebugShader;

        public BufferObject vertexBuffer;
        public BufferObject indexBuffer;

        public List<RMesh> subMeshes = new List<RMesh>();

        public void Render(Camera Camera)
        {
            Render(Camera, null);
        }

        public void Render(Camera Camera, RSkeleton Skeleton = null)
        {
            SetUpShaders();

            Shader currentShader = GetCurrentShader();
            if (!currentShader.LinkStatusIsOk)
                return;

            currentShader.UseProgram();

            currentShader.SetVector4("renderChannels", RenderSettings.renderChannels);
            currentShader.SetInt("renderMode", RenderSettings.renderMode);

            currentShader.EnableVertexAttributes();

            // Camera
            Matrix4 View = Camera.MvpMatrix;
            currentShader.SetMatrix4x4("mvp", ref View);

            // Bind Buffers
            indexBuffer.Bind();
            vertexBuffer.Bind();

            DrawMeshes(Camera, Skeleton, currentShader);

            currentShader.DisableVertexAttributes();
        }

        private void DrawMeshes(Camera Camera, RSkeleton Skeleton, Shader currentShader)
        {
            foreach (RMesh m in subMeshes)
            {
                m.Draw(currentShader, Camera, Skeleton);
            }
        }

        private static Shader GetCurrentShader()
        {
            Shader currentShader = shader;
            if (RenderSettings.useDebugShading)
                currentShader = textureDebugShader;
            return currentShader;
        }

        private static void SetUpShaders()
        {
            if (shader == null)
            {
                shader = new Shader();
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.vert"), ShaderType.VertexShader);
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.frag"), ShaderType.FragmentShader);
                System.Diagnostics.Debug.WriteLine(shader.GetErrorLog());
            }

            if (textureDebugShader == null)
            {
                textureDebugShader = new Shader();
                textureDebugShader.LoadShader(System.IO.File.ReadAllText("Shaders/RTexDebug.frag"), ShaderType.FragmentShader);
                textureDebugShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.vert"), ShaderType.VertexShader);
                System.Diagnostics.Debug.WriteLine(textureDebugShader.GetErrorLog());
            }
        }
    }
}
