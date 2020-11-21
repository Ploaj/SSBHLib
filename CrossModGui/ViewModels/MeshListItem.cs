namespace CrossModGui.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class MeshListItem : ViewModelBase
        {
            public string Name { get; set; } = "";

            public bool IsChecked { get; set; }
        }
    }
}
