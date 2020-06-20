using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using System.Collections.Generic;
using System.Linq;
using CrossMod.Rendering.GlTools;

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
            renderSettings.alphaBlendSettings = new SFGenericModel.RenderState.AlphaBlendSettings(true, material.BlendSrc, material.BlendDst, BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
            SFGenericModel.RenderState.GLRenderSettings.SetAlphaBlending(renderSettings.alphaBlendSettings);

            // Meshes with screen door transparency enable this OpenGL extension.
            if (RenderSettings.Instance.EnableExperimental && material.UseAlphaSampleCoverage)
                GL.Enable(EnableCap.SampleAlphaToCoverage);
            else
                GL.Disable(EnableCap.SampleAlphaToCoverage);

            SFGenericModel.RenderState.GLRenderSettings.SetFaceCulling(new SFGenericModel.RenderState.FaceCullingSettings(true, material.CullMode));
        }
    }
}
