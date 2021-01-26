namespace CrossModGui.ViewModels
{
    public class MaterialPreset : ViewModelBase
    {
        public string Name { get; set; }
        // TODO: Give each preset a unique thumbnail.
        public string ImageKey => "material";

    }
}