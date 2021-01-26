using System.Collections.ObjectModel;

namespace CrossModGui.ViewModels
{
    public class MaterialPresetWindowViewModel : ViewModelBase
    {
        public ObservableCollection<MaterialPreset> Presets { get; } = new ObservableCollection<MaterialPreset>();

        public MaterialPreset? SelectedPreset { get; set; }
    }
}
