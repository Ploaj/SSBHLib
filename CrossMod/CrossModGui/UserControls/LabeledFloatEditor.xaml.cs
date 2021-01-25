using System.Windows;
using System.Windows.Controls;

namespace CrossModGui.UserControls
{
    /// <summary>
    /// Interaction logic for LabeledFloatEditor.xaml
    /// </summary>
    public partial class LabeledFloatEditor : UserControl
    {
        public static DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(float), typeof(LabeledFloatEditor),
            new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static DependencyProperty MinProperty = DependencyProperty.Register(nameof(Min), typeof(float), typeof(LabeledFloatEditor),
            new FrameworkPropertyMetadata(0f));

        public static DependencyProperty MaxProperty = DependencyProperty.Register(nameof(Max), typeof(float), typeof(LabeledFloatEditor),
            new FrameworkPropertyMetadata(1f));

        public static DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(LabeledFloatEditor),
            new FrameworkPropertyMetadata(""));

        public static DependencyProperty LabelWidthProperty = DependencyProperty.Register(nameof(LabelWidth), typeof(double), typeof(LabeledFloatEditor),
            new FrameworkPropertyMetadata(120.0));

        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public float Min
        {
            get => (float)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public float Max
        {
            get => (float)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public double LabelWidth
        {
            get => (double)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public LabeledFloatEditor()
        {
            InitializeComponent();
        }
    }
}
