using System;
using System.Windows.Data;
using System.Windows.Media;

namespace CrossModGui.Converters
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(Brush))]

	public class IsActiveBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// TODO: Don't hardcode these values.
			// TODO: Share these values with the control styles.
			var isActive = (bool)value;
			if (isActive)
				return new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));

			return new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// This conversion isn't needed.
			return null;
		}
	}
}
