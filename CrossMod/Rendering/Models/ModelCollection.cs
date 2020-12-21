using CrossMod.Rendering.GlTools;
using OpenTK;
using SFGenericModel.Materials;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossMod.Rendering.Models
{
    // TODO: A lot of rendering code is shared with RModel

    /// <summary>
    /// A collection of <see cref="RMesh"/> that handles render pass grouping and rendering.
    /// </summary>
    public class ModelCollection : IRenderable
    {
        public List<Tuple<RMesh, RSkeleton>> Meshes { get; } = new List<Tuple<RMesh, RSkeleton>>();

        private UniformBlock? boneUniformBuffer;

        public void Render(Camera camera)
        {
            var shader = ShaderContainer.GetCurrentRModelShader();
            if (!shader.LinkStatusIsOk)
                return;

            shader.UseProgram();

            if (boneUniformBuffer == null)
                boneUniformBuffer = new UniformBlock(shader, "Bones");

            boneUniformBuffer.BindBlock(shader);

            RModel.SetRenderSettingsUniforms(shader);
            RModel.SetCameraUniforms(camera, shader);

            DrawMeshes(Meshes, shader, boneUniformBuffer);
        }

        private static void DrawMeshes(List<Tuple<RMesh, RSkeleton>> subMeshes, Shader currentShader, UniformBlock boneUniformBuffer)
        {
            var sorted = subMeshes.Where(m => !m.Item1.Name.ToLower().Contains("inkmesh"));
            GroupSubMeshesByPass(sorted,
                out List<Tuple<RMesh, RSkeleton>> opaqueMeshes,
                out List<Tuple<RMesh, RSkeleton>> sortMeshes,
                out List<Tuple<RMesh, RSkeleton>> nearMeshes,
                out List<Tuple<RMesh, RSkeleton>> farMeshes);

            // Meshes often share a material, so skip redundant and costly state changes.
            RMaterial? previousMaterial = null;

            foreach (var m in opaqueMeshes)
            {
                DrawMesh(m.Item1, m.Item2, currentShader, previousMaterial, boneUniformBuffer);
                previousMaterial = m.Item1.Material;
            }

            // Shader labels with _sort or _far get rendered in a second pass for proper alpha blending.
            foreach (var m in farMeshes)
            {
                DrawMesh(m.Item1, m.Item2, currentShader, previousMaterial, boneUniformBuffer);
                previousMaterial = m.Item1.Material;
            }

            foreach (var m in sortMeshes)
            {
                DrawMesh(m.Item1, m.Item2, currentShader, previousMaterial, boneUniformBuffer);
                previousMaterial = m.Item1.Material;
            }

            // Shader labels with _near get rendered last after post processing is done.
            foreach (var m in nearMeshes)
            {
                DrawMesh(m.Item1, m.Item2, currentShader, previousMaterial, boneUniformBuffer);
                previousMaterial = m.Item1.Material;
            }
        }

        private static void DrawMesh(RMesh m, RSkeleton? skeleton, Shader currentShader, RMaterial? previousMaterial, UniformBlock boneUniformBuffer)
        {
            // Check if the uniform values have already been set for this shader.
            if (previousMaterial == null || (m.Material != null && m.Material.MaterialLabel != previousMaterial.MaterialLabel))
            {
                m.Material?.SetMaterialUniforms(currentShader, previousMaterial);
                m.Material?.SetRenderState();
            }

            if (skeleton != null)
            {
                var boneBinds = skeleton.GetAnimationTransforms();
                boneUniformBuffer.SetValues("transforms", boneBinds);
            }

            m.Draw(currentShader, skeleton);
        }

        private static void GroupSubMeshesByPass(IEnumerable<Tuple<RMesh, RSkeleton>> subMeshes,
            out List<Tuple<RMesh, RSkeleton>> opaqueMeshes,
            out List<Tuple<RMesh, RSkeleton>> sortMeshes,
            out List<Tuple<RMesh, RSkeleton>> nearMeshes,
            out List<Tuple<RMesh, RSkeleton>> farMeshes)
        {
            opaqueMeshes = new List<Tuple<RMesh, RSkeleton>>();
            sortMeshes = new List<Tuple<RMesh, RSkeleton>>();
            nearMeshes = new List<Tuple<RMesh, RSkeleton>>();
            farMeshes = new List<Tuple<RMesh, RSkeleton>>();

            // Meshes are split into render passes based on the shader label.
            foreach (var m in subMeshes)
            {
                if (m.Item1.Material == null)
                {
                    opaqueMeshes.Add(m);
                    continue;
                }

                // Unrecognized meshes will just be placed in the first pass.
                // TODO: Does the game use a red checkerboard for missing labels?
                if (m.Item1.Material.ShaderLabel.EndsWith("_far"))
                    farMeshes.Add(m);
                else if (m.Item1.Material.ShaderLabel.EndsWith("_sort"))
                    sortMeshes.Add(m);
                else if (m.Item1.Material.ShaderLabel.EndsWith("_near"))
                    nearMeshes.Add(m);
                else if (m.Item1.Material.ShaderLabel.EndsWith("_opaque"))
                    opaqueMeshes.Add(m);
                else
                    opaqueMeshes.Add(m);
            }
        }
    }
}
