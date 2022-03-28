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
using System.Linq;
using SSBHLib.Formats.Materials;
using CrossMod.Rendering.Models;

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
            windowViewModel.PropertyChanged += (s, e) =>
            {
                windowViewModel.SetValues(RenderSettings.Instance);
                RenderFrameIfNeeded();
            };

            var window = new RenderSettingsWindow(windowViewModel);
            window.Show();
        }

        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the window has current values.
            var windowViewModel = new CameraSettingsWindowViewModel(viewModel.Renderer.Camera);
            windowViewModel.PropertyChanged += (s, e) =>
            {
                windowViewModel.SetValues(viewModel.Renderer.Camera);
                viewModel.Renderer.UpdateCameraFromMouse();
                RenderFrameIfNeeded();
            };

            var window = new CameraSettingsWindow(windowViewModel);
            window.Show();
        }

        private void MaterialEditor_Click(object sender, RoutedEventArgs e)
        {
            // Trigger frame updates manually to avoid accessing the graphics context from multiple threads.
            // This also improves UI responsiveness.
            viewModel.IsPlayingAnimation = false;
            glViewport.PauseRendering();

            // TODO: Just search the file tree for matls?
            var vm = new MaterialEditorWindowViewModel(viewModel.FileTreeItems, viewModel.Renderer.ItemToRender as ModelCollection);
            vm.RenderFrameNeeded += (s, e) => RenderFrameIfNeeded();

            var window = new MaterialEditorWindow(vm);
            window.Show();
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (FileTools.TryOpenFolderDialog(out string folderPath))
            {
                viewModel.PopulateFileTree(folderPath, false, () => { });
            }
        }

        private void OpenFolderRecursive_Click(object sender, RoutedEventArgs e)
        {
            if (FileTools.TryOpenFolderDialog(out string folderPath))
            {
                // Render a frame on update to show progress when opening lots of model folders.
                viewModel.PopulateFileTree(folderPath, true, () => RenderFrameIfNeeded());
            }
        }

        private void ReloadFiles_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ReloadFiles();
        }

        private void ClearWorkspace_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Clear();
            viewModel.Renderer.Clear();
            // Make sure the viewport buffer clears.
            glViewport.RenderFrame();
        }

        private void FileTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // TODO: This can just be a property on the view model.
            if (!(e.NewValue is FileNode item))
                return;

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
            viewModel.ClearViewport();
            
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

        private void SaveScreenshot_Click(object sender, RoutedEventArgs e)
        {
            RenderFrameIfNeeded();
            using var bmp = viewModel.Renderer.GetScreenshot();
            if (FileTools.TryOpenSaveFileDialog(out string fileName, "PNG (*.png)|*.png*", "screenshot.png"))
            {
                bmp.Save(fileName);
            }
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
            {
                
                mainWindowGrid.ColumnDefinitions[0].Width = item.IsChecked ? new GridLength(400) : new GridLength(0);
                RenderFrameIfNeeded();
            }
        }

        private void MeshBoneTabMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                mainWindowGrid.ColumnDefinitions[4].Width = item.IsChecked ? new GridLength(400) : new GridLength(0);
                RenderFrameIfNeeded();
            }
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            var window = new PreferencesWindow(PreferencesWindowViewModel.Instance);
            window.Show();
        }
    }
}
