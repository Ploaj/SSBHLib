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

        Matrix4[] boneBinds = new Matrix4[200];
        public BufferObject boneBuffer;
        public BufferObject vertexBuffer;
        public BufferObject indexBuffer;

        public List<RMesh> subMeshes = new List<RMesh>();

        public RModel()
        {
            boneBuffer = new BufferObject(BufferTarget.UniformBuffer);
        }

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

            currentShader.SetVector4("renderChannels", RenderSettings.Instance.renderChannels);
            currentShader.SetInt("renderMode", (int)RenderSettings.Instance.renderMode);
            currentShader.SetBoolToInt("renderDiffuse", RenderSettings.Instance.enableDiffuse);
            currentShader.SetBoolToInt("renderSpecular", RenderSettings.Instance.enableSpecular);
            currentShader.SetBoolToInt("renderWireframe", RenderSettings.Instance.enableWireframe);
            currentShader.SetBoolToInt("useDittoForm", RenderSettings.Instance.useDittoForm);

            currentShader.EnableVertexAttributes();

            // Camera
            Matrix4 View = Camera.MvpMatrix;
            currentShader.SetMatrix4x4("mvp", ref View);

            // Bones
            int blockIndex = GL.GetUniformBlockIndex(shader.Id, "bones");
            boneBuffer.BindBase(BufferRangeTarget.UniformBuffer, blockIndex);
            if(Skeleton != null)
            {
                boneBinds = Skeleton.GetAnimationTransforms();
                /*for(int i = 0; i < Skeleton.Bones.Count; i++)
                {
                    boneBinds[i] = Skeleton.GetInvWorldTransforms()[i] * Skeleton.GetWorldTransforms()[i];
                }*/
            }
            boneBuffer.SetData(boneBinds, BufferUsageHint.DynamicDraw);

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
            if (RenderSettings.Instance.UseDebugShading)
                currentShader = textureDebugShader;
            return currentShader;
        }

        private static void SetUpShaders()
        {
            if (shader == null)
            {
                shader = new Shader();
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.vert"), ShaderType.VertexShader);
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.geom"), ShaderType.GeometryShader);
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.frag"), ShaderType.FragmentShader);
                System.Diagnostics.Debug.WriteLine(shader.GetErrorLog());
            }

            if (textureDebugShader == null)
            {
                textureDebugShader = new Shader();
                textureDebugShader.LoadShader(System.IO.File.ReadAllText("Shaders/RTexDebug.frag"), ShaderType.FragmentShader);
                textureDebugShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.geom"), ShaderType.GeometryShader);
                textureDebugShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.vert"), ShaderType.VertexShader);
                System.Diagnostics.Debug.WriteLine(textureDebugShader.GetErrorLog());
            }
        }
    }
}
