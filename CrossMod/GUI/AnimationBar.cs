using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrossMod.Rendering;

// this tool is for playing RAnimation files with skeleton

namespace CrossMod.GUI
{
    public partial class AnimationBar : UserControl
    {
        public RModel Model;
        public RSkeleton Skeleton;
        public IRenderableAnimation Animation;

        public AnimationBar()
        {
            InitializeComponent();
        }

        private void animationTrack_ValueChanged(object sender, EventArgs e)
        {

        }

        private void playButton_Click(object sender, EventArgs e)
        {

        }
    }
}
