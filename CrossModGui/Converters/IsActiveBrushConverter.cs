using CrossModGui.ViewModels;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace CrossModGui.Converters
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(Brush))]

	public class IsActiveBrushConverter : IValueConverter
	{
		private readonly Color activeColor = PreferencesWindowViewModel.Instance.EnableDarkTheme ? Color.FromArgb(255, 230, 230, 230) : Color.FromArgb(255, 0, 0, 0);
		private readonly Color disabledColor = Color.FromArgb(255, 180, 180, 180);

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var isActive = (bool)value;
			if (isActive)
				return new SolidColorBrush(activeColor);

			return new SolidColorBrush(disabledColor);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// This conversion isn't needed.
			return null;
		}
	}
}
