using System;
using System.Globalization;
using System.Windows.Data;

namespace BEHGestPro.UI.Converters;

public class HeightMultiplierConverter : IValueConverter
{
    public static readonly HeightMultiplierConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count) return Math.Max(4, count * 6);
        return 4;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
