using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using CrossMod.Nodes;
using System;
using System.Windows.Forms;

namespace CrossMod.GUI
{
    /// <summary>
    /// A <see cref="Control"/> for playing RAnimation files with a skeleton.
    /// </summary>
    public partial class AnimationBar : UserControl
    {
        private static readonly string playingText = "||";
        private static readonly string stoppedText = ">";

        public int FrameCount
        {
            get => frameCount;
            protected set
            {
                //animationTrack.Maximum = value;
                frameCount = value;
                totalFrame.Maximum = value;
                currentFrame_UpDown.Maximum = value;
                totalFrame.Value = value;
            }
        }
        private int frameCount;

        public float Frame
        {
            get => (float)currentFrame_UpDown.Value;
            set => currentFrame_UpDown.Value = (decimal)value;
        }

        public float MotionRate
        {
            get
            {
                if (ScriptNode == null)
                    return 1;
                return ScriptNode.MotionRate;
            }
        }

        public RModel Model { get; set; }
        public RSkeleton Skeleton { get; set; }
        public ScriptNode ScriptNode { get; set; }

        /// <summary>
        /// Sets the current animation.
        /// Setting it to null stops playback, setting it when an animation
        /// is ongoing starts a new animation but from frame 0.
        /// </summary>
        public IRenderableAnimation Animation
        {
            get => animation;
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
                        currentFrame_UpDown.Value = 0;
                    }
                }
                UpdateScript();
            }
        }
        private IRenderableAnimation animation;

        private Timer animationPlayer;

        public bool IsPlaying => playButton.Text.Equals(playingText);

        public AnimationBar()
        {
            InitializeComponent();
            SetupTimer();
            currentFrame_UpDown.Increment = (decimal)MotionRate;
        }

        public void Start()
        {
            playButton.Text = playingText;
            animationPlayer.Start();
        }

        public void Stop()
        {
            playButton.Text = stoppedText;
            animationPlayer.Stop();
        }

        private void SetupTimer()
        {
            animationPlayer = new Timer
            {
                Interval = 100 / 60
            };
            animationPlayer.Tick += new EventHandler(animationTimer_Tick);
        }

        public void Clear()
        {
            Model = null;
            Skeleton = null;
            animation = null;
        }

        /**
         * Updates the held animation object to hold the current state.
         * This includes the model, skeleton, and current frame.
         */
        private void UpdateAnimation()
        {
            if (Animation == null)
                return;

            Animation.SetFrameModel(Model, Frame);
            Animation.SetFrameSkeleton(Skeleton, Frame);
        }

        private void UpdateScript()
        {
            if (ScriptNode == null)
                return;

            if (Frame == 0)
                ScriptNode.Start();
            ScriptNode.Update(Frame);

            currentFrame_UpDown.Increment = (decimal)MotionRate;
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            // Loop back to the beginning at the end.
            float nextFrame = Frame + MotionRate;

            if (nextFrame > FrameCount)
                currentFrame_UpDown.Value = 0;
            else
                currentFrame_UpDown.Value = (decimal)nextFrame;
        }

        private void currentFrame_ValueChanged(object sender, EventArgs e)
        {
            UpdateAnimation();
            UpdateScript();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (IsPlaying)
                Stop();
            else
                Start();
        }
    }
}
