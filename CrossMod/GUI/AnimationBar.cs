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

        private Timer AnimationPlayer;

        public AnimationBar()
        {
            InitializeComponent();
            animationTrack.TickFrequency = 1;
            SetupTimer();
        }

        private void SetupTimer()
        {
            AnimationPlayer = new Timer();
            AnimationPlayer.Interval = 100 / 60;
            AnimationPlayer.Tick += new EventHandler(animationTimer_Tick);
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            if(animationTrack.Value == animationTrack.Maximum)
            {
                animationTrack.Value = 0;
            }
            else
            {
                animationTrack.Value++;
            }
        }

        private void animationTrack_ValueChanged(object sender, EventArgs e)
        {
            currentFrame.Value = animationTrack.Value;
            Animation.SetFrameModel(Model, Frame);
            Animation.SetFrameSkeleton(Skeleton, Frame);
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (playButton.Text.Equals(">"))
            {
                playButton.Text = "||";
                AnimationPlayer.Start();
            }
            else
            {
                playButton.Text = ">";
                AnimationPlayer.Stop();
            }
        }
    }
}
