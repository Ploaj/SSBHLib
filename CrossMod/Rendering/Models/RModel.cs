using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.BufferObjects;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;

namespace CrossMod.Rendering.Models
{
    public class RModel : IRenderable
    {
        public static Shader shader;
        public static Shader textureDebugShader;
        public static Shader uvShader;

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

            SetUniforms(currentShader);

            currentShader.EnableVertexAttributes();

            // Camera
            if (RenderSettings.Instance.RenderUVs)
            {
                // TODO: Adjust scale.
                // Flip UVs vertically.
                float scale = 2;
                Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(-scale, scale, scale, -scale, -scale, scale);
                currentShader.SetMatrix4x4("mvp", ref mvp);
            }
            else
            {
                Matrix4 mvp = Camera.MvpMatrix;
                currentShader.SetMatrix4x4("mvp", ref mvp);
            }

            // Bones
            int blockIndex = GL.GetUniformBlockIndex(shader.Id, "Bones");
            boneBuffer.BindBase(BufferRangeTarget.UniformBuffer, blockIndex);
            if (Skeleton != null)
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

        private static void SetUniforms(Shader currentShader)
        {
            currentShader.SetVector4("renderChannels", RenderSettings.Instance.renderChannels);
            currentShader.SetInt("renderMode", (int)RenderSettings.Instance.ShadingMode);

            currentShader.SetInt("transitionEffect", (int)RenderSettings.Instance.TransitionEffect);
            currentShader.SetFloat("transitionFactor", RenderSettings.Instance.TransitionFactor);

            currentShader.SetBoolToInt("renderDiffuse", RenderSettings.Instance.EnableDiffuse);
            currentShader.SetBoolToInt("renderSpecular", RenderSettings.Instance.EnableSpecular);
            currentShader.SetBoolToInt("renderEmission", RenderSettings.Instance.EnableEmission);

            currentShader.SetBoolToInt("renderWireframe", RenderSettings.Instance.EnableWireframe);
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
            if (RenderSettings.Instance.RenderUVs)
                return uvShader;
            else if (RenderSettings.Instance.UseDebugShading)
                return textureDebugShader;
            else 
                return shader;
        }

        private static void SetUpShaders()
        {
            if (shader == null)
            {
                shader = new Shader();
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.vert"), ShaderType.VertexShader);
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.geom"), ShaderType.GeometryShader);
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.frag"), ShaderType.FragmentShader);
                shader.LoadShader(System.IO.File.ReadAllText("Shaders/Wireframe.frag"), ShaderType.FragmentShader);

                System.Diagnostics.Debug.WriteLine(shader.GetErrorLog());
            }

            if (uvShader == null)
            {
                uvShader = new Shader();
                uvShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModelUV.vert"), ShaderType.VertexShader);
                uvShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.geom"), ShaderType.GeometryShader);
                uvShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModelUV.frag"), ShaderType.FragmentShader);
                uvShader.LoadShader(System.IO.File.ReadAllText("Shaders/Wireframe.frag"), ShaderType.FragmentShader);

                System.Diagnostics.Debug.WriteLine(uvShader.GetErrorLog());
            }

            if (textureDebugShader == null)
            {
                textureDebugShader = new Shader();
                textureDebugShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModelDebug.frag"), ShaderType.FragmentShader);
                textureDebugShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.geom"), ShaderType.GeometryShader);
                textureDebugShader.LoadShader(System.IO.File.ReadAllText("Shaders/RModel.vert"), ShaderType.VertexShader);
                textureDebugShader.LoadShader(System.IO.File.ReadAllText("Shaders/Wireframe.frag"), ShaderType.FragmentShader);

                System.Diagnostics.Debug.WriteLine(textureDebugShader.GetErrorLog());
            }
        }
    }
}
