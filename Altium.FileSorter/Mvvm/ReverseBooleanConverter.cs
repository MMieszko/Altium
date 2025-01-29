using System.Globalization;
using System.Windows.Data;

namespace Altium.FileSorter.Mvvm;

public class ReverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is not bool @bool ? value : !@bool;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}