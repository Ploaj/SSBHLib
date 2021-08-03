using CrossModGui.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for NewReleaseWindow.xaml
    /// </summary>
    public partial class NewReleaseWindow : Window
    {
        private readonly NewReleaseWindowViewModel viewModel;

        public NewReleaseWindow(NewReleaseWindowViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            DataContext = this.viewModel;
        }
    }
}
