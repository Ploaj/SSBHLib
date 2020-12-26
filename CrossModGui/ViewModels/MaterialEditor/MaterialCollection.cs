using System.Collections.ObjectModel;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class MaterialCollection : ViewModelBase
    {
        public string Name { get; set; } = "";
        public ObservableCollection<Material> Materials { get; } = new ObservableCollection<Material>();
    }
}
