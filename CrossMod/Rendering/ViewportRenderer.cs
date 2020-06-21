using CrossMod.Nodes;
using CrossMod.Rendering.GlTools;
using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public static class ViewportRenderer
    {
        public static void RenderNodes(IRenderable renderTexture, IEnumerable<IRenderable> renderableNodes, Camera camera, ScriptNode scriptNode)
        {
            // Ensure shaders are created before drawing anything.
            if (!ShaderContainer.HasSetUp)
                ShaderContainer.SetUpShaders();

            SetUpViewport();

            if (renderTexture != null)
            {
                renderTexture.Render(camera);
            }
            else if (renderableNodes != null)
            {
                foreach (var node in renderableNodes)
                    node.Render(camera);

                ParamNodeContainer.Render(camera);
                scriptNode?.Render(camera);
            }
        }

        private static void SetUpViewport()
        {
            DrawBackgroundClearBuffers();
            SetRenderState();
        }

        private static void DrawBackgroundClearBuffers()
        {
            // TODO: Clearing can be skipped if there is a background to draw.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);
        }

        private static void SetRenderState()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }
    }
}
