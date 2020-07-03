using System;
using System.Windows.Data;
using System.Windows.Media;

namespace CrossModGui.Converters
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(Brush))]

	public class ColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var color = (System.Drawing.Color)value;
			// The default ForeColor for FileNodes has 0 alpha, so just hard code the alpha for now.
			return new SolidColorBrush(Color.FromArgb(255, color.R, color.G, color.B));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// This conversion isn't needed.
			return null;
		}
	}
}
