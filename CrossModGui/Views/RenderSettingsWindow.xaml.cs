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
            materialParamDebugGroupBox.IsEnabled = (selected.ToLower().Contains("param"));
            channelTogglesGroupBox.IsEnabled = !(selected.ToLower().Contains("shaded"));
        }
    }
}
