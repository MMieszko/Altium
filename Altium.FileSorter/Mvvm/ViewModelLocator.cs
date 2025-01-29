using Altium.FileSorter.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Altium.FileSorter.Mvvm;

public class ViewModelLocator
{
    public static HomeViewModel HomeViewModel => App.ServiceProvider.GetService<HomeViewModel>()!;
}