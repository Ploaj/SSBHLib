using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CrossModGui.UserControls
{
    /// <summary>
    /// Interaction logic for EnumEditor.xaml
    /// </summary>
    public partial class EnumEditor : UserControl
    {
        public static readonly DependencyProperty DescriptionByValueProperty = DependencyProperty.Register(nameof(DescriptionByValue),
            typeof(IDictionary), typeof(EnumEditor),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(nameof(SelectedValue),
            typeof(object), typeof(EnumEditor),
            new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label),
            typeof(string), typeof(EnumEditor),
            new FrameworkPropertyMetadata());

        public Dictionary<object, string> DescriptionByValue { get; set; } = new Dictionary<object, string>();

        public object? SelectedValue { get; set; }

        public string Label { get; set; } = "";

        public EnumEditor()
        {
            InitializeComponent();
        }
    }
}
