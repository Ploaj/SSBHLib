using System.Windows.Media;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class TextureParam : ViewModelBase
    {
        public string ParamId { get; set; }
        public string Value { get; set; }
        public ImageSource Image { get; set; }
    }
}
