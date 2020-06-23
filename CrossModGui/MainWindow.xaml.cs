using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossModGui.ViewModels;
using CrossModGui.Views;
using System;
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

        public ViewportRenderer Renderer { get; } 

        // TODO: Link view models to model classes.
        private readonly RenderSettingsWindowViewModel renderSettingsViewModel = new RenderSettingsWindowViewModel();
        private readonly CameraSettingsWindowViewModel cameraSettingsViewModel = new CameraSettingsWindowViewModel();

        public MainWindow(MainWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
            Renderer = new ViewportRenderer(glViewport);
        }

        private void GlViewport_OnRenderFrame(object sender, System.EventArgs e)
        {
            // TODO: Update camera from mouse.
            // TODO: Script node.
            Renderer.RenderNodes(null);
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
                // TODO: Populate subnodes after expanding the directory node.
                var rootNode = new DirectoryNode(folderPath);

                // TODO: Combine these two methods?
                rootNode.Open();
                rootNode.OpenChildNodes();

                ViewModel.FileTreeItems.Clear();
                ViewModel.FileTreeItems.Add(rootNode);
            }
        }

        private void ClearWorkspace_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Clear();
            Renderer.ClearRenderableNodes();
        }

        private void FileTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // TODO: Use some form of data binding?
            if (!(e.NewValue is FileNode item))
                return;

            // Assume only directory nodes have children.
            if (item.Parent is DirectoryNode dir)
            {
                dir.OpenChildNodes();
            }
            UpdateCurrentViewportRenderables(item);
        }

        private void UpdateCurrentViewportRenderables(FileNode item)
        {
            if (item is NutexNode texture)
            {
                Renderer.UpdateTexture(texture);
            }
            else if (item is IRenderableNode renderableNode)
            {
                Renderer.AddRenderableNode(item.AbsolutePath, renderableNode);
                // TODO: Update the mesh and bone list.
                Renderer.UpdateTexture(null);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            glViewport.OnRenderFrame += GlViewport_OnRenderFrame;
            glViewport.RestartRendering();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            glViewport.Dispose();
        }
    }
}
