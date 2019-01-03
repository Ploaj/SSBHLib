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

                    // Set frame to 0 and ensure the animation always re-renders on swap.
                    // Value change events don't trigger if the value doesn't change.
                    if (Frame == 0)
                    {
                        UpdateAnimation();
                    }
                    else
                    {
                        Frame = 0;
                    }
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
        
        /**
         * Updates the held animation object to hold the current state.
         * This includes the model, skeleton, and current frame.
         */
        private void UpdateAnimation()
        {
            if (Animation == null)
            {
                return;
            }

            Animation.SetFrameModel(Model, Frame);
            Animation.SetFrameSkeleton(Skeleton, Frame);
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
            UpdateAnimation();
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
