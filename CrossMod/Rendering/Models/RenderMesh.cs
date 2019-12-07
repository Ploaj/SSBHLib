using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using System.Collections.Generic;
using System.Linq;

namespace CrossMod.Rendering.Models
{
    public class RenderMesh : GenericMesh<CustomVertex>
    {
        private readonly SFGenericModel.RenderState.RenderSettings renderSettings = new SFGenericModel.RenderState.RenderSettings();

        public RenderMesh(IList<CustomVertex> vertices, IList<uint> indices) : base(vertices.ToArray(), indices.ToArray(), PrimitiveType.Triangles)
        {

        }

        public void SetRenderState(Material material)
        {
            // TODO: The screen door transparency may also use alpha blending.
            renderSettings.alphaBlendSettings = new SFGenericModel.RenderState.AlphaBlendSettings(!material.UseStippleBlend, material.BlendSrc, material.BlendDst, BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
            SFGenericModel.RenderState.GLRenderSettings.SetAlphaBlending(renderSettings.alphaBlendSettings);
        }
    }
}
