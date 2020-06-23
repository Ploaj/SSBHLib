using CrossModGui.ViewModels;
using System.Windows;

namespace CrossModGui.Views
{
    /// <summary>
    /// Interaction logic for CameraSettingsWindow.xaml
    /// </summary>
    public partial class CameraSettingsWindow : Window
    {
        public CameraSettingsWindowViewModel ViewModel { get; }

        public CameraSettingsWindow(CameraSettingsWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
