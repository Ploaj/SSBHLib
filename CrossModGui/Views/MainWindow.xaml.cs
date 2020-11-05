using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Tools;
using CrossModGui.Rendering;
using CrossModGui.Tools;
using CrossModGui.ViewModels;
using CrossModGui.ViewModels.MaterialEditor;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CrossModGui.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel(new GlViewportRenderer(glViewport));
            DataContext = viewModel;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;

            glViewport.HandleCreated += GlViewport_HandleCreated;
        }

        private void GlViewport_HandleCreated(object? sender, EventArgs e)
        {
            // The context is created after the handle is created,
            // so do any setup here before rendering starts.
            glViewport.FrameRendering += GlViewport_OnRenderFrame;

            CrossMod.Rendering.Resources.DefaultTextures.Initialize();
            CrossMod.Rendering.GlTools.ShaderContainer.SetUpShaders();
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

        private void GlViewport_OnRenderFrame(object? sender, EventArgs e)
        {
            viewModel.RenderNodes();
        }

        private void RenderSettings_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = new RenderSettingsWindowViewModel(RenderSettings.Instance);
            windowViewModel.PropertyChanged += (s, e) => windowViewModel.SetValues(RenderSettings.Instance);

            DisplayWindowWithRealTimeViewportUpdates(new RenderSettingsWindow(windowViewModel));
        }

        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the window has current values.
            var windowViewModel = new CameraSettingsWindowViewModel(viewModel.Renderer.Camera);
            windowViewModel.PropertyChanged += (s, e) =>
            {
                windowViewModel.SetValues(viewModel.Renderer.Camera);
                viewModel.Renderer.UpdateCameraFromMouse();
            };

            DisplayWindowWithRealTimeViewportUpdates(new CameraSettingsWindow(windowViewModel));
        }

        private void MaterialEditor_Click(object sender, RoutedEventArgs e)
        {
            DisplayWindowWithRealTimeViewportUpdates(new MaterialEditorWindow(new MaterialEditorWindowViewModel(viewModel.Rnumdl?.Material, viewModel.Rnumdl?.TextureByName.Keys)));
        }

        private void DisplayWindowWithRealTimeViewportUpdates(Window window)
        {
            // Start automatic frame updates instead of making the window have to refresh the viewport.
            var wasRendering = glViewport.IsRendering;
            glViewport.RestartRendering();

            window.Show();

            window.Closed += (s, e2) =>
            {
                // The main window may close first, so make sure the viewport still exists.
                if (!glViewport.IsDisposed && !wasRendering)
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

        private void Window_Closed(object sender, EventArgs e)
        {
            glViewport.Dispose();

            // Ensure the changes are preserved between sessions.
            PreferencesWindowViewModel.Instance.SaveChangesToFile();
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
            RenderFrameIfNeeded();
        }

        private void FrameModel_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Renderer.FrameRenderableModels();
            RenderFrameIfNeeded();
        }

        private void ClearViewport_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Renderer.ClearRenderableNodes();
            RenderFrameIfNeeded();
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

        private void BatchRenderModels_Click(object sender, RoutedEventArgs e)
        {
            if (!FileTools.TryOpenFolderDialog(out string folderPath, "Select Source Directory"))
                return;

            if (!FileTools.TryOpenFolderDialog(out string outputPath, "Select PNG Output Directory"))
                return;

            BatchRendering.RenderModels(folderPath, outputPath, viewModel.Renderer);
        }

        private void glViewport_MouseEnter(object sender, EventArgs e)
        {
            // Workaround for mouse scroll state not being updated while the mouse pointer isn't on the viewport.
            viewModel.Renderer.UpdateMouseScroll();
        }

        private void FileTreeMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
                fileTreeView.Visibility = item.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MeshBoneTabMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
                meshBoneTabControl.Visibility = item.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            var window = new PreferencesWindow(PreferencesWindowViewModel.Instance);
            window.Show();
        }
    }
}
