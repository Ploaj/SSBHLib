using CrossMod.Rendering.GlTools;
using SFGenericModel.Materials;
using SFGraphics.Cameras;
using System;
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

            RModel.DrawMeshes(Meshes, shader, boneUniformBuffer);
        }
    }
}
