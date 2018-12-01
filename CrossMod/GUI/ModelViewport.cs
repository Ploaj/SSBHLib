using System;
using System.Windows.Forms;
using CrossMod.Rendering;

namespace CrossMod.GUI
{
    public partial class ModelViewport : UserControl
    {
        // Render-ables
        public IRenderable RenderableNode;

        public ModelViewport()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            RenderableNode = null;
        }
    }
}
