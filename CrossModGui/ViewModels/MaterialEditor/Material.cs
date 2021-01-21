using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class Material : ViewModelBase
    {
        public string Name { get; set; } = "";

        public string ShaderLabel { get; set; } = "";

        public SolidColorBrush? MaterialIdColor { get; set; }

        public RasterizerStateParam? RasterizerState0 { get; set; }
        public BlendStateParam? BlendState0 { get; set; }

        public ObservableCollection<BooleanParam> BooleanParams { get; } = new ObservableCollection<BooleanParam>();

        public ObservableCollection<FloatParam> FloatParams { get; } = new ObservableCollection<FloatParam>();

        public ObservableCollection<Vec4Param> Vec4Params { get; } = new ObservableCollection<Vec4Param>();

        public ObservableCollection<TextureSamplerParam> TextureParams { get; } = new ObservableCollection<TextureSamplerParam>();
    }
}
