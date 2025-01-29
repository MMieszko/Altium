using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Altium.FileSorter.Mvvm;

public abstract class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Dictionary<string, object> _propertyBackingDictionary;

    protected BindableBase()
    {
        _propertyBackingDictionary = new Dictionary<string, object>();
    }

    protected virtual T GetPropertyValue<T>([CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

        if (_propertyBackingDictionary.TryGetValue(propertyName, out var value))
            return (T)value;

        return default!;
    }

    protected virtual bool SetPropertyValue<T>(T? newValue, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

        if (EqualityComparer<T>.Default.Equals(newValue, GetPropertyValue<T>(propertyName))) return false;

        _propertyBackingDictionary[propertyName] = newValue;
        RaisePropertyChanged(propertyName);
        return true;
    }

    protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}