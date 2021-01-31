using CrossModGui.Tools;
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

            var paramIdText = attribute.ParamId.ToString();
            ParamId = MaterialParamDescriptions.Instance.GetDescriptionText(paramIdText);

            UpdateColor();
            TryAssignValuesFromDescription(paramIdText);
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

        private bool TryAssignValuesFromDescription(string paramId)
        {
            if (CustomVectorDescriptions.Instance.ParamDescriptionsByName.TryGetValue(paramId,
                out CustomVectorDescriptions.CustomVectorParamDescription? description))
            {
                Label1 = description.Label1 ?? "Unused";
                Min1 = description.Min1.GetValueOrDefault(0);
                Max1 = description.Max1.GetValueOrDefault(1);

                Label2 = description.Label2 ?? "Unused";
                Min2 = description.Min2.GetValueOrDefault(0);
                Max2 = description.Max2.GetValueOrDefault(1);

                Label3 = description.Label3 ?? "Unused";
                Min3 = description.Min3.GetValueOrDefault(0);
                Max3 = description.Max3.GetValueOrDefault(1);

                Label4 = description.Label4 ?? "Unused";
                Min4 = description.Min4.GetValueOrDefault(0);
                Max4 = description.Max4.GetValueOrDefault(1);
                return true;
            }

            return false;
        }
    }
}
