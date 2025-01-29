using System.IO;
using System.Windows;
using Altium.FileSorter.Algorithm;
using Altium.FileSorter.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Altium.FileSorter;

public partial class App : Application
{
    internal static IServiceProvider ServiceProvider { get; }
    static App()
    {
        ServiceProvider = new ServiceCollection()
            .AddScoped<HomeViewModel>()
            .AddSingleton(new FileManager(Directory.GetCurrentDirectory()))
            .AddSingleton<DataGenerator>()
            .AddSingleton<FileLineFilesMerger>()
            .AddSingleton<FileLineSplitter>()
            .AddSingleton<Logger>()
            .BuildServiceProvider();
    }
}