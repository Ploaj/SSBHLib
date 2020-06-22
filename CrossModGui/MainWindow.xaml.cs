using CrossMod.GUI;
using CrossMod.Nodes;
using CrossModGui.ViewModels;
using CrossModGui.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrossModGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }

        // TODO: Link view models to model classes.
        private readonly RenderSettingsWindowViewModel renderSettingsViewModel = new RenderSettingsWindowViewModel();
        private readonly CameraSettingsWindowViewModel cameraSettingsViewModel = new CameraSettingsWindowViewModel();

        public MainWindow(MainWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void RenderSettings_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Link render settings to the view model.
            var window = new RenderSettingsWindow(renderSettingsViewModel);
            window.Show();
        }

        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            var window = new CameraSettingsWindow(cameraSettingsViewModel);
            window.Show();
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (CrossMod.Tools.FileTools.TryOpenFolderDialog(out string folderPath))
            {
                // Populate the treeview with the folder structure.
                // TODO: Don't populate subnodes until expanding the directory node.
                var rootNode = new DirectoryNode(folderPath);
                rootNode.OpenRecursive();

                ViewModel.FileTreeItems.Clear();
                ViewModel.FileTreeItems.Add(rootNode);
            }
        }

        private void ClearWorkspace_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FileTreeItems.Clear();
        }
    }
}
