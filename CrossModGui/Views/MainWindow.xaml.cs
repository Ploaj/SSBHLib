using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Tools;
using CrossModGui.ViewModels;
using System;
using System.Windows;

namespace CrossModGui.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;

        private readonly RenderSettingsWindowViewModel renderSettingsViewModel;
        private readonly CameraSettingsWindowViewModel cameraSettingsViewModel;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel = new MainWindowViewModel(new ViewportRenderer(glViewport));
            DataContext = viewModel;

            // Link view models to models.
            renderSettingsViewModel = RenderSettings.Instance;
            renderSettingsViewModel.PropertyChanged += (s, e) => RenderSettings.Instance.SetValues(renderSettingsViewModel);

            cameraSettingsViewModel = viewModel.Renderer.Camera;
            cameraSettingsViewModel.PropertyChanged += CameraSettingsViewModel_PropertyChanged;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Ensure animations update the viewport.
            if (e.PropertyName == nameof(MainWindowViewModel.IsPlayingAnimation))
            {
                if (viewModel.IsPlayingAnimation)
                    glViewport.RestartRendering();
                else
                    glViewport.PauseRendering();
            }
            if (e.PropertyName == nameof(MainWindowViewModel.CurrentFrame))
            {
                // Only refresh the view if animations aren't playing.
                // Animation playback already updates the current frame.
                RenderFrameIfNeeded();
            }
        }

        private void CameraSettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CameraSettingsWindowViewModel.RotationXDegrees):
                    viewModel.Renderer.Camera.RotationXDegrees = cameraSettingsViewModel.RotationXDegrees;
                    break;
                case nameof(CameraSettingsWindowViewModel.RotationYDegrees):
                    viewModel.Renderer.Camera.RotationYDegrees = cameraSettingsViewModel.RotationYDegrees;
                    break;
                case nameof(CameraSettingsWindowViewModel.PositionX):
                    viewModel.Renderer.Camera.TranslationX = cameraSettingsViewModel.PositionX;
                    break;
                case nameof(CameraSettingsWindowViewModel.PositionY):
                    viewModel.Renderer.Camera.TranslationY = cameraSettingsViewModel.PositionY;
                    break;
                case nameof(CameraSettingsWindowViewModel.PositionZ):
                    viewModel.Renderer.Camera.TranslationZ = cameraSettingsViewModel.PositionZ;
                    break;
            }
        }

        private void GlViewport_OnRenderFrame(object sender, EventArgs e)
        {
            // TODO: Script node.
            viewModel.RenderNodes();
        }

        private void RenderSettings_Click(object sender, RoutedEventArgs e)
        {
            DisplayEditorWindow(new RenderSettingsWindow(renderSettingsViewModel));
        }

        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the window has current values.
            cameraSettingsViewModel.SetValues(viewModel.Renderer.Camera);
            DisplayEditorWindow(new CameraSettingsWindow(cameraSettingsViewModel));
        }

        private void MaterialEditor_Click(object sender, RoutedEventArgs e)
        {
            DisplayEditorWindow(new MaterialEditorWindow(new MaterialEditorWindowViewModel(viewModel.RNumdl)));
        }

        private void DisplayEditorWindow(Window window)
        {
            // Start automatic frame updates instead of making the window have to refresh the viewport.
            var wasRendering = glViewport.IsRendering;
            glViewport.RestartRendering();

            window.Show();

            window.Closed += (s, e2) =>
            {
                if (!wasRendering)
                    glViewport.PauseRendering();
            };
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (FileTools.TryOpenFolderDialog(out string folderPath))
            {
                viewModel.PopulateFileTree(folderPath);
            }
        }

        private void ClearWorkspace_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Clear();
            viewModel.Renderer.ClearRenderableNodes();
            // Make sure the viewport buffer clears.
            glViewport.RenderFrame();
        }

        private void FileTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(e.NewValue is FileNode item))
                return;

            // Open all files in the folder when the folder is selected.
            // TODO: This could be moved to the expanded event instead.
            if (item.Parent is DirectoryNode dir)
            {
                dir.OpenFileNodes();
            }

            // Update the current viewport item.
            viewModel.UpdateCurrentRenderableNode(item);
            RenderFrameIfNeeded();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Don't start rendering here to save CPU usage.
            glViewport.OnRenderFrame += GlViewport_OnRenderFrame;
            // Render twice to ensure the background is updated.
            glViewport.RenderFrame();
            glViewport.RenderFrame();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            glViewport.Dispose();
        }

        private void glViewport_Resize(object sender, EventArgs e)
        {
            viewModel.Renderer.Camera.RenderWidth = glViewport.Width;
            viewModel.Renderer.Camera.RenderHeight = glViewport.Height;

            RenderFrameIfNeeded();
        }

        private void glViewport_MouseInteract(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            viewModel.Renderer.UpdateCameraFromMouse();
            cameraSettingsViewModel.SetValues(viewModel.Renderer.Camera);

            RenderFrameIfNeeded();
        }

        private void FrameModel_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Renderer.FrameRenderableModels();
            cameraSettingsViewModel.SetValues(viewModel.Renderer.Camera);

            RenderFrameIfNeeded();
        }

        private void ClearViewport_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Renderer.ClearRenderableNodes();
            RenderFrameIfNeeded();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsPlayingAnimation = !viewModel.IsPlayingAnimation;
            if (viewModel.IsPlayingAnimation)
                viewModel.Renderer.RestartRendering();
            else
                viewModel.Renderer.PauseRendering();
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
            viewModel.Renderer.ReloadShaders();
        }

        private void ReloadScripts_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
        }

        private void BatchRenderModels_Click(object sender, RoutedEventArgs e)
        {
            BatchRendering.RenderModels(viewModel.Renderer);
        }

        private void NextFrame_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CurrentFrame++;
        }

        private void LastFrame_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CurrentFrame = viewModel.TotalFrames;
        }

        private void PreviousFrame_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CurrentFrame--;
        }

        private void FirstFrame_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CurrentFrame = 0f;
        }

        private void glViewport_MouseEnter(object sender, EventArgs e)
        {
            // Workaround for mouse scroll state not being updated
            // while the mouse pointer isn't on the viewport.
            viewModel.Renderer.UpdateMouseScroll();
        }
    }
}
