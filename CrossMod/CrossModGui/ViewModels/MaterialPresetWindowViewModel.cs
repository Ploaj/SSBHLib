using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CrossModGui.ViewModels
{
    public class MaterialPresetWindowViewModel : ViewModelBase
    {
        public ObservableCollection<MaterialPreset> Presets { get; }

        public MaterialPreset? SelectedPreset { get; set; }

        public event EventHandler<MaterialPreset?>? PresetApplying;

        public MaterialPresetWindowViewModel()
        {
            // Use a single XML file to simplify storing/loading materials. 
            // The material label won't be used, so treat it as an identifier for the preset.
            Presets = new ObservableCollection<MaterialPreset>(MaterialPresets.MaterialPresets.Presets.Value.Entries
                .Select(e => new MaterialPreset { Name = e.MaterialLabel }));
        }

        public void OnPresetApply()
        {
            PresetApplying?.Invoke(this, SelectedPreset);
        }
    }
}
