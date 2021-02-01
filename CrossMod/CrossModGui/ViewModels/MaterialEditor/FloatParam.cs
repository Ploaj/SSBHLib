using CrossModGui.Tools;
using SSBHLib.Formats.Materials;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class FloatParam : ViewModelBase
    {
        public string ParamId { get; }
        public string ParamIdDescription { get; }

        public float Min { get; } = 0.0f;
        public float Max { get; } = 1.0f;

        public float Value
        {
            get => (float)attribute.DataObject;
            set
            {
                attribute.DataObject = value;
                OnPropertyChanged();
            }
        }

        private readonly MatlAttribute attribute;

        public FloatParam(MatlAttribute attribute)
        {
            this.attribute = attribute;
            ParamId = attribute.ParamId.ToString();
            ParamIdDescription = MaterialParamDescriptions.Instance.GetDescriptionText(ParamId);
        }
    }
}
