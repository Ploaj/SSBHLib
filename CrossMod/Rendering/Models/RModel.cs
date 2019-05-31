using OpenTK;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.BufferObjects;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;

namespace CrossMod.Rendering.Models
{
    public class RModel : IRenderable
    {
        public Vector4 BoundingSphere { get; set; }

        Matrix4[] boneBinds = new Matrix4[200];
        public BufferObject boneUniformBuffer;

        public List<RMesh> subMeshes = new List<RMesh>();

        public RModel()
        {
            boneUniformBuffer = new BufferObject(BufferTarget.UniformBuffer);
        }

        public void Render(Camera Camera)
        {
            Render(Camera, null);
        }

        public void Render(Camera Camera, RSkeleton Skeleton = null)
        {
            Shader shader = GetCurrentShader();
            if (!shader.LinkStatusIsOk)
                return;

            shader.UseProgram();

            SetUniforms(shader);
            SetCameraUniforms(Camera, shader);

            shader.EnableVertexAttributes();

            // Bones
            int blockIndex = GL.GetUniformBlockIndex(shader.Id, "Bones");
            boneUniformBuffer.BindBase(BufferRangeTarget.UniformBuffer, blockIndex);
            if (Skeleton != null)
            {
                boneBinds = Skeleton.GetAnimationTransforms();
            }
            boneUniformBuffer.SetData(boneBinds, BufferUsageHint.DynamicDraw);

            DrawMeshes(Camera, Skeleton, shader);

            shader.DisableVertexAttributes();
        }

        private static void SetCameraUniforms(Camera Camera, Shader currentShader)
        {
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

            currentShader.SetVector3("V", Camera.ViewVector);
        }

        private static void SetUniforms(Shader currentShader)
        {
            currentShader.SetVector4("renderChannels", RenderSettings.Instance.renderChannels);
            currentShader.SetInt("renderMode", (int)RenderSettings.Instance.ShadingMode);

            currentShader.SetInt("transitionEffect", (int)RenderSettings.Instance.TransitionEffect);
            currentShader.SetFloat("transitionFactor", RenderSettings.Instance.TransitionFactor);

            currentShader.SetFloat("directLightIntensity", RenderSettings.Instance.DirectLightIntensity);
            currentShader.SetFloat("iblIntensity", RenderSettings.Instance.IblIntensity);

            currentShader.SetBoolToInt("renderDiffuse", RenderSettings.Instance.EnableDiffuse);
            currentShader.SetBoolToInt("renderSpecular", RenderSettings.Instance.EnableSpecular);
            currentShader.SetBoolToInt("renderEmission", RenderSettings.Instance.EnableEmission);
            currentShader.SetBoolToInt("renderRimLighting", RenderSettings.Instance.EnableRimLighting);
            currentShader.SetBoolToInt("renderExperimental", RenderSettings.Instance.EnableExperimental);

            currentShader.SetBoolToInt("renderNormalMaps", RenderSettings.Instance.RenderNormalMaps);
            currentShader.SetBoolToInt("renderVertexColor", RenderSettings.Instance.RenderVertexColor);

            currentShader.SetBoolToInt("renderWireframe", RenderSettings.Instance.EnableWireframe);
        }

        private void DrawMeshes(Camera Camera, RSkeleton Skeleton, Shader currentShader)
        {
            var opaqueZSorted = new List<RMesh>();
            var transparentZSorted = new List<RMesh>();

            foreach (RMesh m in subMeshes)
            {
                if (m.Material.HasAlphaBlending)
                    transparentZSorted.Add(m);
                else
                    opaqueZSorted.Add(m);
            }

            // TODO: Account for bounding sphere center in depth sorting.
            opaqueZSorted = opaqueZSorted.OrderBy(m => m.BoundingSphere.W).ToList();
            transparentZSorted = transparentZSorted.OrderBy(m => m.BoundingSphere.W).ToList();

            // Draw transparent meshes last for proper alpha blending.
            foreach (RMesh m in opaqueZSorted)
            {
                m.Draw(currentShader, Camera, Skeleton);
            }

            foreach (RMesh m in transparentZSorted)
            {
                m.Draw(currentShader, Camera, Skeleton);
            }
        }

        private static Shader GetCurrentShader()
        {
            if (RenderSettings.Instance.RenderUVs)
                return ShaderContainer.GetShader("RModelUV");
            else if (RenderSettings.Instance.UseDebugShading)
                return ShaderContainer.GetShader("RModelDebug");
            else
                return ShaderContainer.GetShader("RModel");
        }
    }
}
