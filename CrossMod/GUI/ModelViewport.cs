using System;
using System.Windows.Forms;
using CrossMod.Rendering;

namespace CrossMod.GUI
{
    public partial class ModelViewport : UserControl
    {
        // Render-ables
        public IRenderable RenderableNode = null;

        public ModelViewport()
        {
            InitializeComponent();

            glViewport.OnRenderFrame += GlViewport_OnRenderFrame;
        }

        public void RenderFrame()
        {
            glViewport.RenderFrame();
        }

        private void GlViewport_OnRenderFrame(object sender, EventArgs e)
        {
            RenderableNode?.Render();
        }

        public void Clear()
        {
            RenderableNode = null;
        }
    }
}
