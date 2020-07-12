using CrossMod.Tools;
using CrossModGui.ViewModels;
using System.Diagnostics;
using System.Windows;

namespace CrossModGui.Views
{
    /// <summary>
    /// Interaction logic for MaterialEditorWindow.xaml
    /// </summary>
    public partial class MaterialEditorWindow : Window
    {
        private readonly MaterialEditorWindowViewModel viewModel;
        public MaterialEditorWindow(MaterialEditorWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = this.viewModel;
            InitializeComponent();
        }

        private void ExportMatl_Click(object sender, RoutedEventArgs e)
        {
            if (FileTools.TryOpenSaveFileDialog(out string fileName, "Ultimate Material (*.numatb)|*.*", "model.numatb"))
                viewModel.SaveMatl(fileName);
        }

        private void MaterialReference_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/ScanMountGoat/Smush-Material-Research/blob/master/Material%20Parameters.md"));
        }
    }
}
