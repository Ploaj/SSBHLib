using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using System.Collections.Generic;

namespace CrossMod.Rendering.Models
{
    public class RenderMesh : GenericMesh<CustomVertex>
    {
        private SFGenericModel.RenderState.RenderSettings renderSettings = new SFGenericModel.RenderState.RenderSettings();

        public RenderMesh(List<CustomVertex> vertices, List<uint> indices) : base(vertices, indices, PrimitiveType.Triangles)
        {

        }

        public void SetRenderState(Material material)
        {
            // TODO: The screen door transparency may also use alpha blending.
            renderSettings.alphaBlendSettings = new SFGenericModel.RenderState.AlphaBlendSettings(!material.UseStippleBlend, material.BlendSrc, material.BlendDst, BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
            SFGenericModel.RenderState.GLRenderSettings.SetRenderSettings(renderSettings);
        }
    }
}
