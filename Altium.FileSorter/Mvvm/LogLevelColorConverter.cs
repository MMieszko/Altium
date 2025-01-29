using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Altium.FileSorter.Mvvm;

public class LogLevelColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        => value is not LogMessage log
            ? Brushes.Black
            : log.Level switch
            {
                LogLevel.Success => Brushes.Green,
                LogLevel.Warning => Brushes.Orange,
                LogLevel.Error => Brushes.Red,
                _ => Brushes.Black
            };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}