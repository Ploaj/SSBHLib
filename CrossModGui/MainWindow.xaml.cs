using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Tools;
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
            renderSettingsViewModel.PropertyChanged += (s, e) => RenderSettings.Instance.SetValues(renderSettingsViewModel);

            cameraSettingsViewModel = ViewModel.Renderer.Camera;
            cameraSettingsViewModel.PropertyChanged += CameraSettingsViewModel_PropertyChanged;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Ensure animations update the viewport.
            if (e.PropertyName == nameof(MainWindowViewModel.IsPlayingAnimation))
            {
                if (ViewModel.IsPlayingAnimation)
                    glViewport.RestartRendering();
                else
                    glViewport.PauseRendering();
            }
            if (e.PropertyName == nameof(MainWindowViewModel.CurrentFrame))
            {
                glViewport.RenderFrame();
            }
        }

        private void CameraSettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // TODO: Do this with less code using reactiveui?
            switch (e.PropertyName)
            {
                case nameof(CameraSettingsWindowViewModel.RotationXDegrees):
                    ViewModel.Renderer.Camera.RotationXDegrees = cameraSettingsViewModel.RotationXDegrees;
                    break;
                case nameof(CameraSettingsWindowViewModel.RotationYDegrees):
                    ViewModel.Renderer.Camera.RotationYDegrees = cameraSettingsViewModel.RotationYDegrees;
                    break;
                case nameof(CameraSettingsWindowViewModel.PositionX):
                    ViewModel.Renderer.Camera.TranslationX = cameraSettingsViewModel.PositionX;
                    break;
                case nameof(CameraSettingsWindowViewModel.PositionY):
                    ViewModel.Renderer.Camera.TranslationY = cameraSettingsViewModel.PositionY;
                    break;
                case nameof(CameraSettingsWindowViewModel.PositionZ):
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
            // Start automatic frame updates instead of making the window have to refresh the viewport.
            var wasRendering = glViewport.IsRendering;
            glViewport.RestartRendering();

            var window = new RenderSettingsWindow(renderSettingsViewModel);
            window.Show();

            window.Closed += (s, e2) =>
            {
                if (!wasRendering)
                    glViewport.PauseRendering();
            };
        }

        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            // Start automatic frame updates instead of making the window have to refresh the viewport.
            var wasRendering = glViewport.IsRendering;
            glViewport.RestartRendering();

            // Make sure the window has current values.
            cameraSettingsViewModel.SetValues(ViewModel.Renderer.Camera);

            var window = new CameraSettingsWindow(cameraSettingsViewModel);
            window.Show();

            window.Closed += (s, e2) =>
            {
                if (!wasRendering)
                    glViewport.PauseRendering();
            };
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

            // Update the current viewport item.
            ViewModel.UpdateCurrentRenderableNode(item);
            RenderFrameIfNeeded();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Don't start rendering here to save CPU usage.
            glViewport.OnRenderFrame += GlViewport_OnRenderFrame;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            glViewport.Dispose();
        }

        private void glViewport_Resize(object sender, EventArgs e)
        {
            ViewModel.Renderer.Camera.RenderWidth = glViewport.Width;
            ViewModel.Renderer.Camera.RenderHeight = glViewport.Height;

            RenderFrameIfNeeded();
        }

        private void glViewport_MouseInteract(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ViewModel.Renderer.UpdateCameraFromMouse();
            cameraSettingsViewModel.SetValues(ViewModel.Renderer.Camera);

            RenderFrameIfNeeded();
        }

        private void FrameModel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Renderer.FrameRenderableModels();
            cameraSettingsViewModel.SetValues(ViewModel.Renderer.Camera);

            RenderFrameIfNeeded();
        }

        private void ClearViewport_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Renderer.ClearRenderableNodes();
            RenderFrameIfNeeded();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsPlayingAnimation = !ViewModel.IsPlayingAnimation;
        }

        private void RenderFrameIfNeeded()
        {
            if (!glViewport.IsRendering)
                glViewport.RenderFrame();
        }

        private void MeshListCheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Ensure mesh visibility is updated.
            RenderFrameIfNeeded();
        }

        private void ReloadShaders_Click(object sender, RoutedEventArgs e)
        {
            // Force the shaders to be generated again.
            ViewModel.Renderer.ReloadShaders();
        }

        private void ReloadScripts_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
        }

        private void BatchRenderModels_Click(object sender, RoutedEventArgs e)
        {
            BatchRendering.RenderModels(ViewModel.Renderer);
        }

        private void NextFrame_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentFrame++;
        }

        private void LastFrame_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentFrame = ViewModel.TotalFrames;
        }

        private void PreviousFrame_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentFrame--;
        }

        private void FirstFrame_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentFrame = 0f;
        }
    }
}
