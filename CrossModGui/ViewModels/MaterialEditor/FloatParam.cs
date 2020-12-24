namespace CrossModGui.ViewModels.MaterialEditor
{
    public class FloatParam : ViewModelBase
    {
        public string ParamId { get; set; } = "";
        public float Value { get; set; }
        public float Min { get; set; } = 0.0f;
        public float Max { get; set; } = 1.0f;
    }
}
