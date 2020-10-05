namespace CrossModGui.ViewModels
{
    public partial class MaterialEditorWindowViewModel
    {
        public class BooleanParam : ViewModelBase
        {
            public string ParamId { get; set; }
            public bool Value { get; set; }
        }
    }
}
