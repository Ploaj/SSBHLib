using SSBHLib.Formats.Materials;
using System.Collections.ObjectModel;

namespace CrossModGui.ViewModels.MaterialEditor
{
    /// <summary>
    /// A wrapper class for editing and displaying a <see cref="SSBHLib.Formats.Materials.Matl"/>.
    /// </summary>
    public class MaterialCollection : ViewModelBase
    {
        public string Name { get; }
        public ObservableCollection<Material> Materials { get; } = new ObservableCollection<Material>();

        public Matl Matl { get; }

        public MaterialCollection(string name, Matl matl)
        {
            Name = name;
            Matl = matl;
        }
    }
}
