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
        public SFGenericModel.Materials.UniformBlock boneUniformBuffer;

        public List<RMesh> subMeshes = new List<RMesh>();

        public RModel()
        {
            boneUniformBuffer = new SFGenericModel.Materials.UniformBlock(ShaderContainer.GetShader("RModel"), "Bones");
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

            // Bones
            boneUniformBuffer.BindBlock(shader, "Bones");
            if (Skeleton != null)
            {
                boneBinds = Skeleton.GetAnimationTransforms();
            }
            boneUniformBuffer.SetValues("transforms", boneBinds);

            DrawMeshes(Camera, Skeleton, shader);
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

            currentShader.SetVector3("cameraPos", Camera.Position);
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
            var opaque = new List<RMesh>();
            var transparentDepthSorted = new List<RMesh>();

            foreach (RMesh m in subMeshes)
            {
                if (m.Material.HasAlphaBlending)
                    transparentDepthSorted.Add(m);
                else
                    opaque.Add(m);
            }

            transparentDepthSorted = transparentDepthSorted.OrderBy(m => (Camera.Position - m.BoundingSphere.Xyz).Length + m.BoundingSphere.W).ToList();

            // Models often share a material, so skip redundant and costly state changes.
            string previousMaterialName = "";

            foreach (RMesh m in opaque)
            {
                DrawMesh(Camera, Skeleton, currentShader, previousMaterialName, m);
                previousMaterialName = m.Material.Name;
            }

            // Draw transparent meshes last for proper alpha blending.
            foreach (RMesh m in transparentDepthSorted)
            {
                DrawMesh(Camera, Skeleton, currentShader, previousMaterialName, m);
                previousMaterialName = m.Material.Name;
            }
        }

        private static void DrawMesh(Camera Camera, RSkeleton Skeleton, Shader currentShader, string previousMaterialName, RMesh m)
        {
            if (m.Material != null && m.Material.Name != previousMaterialName)
            {
                m.SetMaterialUniforms(currentShader);
                m.RenderMesh.SetRenderState(m.Material);
            }

            m.Draw(currentShader, Camera, Skeleton);
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
