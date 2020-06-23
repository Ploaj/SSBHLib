using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossModGui.ViewModels;
using CrossModGui.Views;
using System;
using System.Windows;
using System.Windows.Input;

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
            InitializeComponent();

            ViewModel = viewModel;
            ViewModel.Renderer = new ViewportRenderer(glViewport);
            DataContext = viewModel;
        }

        private void GlViewport_OnRenderFrame(object sender, EventArgs e)
        {
            // TODO: Only call this on new input.
            // TODO: Update camera from mouse.
            // Accessing the control properties can't be done on another thread.
            glViewport.BeginInvoke(new Action(() =>
            {
                // TODO: This conversion is questionable.
                // Ignoring mouse input not focused can probably be handled more cleanly.
                var point = PointToScreen(Mouse.GetPosition(this));
                ViewModel.Renderer.UpdateCameraFromMouseKeyboard(new System.Drawing.Point((int)point.X, (int)point.Y));
            }));

            // TODO: Script node.
            ViewModel.Renderer.RenderNodes(null);
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
                ViewModel.PopulateFileTree(folderPath);
            }
        }

        private void ClearWorkspace_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Clear();
            ViewModel.Renderer.ClearRenderableNodes();
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

            ViewModel.UpdateCurrentRenderableNode(item);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            glViewport.OnRenderFrame += GlViewport_OnRenderFrame;
            glViewport.RestartRendering();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            glViewport.Dispose();
        }

        private void glViewport_Resize(object sender, EventArgs e)
        {
            ViewModel.Renderer.Camera.RenderWidth = glViewport.Width;
            ViewModel.Renderer.Camera.RenderHeight = glViewport.Height;
        }
    }
}
