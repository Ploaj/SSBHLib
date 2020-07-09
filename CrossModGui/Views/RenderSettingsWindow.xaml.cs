using CrossModGui.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CrossModGui.Views
{
    /// <summary>
    /// Interaction logic for RenderSettingsWindow.xaml
    /// </summary>
    public partial class RenderSettingsWindow : Window
    {
        private readonly RenderSettingsWindowViewModel viewModel;

        public RenderSettingsWindow(RenderSettingsWindowViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            DataContext = this.viewModel;
        }

        private void RenderMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: How to properly check the text?
            // TODO: Associate this info with the render mode?
            var selected = (sender as ComboBox)?.SelectedItem.ToString();

            var isParam = selected.ToLower().Contains("param");
            paramDebugLabel.Visibility = isParam ? Visibility.Visible : Visibility.Collapsed;
            paramDebugText.Visibility = isParam ? Visibility.Visible : Visibility.Collapsed;

            var hasChannels = !selected.ToLower().Contains("shaded");
            redCheckBox.Visibility = hasChannels ? Visibility.Visible : Visibility.Collapsed;
            greenCheckBox.Visibility = hasChannels ? Visibility.Visible : Visibility.Collapsed;
            blueCheckBox.Visibility = hasChannels ? Visibility.Visible : Visibility.Collapsed;
            alphaCheckBox.Visibility = hasChannels ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
