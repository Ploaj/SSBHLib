using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrossModGui.UserControls
{
    /// <summary>
    /// Interaction logic for AnimationBar.xaml
    /// </summary>
    public partial class AnimationBar : UserControl
    {
        public static readonly DependencyProperty CurrentFrameProperty = DependencyProperty.Register(nameof(CurrentFrame),
            typeof(float), typeof(AnimationBar),
            new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty TotalFramesProperty = DependencyProperty.Register(nameof(TotalFrames),
            typeof(float), typeof(AnimationBar),
            new FrameworkPropertyMetadata(0f));

        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(nameof(IsPlaying),
            typeof(bool), typeof(AnimationBar),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsPlaying
        {
            get => (bool)GetValue(IsPlayingProperty);
            set => SetValue(IsPlayingProperty, value);
        }

        public float CurrentFrame
        {
            get => (float)GetValue(CurrentFrameProperty);
            set => SetValue(CurrentFrameProperty, value);
        }

        public float TotalFrames
        {
            // TODO: This isn't updating?
            get => (float)GetValue(TotalFramesProperty);
            set => SetValue(TotalFramesProperty, value);
        }

        public AnimationBar()
        {
            InitializeComponent();
        }

        private void NextFrame_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame++;
        }

        private void LastFrame_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame = TotalFrames;
        }

        private void PreviousFrame_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame--;
        }

        private void FirstFrame_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame = 0f;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = !IsPlaying;
        }
    }
}
