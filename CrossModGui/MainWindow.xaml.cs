using CrossMod.IO;
using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossModGui.ViewModels;
using CrossModGui.Views;
using System;
using System.Windows;

namespace CrossModGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }

        private readonly RenderSettingsWindowViewModel renderSettingsViewModel;
        private readonly CameraSettingsWindowViewModel cameraSettingsViewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
            ViewModel.Renderer = new ViewportRenderer(glViewport);
            DataContext = viewModel;

            // Link view models to models.
            renderSettingsViewModel = RenderSettings.Instance;
            renderSettingsViewModel.PropertyChanged += (s,e) => RenderSettings.Instance.SetValues(renderSettingsViewModel);

            // TODO: Sync mouse movement with viewmodel?
            cameraSettingsViewModel = ViewModel.Renderer.Camera;
            cameraSettingsViewModel.PropertyChanged += CameraSettingsViewModel_PropertyChanged;
        }

        private void CameraSettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // TODO: Do this with less code using reactiveui?
            switch (e.PropertyName)
            {
                case "RotationXDegrees":
                    ViewModel.Renderer.Camera.RotationXDegrees = cameraSettingsViewModel.RotationXDegrees;
                    break;
                case "RotationYDegrees":
                    ViewModel.Renderer.Camera.RotationYDegrees = cameraSettingsViewModel.RotationYDegrees;
                    break;
                case "PositionX":
                    ViewModel.Renderer.Camera.TranslationX = cameraSettingsViewModel.PositionX;
                    break;
                case "PositionY":
                    ViewModel.Renderer.Camera.TranslationY = cameraSettingsViewModel.PositionY;
                    break;
                case "PositionZ":
                    ViewModel.Renderer.Camera.TranslationZ = cameraSettingsViewModel.PositionZ;
                    break;
            }
        }

        private void GlViewport_OnRenderFrame(object sender, EventArgs e)
        {
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
            // Make sure the window has current values.
            cameraSettingsViewModel.SetValues(ViewModel.Renderer.Camera);
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
            if (!(e.NewValue is FileNode item))
                return;

            // TODO: Use some form of binding to the directory's expanded event?
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

        private void glViewport_MouseInteract(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ViewModel.Renderer.UpdateCameraFromMouse();
            cameraSettingsViewModel.SetValues(ViewModel.Renderer.Camera);
        }

        private void FrameModel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Renderer.FrameRnumdl();
            cameraSettingsViewModel.SetValues(ViewModel.Renderer.Camera);
        }

        private void ClearViewport_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Renderer.ClearRenderableNodes();
        }
    }
}
