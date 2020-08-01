using CrossMod.Rendering.GlTools;
using OpenTK;
using SFGenericModel.Materials;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;

namespace CrossMod.Rendering.Models
{
    public class RModel : IRenderable
    {
        public Vector4 BoundingSphere { get; set; }
        public List<RMesh> SubMeshes { get; } = new List<RMesh>();

        private Matrix4[] boneBinds = new Matrix4[200];
        private readonly UniformBlock boneUniformBuffer;

        public RModel()
        {
            var shader = ShaderContainer.GetShader("RModel");
            if (shader.LinkStatusIsOk)
                boneUniformBuffer = new UniformBlock(shader, "Bones");
        }

        public void HideExpressionMeshes()
        {
            // TODO: Compile regex patterns and use that instead?
            string[] expressionPatterns = { "Blink", "Attack", "Ouch", "Talk",
                "Capture", "Ottotto", "Escape", "Half",
                "Pattern", "Result", "Harf", "Hot", "Heavy",
                "Voice", "Fura", "Catch", "Cliff", "FLIP",
                "Bound", "Down", "Final", "Result", "StepPose",
                "Sorori", "Fall", "Appeal", "Damage", "CameraHit", "laugh",
                "breath", "swell", "_low", "_bink", "inkMesh" };

            // TODO: This is probably not a very efficient way of doing this.
            foreach (var mesh in SubMeshes)
            {
                var meshName = mesh.Name.ToLower();
                foreach (var pattern in expressionPatterns)
                {
                    if (meshName.Contains("openblink") || meshName.Contains("belly_low") || meshName.Contains("facen"))
                        continue;

                    if (meshName.Contains(pattern.ToLower()))
                    {
                        mesh.Visible = false;
                    }
                }
            }
        }
        public void Render(Camera camera)
        {
            Render(camera, null);
        }

        public void Render(Camera camera, RSkeleton Skeleton = null)
        {
            Shader shader = ShaderContainer.GetCurrentRModelShader();
            if (!shader.LinkStatusIsOk)
                return;

            shader.UseProgram();

            SetUniforms(shader);
            SetCameraUniforms(camera, shader);

            // Bones
            boneUniformBuffer.BindBlock(shader);
            if (Skeleton != null)
            {
                boneBinds = Skeleton.GetAnimationTransforms();
            }
            boneUniformBuffer.SetValues("transforms", boneBinds);

            DrawMeshes(Skeleton, shader);
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

            currentShader.SetMatrix4x4("modelView", Camera.ModelViewMatrix);
            currentShader.SetVector3("cameraPos", Camera.TransformedPosition);
        }

        private static void SetUniforms(Shader currentShader)
        {
            currentShader.SetVector4("renderChannels", RenderSettings.Instance.renderChannels);
            currentShader.SetInt("renderMode", (int)RenderSettings.Instance.ShadingMode);

            currentShader.SetFloat("floatTestParam", RenderSettings.Instance.FloatTestParam);

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

        private void DrawMeshes(RSkeleton skeleton, Shader currentShader)
        {
            // TODO: _near, _far
            GroupSubMeshesByPass(out List<RMesh> opaqueMeshes, out List<RMesh> sortMeshes);

            // Meshes often share a material, so skip redundant and costly state changes.
            RMaterial previousMaterial = null;

            foreach (RMesh m in opaqueMeshes)
            {
                DrawMesh(m, skeleton, currentShader, previousMaterial);
                previousMaterial = m.Material;
            }

            // Shader labels with "_sort" get rendered in a second pass for proper alpha blending.
            foreach (RMesh m in sortMeshes)
            {
                DrawMesh(m, skeleton, currentShader, previousMaterial);
                previousMaterial = m.Material;
            }
        }

        private void GroupSubMeshesByPass(out List<RMesh> opaqueMeshes, out List<RMesh> sortMeshes)
        {
            opaqueMeshes = new List<RMesh>();
            sortMeshes = new List<RMesh>();

            // Meshes are split into render passes based on the shader label.
            foreach (RMesh m in SubMeshes)
            {
                if (m.Material.HasSortLabel)
                    sortMeshes.Add(m);
                else
                    opaqueMeshes.Add(m);
            }
        }

        private static void DrawMesh(RMesh m, RSkeleton skeleton, Shader currentShader, RMaterial previousMaterial)
        {
            // Check if the uniform values have already been set for this shader.
            if (previousMaterial == null || (m.Material != null && m.Material.MaterialLabel != previousMaterial.MaterialLabel))
            {
                m.Material.SetMaterialUniforms(currentShader, previousMaterial);
                m.Material.SetRenderState();
            }

            m.Draw(currentShader, skeleton);
        }
    }
}
