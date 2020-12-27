using SSBHLib.Formats.Materials;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class Material : ViewModelBase
    {
        public string Name { get; set; } = "";

        public string ShaderLabel { get; set; } = "";

        public SolidColorBrush? MaterialIdColor { get; set; }

        public bool HasFloats => FloatParams.Count > 0;
        public bool HasBooleans => BooleanParams.Count > 0;
        public bool HasVec4Params => Vec4Params.Count > 0;
        public bool HasTextures => TextureParams.Count > 0;

        public MatlCullMode CullMode { get; set; }
        public MatlFillMode FillMode { get; set; }
        public MatlBlendFactor SourceColor { get; set; }
        public MatlBlendFactor DestinationColor { get; set; }

        public ObservableCollection<BooleanParam> BooleanParams { get; } = new ObservableCollection<BooleanParam>();

        public ObservableCollection<FloatParam> FloatParams { get; } = new ObservableCollection<FloatParam>();

        public ObservableCollection<Vec4Param> Vec4Params { get; } = new ObservableCollection<Vec4Param>();

        public ObservableCollection<TextureSamplerParam> TextureParams { get; } = new ObservableCollection<TextureSamplerParam>();
    }
}
