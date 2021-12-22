using CrossMod.Rendering.GlTools;
using SFGenericModel.Materials;
using SFGraphics.Cameras;
using System;
using OpenTK;
using System.Collections.Generic;

namespace CrossMod.Rendering.Models
{
    /// <summary>
    /// A collection of <see cref="RMesh"/> that handles render pass grouping and rendering.
    /// </summary>
    public class ModelCollection : IRenderable
    {
        public List<Tuple<RMesh, RSkeleton?>> Meshes { get; } = new List<Tuple<RMesh, RSkeleton?>>();

        private UniformBlock? boneUniformBuffer;

        // HACK: Keep track of what numdlb nodes have already been added.
        // TODO: This needs to be rewritten at some point.
        public List<string> ModelNames { get; } = new List<string>();

        /// <summary>
        /// The bounding sphere containing all spheres added by <see cref="AddBoundingSphere(Vector4)"/>.
        /// </summary>
        public Vector4 BoundingSphere { get; private set; }

        public Dictionary<string, RTexture> TextureByName { get; } = new Dictionary<string, RTexture>();

        public void AddBoundingSphere(Vector4 newMeshBoundingSphere)
        {
            // Keep extending the bounding sphere as needed.
            BoundingSphere = SFGraphics.Utils.BoundingSphereGenerator.GenerateBoundingSphere(new Vector4[] { BoundingSphere, newMeshBoundingSphere });
        }

        public void Render(Matrix4 modelView, Matrix4 projection)
        {
            var shader = ShaderContainer.GetCurrentRModelShader();
            if (!shader.LinkStatusIsOk)
                return;

            shader.UseProgram();

            if (boneUniformBuffer == null)
                boneUniformBuffer = new UniformBlock(shader, "Bones");

            boneUniformBuffer.BindBlock(shader);

            RModel.SetRenderSettingsUniforms(shader);
            RModel.SetCameraUniforms(modelView, projection, shader);

            RModel.DrawMeshes(Meshes, shader, boneUniformBuffer, modelView, projection);
        }
    }
}
