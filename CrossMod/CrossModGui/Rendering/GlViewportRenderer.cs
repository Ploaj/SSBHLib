using CrossMod.Rendering;
using SFGraphics.Controls;

namespace CrossModGui.Rendering
{
    class GlViewportRenderer : ViewportRenderer
    {
        private readonly GLViewport viewport;

        public GlViewportRenderer(GLViewport viewport)
        {
            this.viewport = viewport;
        }

        public override int Width => viewport.Width;

        public override int Height => viewport.Height;

        public override void PauseRendering()
        {
            viewport.PauseRendering();
        }

        public override void RestartRendering()
        {
            viewport.RestartRendering();
        }

        public override void SwapBuffers()
        {
            viewport.SwapBuffers();
        }
    }
}
