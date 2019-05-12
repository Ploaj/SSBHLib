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
            renderSettings.alphaBlendSettings = new SFGenericModel.RenderState.AlphaBlendSettings(true, material.BlendSrc, material.BlendDst, BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
        }
    }
}
