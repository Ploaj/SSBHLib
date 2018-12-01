using System;
using System.Windows.Forms;
using CrossMod.Rendering;

namespace CrossMod.GUI
{
    public partial class ModelViewport : UserControl
    {
        // Render-ables
        public RSkeleton RenderSkeleton;

        public ModelViewport()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            RenderSkeleton = null;
        }
    }
}
