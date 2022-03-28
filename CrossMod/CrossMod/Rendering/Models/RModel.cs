using CrossMod.Rendering.GlTools;
using OpenTK;
using SFGenericModel.Materials;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CrossMod.Rendering.Models
{
    public class RModel
    {
        public Vector4 BoundingSphere { get; set; }
        public List<RMesh> SubMeshes { get; } = new List<RMesh>();

        private Matrix4[] boneBinds = new Matrix4[200];
        private readonly UniformBlock? boneUniformBuffer;

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
                        mesh.IsVisible = false;
                    }
                }
            }
        }

        public void Render(Matrix4 modelView, Matrix4 projection, RSkeleton? skeleton = null)
        {
            Shader shader = ShaderContainer.GetCurrentRModelShader();
            if (!shader.LinkStatusIsOk)
                return;

            shader.UseProgram();

            SetRenderSettingsUniforms(shader);
            SetCameraUniforms(modelView, projection, shader);

            // Bones
            if (boneUniformBuffer != null)
            {
                boneUniformBuffer.BindBlock(shader);
                if (skeleton != null)
                {
                    boneBinds = skeleton.GetAnimationTransforms();
                }
                boneUniformBuffer.SetValues("transforms", boneBinds);
            }

            DrawMeshes(SubMeshes.Select(m => new Tuple<RMesh, RSkeleton?>(m, skeleton)), shader, boneUniformBuffer, modelView, projection);
        }

        public static void SetCameraUniforms(Matrix4 modelView, Matrix4 projection, Shader currentShader)
        {
            currentShader.SetMatrix4x4("mvp", modelView * projection);
            currentShader.SetMatrix4x4("modelView", modelView);
            currentShader.SetVector3("cameraPos", modelView.Inverted().ExtractTranslation());
        }

        public static void SetRenderSettingsUniforms(Shader currentShader)
        {
            currentShader.SetVector4("renderChannels", RenderSettings.Instance.renderChannels);
            currentShader.SetInt("renderMode", (int)RenderSettings.Instance.ShadingMode);
            currentShader.SetBoolToInt("useUvPattern", RenderSettings.Instance.UseUvPattern);

            currentShader.SetFloat("floatTestParam", RenderSettings.Instance.FloatTestParam);

            currentShader.SetFloat("directLightIntensity", RenderSettings.Instance.DirectLightIntensity);
            currentShader.SetFloat("iblIntensity", RenderSettings.Instance.IblIntensity);

            currentShader.SetBoolToInt("renderDiffuse", RenderSettings.Instance.EnableDiffuse);
            currentShader.SetBoolToInt("renderSpecular", RenderSettings.Instance.EnableSpecular);
            currentShader.SetBoolToInt("renderEmission", RenderSettings.Instance.EnableEmission);
            currentShader.SetBoolToInt("renderRimLighting", RenderSettings.Instance.EnableRimLighting);
            currentShader.SetBoolToInt("renderBakedLighting", RenderSettings.Instance.RenderBakedLighting);

            currentShader.SetBoolToInt("renderExperimental", RenderSettings.Instance.EnableExperimental);

            currentShader.SetBoolToInt("renderNorMaps", RenderSettings.Instance.RenderNorMaps);

            currentShader.SetBoolToInt("renderPrmMetalness", RenderSettings.Instance.RenderPrmMetalness);
            currentShader.SetBoolToInt("renderPrmRoughness", RenderSettings.Instance.RenderPrmRoughness);
            currentShader.SetBoolToInt("renderPrmAo", RenderSettings.Instance.RenderPrmAo);
            currentShader.SetBoolToInt("renderPrmSpec", RenderSettings.Instance.RenderPrmSpecular);

            currentShader.SetBoolToInt("renderVertexColor", RenderSettings.Instance.RenderVertexColor);

            currentShader.SetBoolToInt("renderMaterialErrors", RenderSettings.Instance.EnableMaterialValidationRendering);

            currentShader.SetBoolToInt("renderWireframe", RenderSettings.Instance.EnableWireframe);

            currentShader.SetBoolToInt("enableBloom", RenderSettings.Instance.EnableBloom);
            currentShader.SetFloat("bloomIntensity", RenderSettings.Instance.BloomIntensity);
            currentShader.SetBoolToInt("enablePostProcessing", RenderSettings.Instance.EnablePostProcessing);
        }

        private static int GetOrder(string shaderLabel)
        {
            // Shader labels with _sort or _far get rendered in a second pass for proper alpha blending.
            // Shader labels with _near get rendered last after post processing is done.

            if (shaderLabel.EndsWith("_opaque"))
                return 0;
            else if (shaderLabel.EndsWith("_far"))
                return 1;
            else if (shaderLabel.EndsWith("_sort"))
                return 2;
            else if (shaderLabel.EndsWith("_near"))
                return 3;
            else
                return 0;
        }

        public static void DrawMeshes(IEnumerable<Tuple<RMesh, RSkeleton?>> subMeshes, Shader currentShader, UniformBlock? boneUniformBuffer,
            Matrix4 modelView, Matrix4 projection)
        {
            // Meshes often share a material, so skip redundant and costly state changes.
            RMaterial? previousMaterial = null;
            RSkeleton? previousSkeleton = null;

            // Just put meshes without a shader label or proper render pass tag with opaque for now.
            foreach (var m in subMeshes.OrderBy(m => GetOrder(m?.Item1?.Material?.ShaderLabel ?? "") + m?.Item1?.SortBias ?? 0))
            {
                DrawMesh(m.Item1, m.Item2, currentShader, previousMaterial, boneUniformBuffer, previousSkeleton);
                previousMaterial = m.Item1.Material;
                previousSkeleton = m.Item2;

                // Render skeleton on top.
                if (RenderSettings.Instance.RenderBones)
                    m.Item2?.Render(modelView, projection);
            }
        }

        private static void DrawMesh(RMesh m, RSkeleton? skeleton, Shader currentShader, RMaterial? previousMaterial, UniformBlock? boneUniformBuffer, RSkeleton? previousSkeleton)
        {
            // TODO: This optimization is buggy.
            // Check if the uniform values have already been set for this shader.
            //if (previousMaterial == null || (m.Material != null && m.Material.MaterialLabel != previousMaterial.MaterialLabel))
            {
                m.Material?.SetMaterialUniforms(currentShader, previousMaterial, m.HasRequiredAttributes);
                m.Material?.SetRenderState(previousMaterial);

                var depthFunction = OpenTK.Graphics.OpenGL.DepthFunction.Lequal;
                if (!m.EnableDepthTest)
                    depthFunction = OpenTK.Graphics.OpenGL.DepthFunction.Always;

                var depthSettings = new SFGenericModel.RenderState.DepthTestSettings(m.EnableDepthTest, m.EnableDepthWrites, depthFunction);
                SFGenericModel.RenderState.GLRenderSettings.SetDepthTesting(depthSettings);
            }


            if (skeleton != null && skeleton != previousSkeleton)
            {
                var boneBinds = skeleton.GetAnimationTransforms();
                boneUniformBuffer?.SetValues("transforms", boneBinds);
            }

            m.Draw(currentShader, skeleton);
        }
    }
}
