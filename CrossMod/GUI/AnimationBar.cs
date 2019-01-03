using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using System;
using System.Windows.Forms;

namespace CrossMod.GUI
{
    // this tool is for playing RAnimation files with skeleton
    public partial class AnimationBar : UserControl
    {
        public int FrameCount
        {
            get
            {
                return animationTrack.Maximum;
            }
            protected set
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

        private IRenderableAnimation animation;

        /**
         * Sets the current animation.
         * Setting it to null stops playback, setting it when an animation
         * is ongoing starts a new animation but from frame 0.
         */
        public IRenderableAnimation Animation
        {
            get
            {
                return animation;
            }
            set
            {
                if (value == null)
                {
                    Stop();
                    animation = null;
                }
                else if (animation != value)
                {
                    animation = value;
                    FrameCount = value.GetFrameCount();
                    Frame = 0;
                }
            }
        }

        private Timer AnimationPlayer;

        public AnimationBar()
        {
            InitializeComponent();
            animationTrack.TickFrequency = 1;
            SetupTimer();
        }

        public void Start()
        {
            playButton.Text = "||";
            AnimationPlayer.Start();
        }

        public void Stop()
        {
            playButton.Text = ">";
            AnimationPlayer.Stop();
        }

        private void SetupTimer()
        {
            AnimationPlayer = new Timer
            {
                Interval = 100 / 60
            };
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
                Start();
            }
            else
            {
                Stop();
            }
        }
    }
}
