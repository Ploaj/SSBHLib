using SSBHLib.Formats.Materials;
using System.Windows.Media;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class TextureParam : ViewModelBase
    {
        public string ParamId { get; set; }
        public string SamplerParamId { get; set; }
        public string Value { get; set; }
        public ImageSource Image { get; set; }
        public MatlWrapMode WrapS { get; set; }
        public MatlWrapMode WrapT { get; set; }
        public MatlWrapMode WrapR { get; set; }
        public MatlMinFilter MinFilter { get; set; }
        public MatlMagFilter MagFilter { get; set; }
        public float LodBias { get; set; }
        public int MaxAnisotropy { get; set; }
    }
}
