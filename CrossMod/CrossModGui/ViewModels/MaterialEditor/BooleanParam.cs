using CrossModGui.Tools;
using SSBHLib.Formats.Materials;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class BooleanParam : ViewModelBase
    {
        public string ParamId { get; }
        public bool Value
        {
            get => (bool)attribute.DataObject;
            set
            {
                attribute.DataObject = value;
                OnPropertyChanged();
            }
        }

        private readonly MatlAttribute attribute;

        public BooleanParam(MatlAttribute attribute)
        {
            this.attribute = attribute;
            ParamId = MaterialParamDescriptions.Instance.GetDescriptionText(attribute.ParamId.ToString());
            Value = (bool)attribute.DataObject;
        }
    }
}
