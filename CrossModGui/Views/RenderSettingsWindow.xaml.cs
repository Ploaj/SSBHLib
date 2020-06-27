using CrossModGui.ViewModels;
using System.Windows;

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
            this.viewModel = viewModel;
            DataContext = this.viewModel;
            InitializeComponent();
        }
    }
}
