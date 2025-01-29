using System.Windows;

namespace Altium.FileSorter.Windows;

public partial class HomeWindow : Window
{
    private HomeViewModel ViewModel => (HomeViewModel)DataContext;

    public HomeWindow()
    {
        InitializeComponent();

        Loaded += (_, _) => ViewModel.Initialize();
    }
}