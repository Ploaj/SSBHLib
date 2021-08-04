using CrossModGui.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var info = new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            };
            Process.Start(info);
            e.Handled = true;
        }
    }
}
