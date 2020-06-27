using System.Collections.ObjectModel;

namespace CrossModGui.ViewModels
{
    public class MaterialEditorWindowViewModel : ViewModelBase
    {
        public class BooleanParam : ViewModelBase
        {
            public string Name { get; set; }
            public bool Value { get; set; }
        }

        public class FloatParam : ViewModelBase
        {
            public string Name { get; set; }
            public float Value { get; set; }
        }

        public class Vec4Param : ViewModelBase
        {
            public string Name { get; set; }
            public float Value1 { get; set; }
            public float Value2 { get; set; }
            public float Value3 { get; set; }
            public float Value4 { get; set; }
        }

        public class TextureParam : ViewModelBase
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public ObservableCollection<string> MaterialNames { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> PossibleTextureNames { get; } = new ObservableCollection<string>();

        public ObservableCollection<BooleanParam> BooleanParams { get; } = new ObservableCollection<BooleanParam>();

        public ObservableCollection<FloatParam> FloatParams { get; } = new ObservableCollection<FloatParam>();

        public ObservableCollection<Vec4Param> Vec4Params { get; } = new ObservableCollection<Vec4Param>();

        public ObservableCollection<TextureParam> TextureParams { get; } = new ObservableCollection<TextureParam>();
    }
}
