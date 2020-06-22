using CrossModGui.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CrossModGui.Views
{
    /// <summary>
    /// Interaction logic for RenderSettingsWindow.xaml
    /// </summary>
    public partial class RenderSettingsWindow : Window
    {
        public RenderSettingsWindowViewModel ViewModel { get; }

        public RenderSettingsWindow(RenderSettingsWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
