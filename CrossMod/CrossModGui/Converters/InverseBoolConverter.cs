using System;
using System.Windows.Data;
using System.Windows.Media;

namespace CrossModGui.Converters
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(Brush))]

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
