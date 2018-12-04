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
        public int FrameCount
        {
            get
            {
                return animationTrack.Maximum;
            }
            set
            {
                animationTrack.Maximum = value;
                totalFrame.Maximum = value;
                currentFrame.Maximum = value;
                totalFrame.Value = value;
            }
        }

        public int Frame
        {
            get
            {
                return animationTrack.Value;
            }
            set
            {
                animationTrack.Value = value;
            }
        }

        public RModel Model;
        public RSkeleton Skeleton;
        public IRenderableAnimation Animation;

        public AnimationBar()
        {
            InitializeComponent();
            animationTrack.TickFrequency = 1;
        }

        private void animationTrack_ValueChanged(object sender, EventArgs e)
        {
            currentFrame.Value = animationTrack.Value;
            Animation.SetFrameModel(Model, Frame);
            Animation.SetFrameSkeleton(Skeleton, Frame);
        }

        private void playButton_Click(object sender, EventArgs e)
        {

        }
    }
}
