﻿using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CrossModGui.ViewModels
{
    public partial class MaterialEditorWindowViewModel
    {
        public class Material : ViewModelBase
        {
            public string Name { get; set; }

            public string ShaderLabel { get; set; }

            public SolidColorBrush MaterialIdColor { get; set; }

            public bool HasFloats => FloatParams.Count > 0;
            public bool HasBooleans => BooleanParams.Count > 0;
            public bool HasVec4Params => Vec4Params.Count > 0;
            public bool HasTextures => TextureParams.Count > 0;

            public ObservableCollection<BooleanParam> BooleanParams { get; } = new ObservableCollection<BooleanParam>();

            public ObservableCollection<FloatParam> FloatParams { get; } = new ObservableCollection<FloatParam>();

            public ObservableCollection<Vec4Param> Vec4Params { get; } = new ObservableCollection<Vec4Param>();

            public ObservableCollection<TextureParam> TextureParams { get; } = new ObservableCollection<TextureParam>();
        }
    }
}
