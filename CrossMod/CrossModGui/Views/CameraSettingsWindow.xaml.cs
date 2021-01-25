using CrossModGui.ViewModels;
using System.Windows;

namespace CrossModGui.Views
{
    /// <summary>
    /// Interaction logic for CameraSettingsWindow.xaml
    /// </summary>
    public partial class CameraSettingsWindow : Window
    {
        private readonly CameraSettingsWindowViewModel viewModel;

        public CameraSettingsWindow(CameraSettingsWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = this.viewModel;
            InitializeComponent();
        }
    }
}
