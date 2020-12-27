using SSBHLib.Formats.Materials;
using System;
using System.Windows.Media;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class Vec4Param : ViewModelBase
    {

        public string ParamId { get; }
        public Brush? ColorBrush { get; set; }

        public string Label1 { get; set; } = "X";
        public float Min1 { get; set; } = 0.0f;
        public float Max1 { get; set; } = 1.0f;
        public float Value1
        {
            get => ((MatlAttribute.MatlVector4)attribute.DataObject).X;
            set
            {
                ((MatlAttribute.MatlVector4)attribute.DataObject).X = value;
                UpdateColor();
                OnPropertyChanged();
            }
        }

        public string Label2 { get; set; } = "Y";
        public float Min2 { get; set; } = 0.0f;
        public float Max2 { get; set; } = 1.0f;
        public float Value2
        {
            get => ((MatlAttribute.MatlVector4)attribute.DataObject).Y;
            set
            {
                ((MatlAttribute.MatlVector4)attribute.DataObject).Y = value;
                UpdateColor();
                OnPropertyChanged();
            }
        }

        public string Label3 { get; set; } = "Z";
        public float Min3 { get; set; } = 0.0f;
        public float Max3 { get; set; } = 1.0f;
        public float Value3
        {
            get => ((MatlAttribute.MatlVector4)attribute.DataObject).Z;
            set
            {
                ((MatlAttribute.MatlVector4)attribute.DataObject).Z = value;
                UpdateColor();
                OnPropertyChanged();
            }
        }

        public string Label4 { get; set; } = "W";
        public float Min4 { get; set; } = 0.0f;
        public float Max4 { get; set; } = 1.0f;
        public float Value4
        {
            get => ((MatlAttribute.MatlVector4)attribute.DataObject).W;
            set
            {
                ((MatlAttribute.MatlVector4)attribute.DataObject).W = value;
                UpdateColor();
                OnPropertyChanged();
            }
        }

        private readonly MatlAttribute attribute;

        public Vec4Param(MatlAttribute attribute)
        {
            this.attribute = attribute;
            ParamId = attribute.ParamId.ToString();
            UpdateColor();
        }

        private void UpdateColor()
        {
            // TODO: The linear -> srgb conversion should be handled by a library.
            var gamma = 1.0 / 2.2;
            var red = (float)Math.Pow(Value1, gamma);
            var green = (float)Math.Pow(Value2, gamma);
            var blue = (float)Math.Pow(Value3, gamma);
            var Color = SFGraphics.Utils.ColorUtils.GetColor(red, green, blue);
            ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Color.R, Color.G, Color.B));
        }
    }
}
