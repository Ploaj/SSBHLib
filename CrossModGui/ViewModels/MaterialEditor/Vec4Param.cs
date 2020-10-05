using System;
using System.Windows.Media;

namespace CrossModGui.ViewModels
{
    public partial class MaterialEditorWindowViewModel
    {
        public class Vec4Param : ViewModelBase
        {

            public string ParamId { get; set; }
            public Brush ColorBrush { get; set; }

            public string Label1 { get; set; } = "X";
            public float Min1 { get; set; } = 0.0f;
            public float Max1 { get; set; } = 1.0f;
            public float Value1
            {
                get => value1;
                set
                {
                    value1 = value;
                    UpdateColor();
                }
            }
            private float value1;

            public string Label2 { get; set; } = "Y";
            public float Min2 { get; set; } = 0.0f;
            public float Max2 { get; set; } = 1.0f;
            public float Value2
            {
                get => value2;
                set
                {
                    value2 = value;
                    UpdateColor();
                }
            }
            private float value2;

            public string Label3 { get; set; } = "Z";
            public float Min3 { get; set; } = 0.0f;
            public float Max3 { get; set; } = 1.0f;
            public float Value3
            {
                get => value3;
                set
                {
                    value3 = value;
                    UpdateColor();
                }
            }
            private float value3;

            public string Label4 { get; set; } = "W";
            public float Min4 { get; set; } = 0.0f;
            public float Max4 { get; set; } = 1.0f;
            public float Value4
            {
                get => value4;
                set
                {
                    value4 = value;
                    UpdateColor();
                }
            }
            private float value4;

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
}
