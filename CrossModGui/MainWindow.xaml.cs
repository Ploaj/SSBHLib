using CrossMod.Nodes;
using CrossModGui.ViewModels;
using CrossModGui.Views;
using System.Collections.Generic;
using System.Windows;

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

        private void FileTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // TODO: Use some form of data binding.
            if (!(e.NewValue is FileNode item))
                return;

            ViewModel.SelectedFileNode = item;
        }
    }
}
