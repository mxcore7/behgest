using BEHGestPro.UI.ViewModels;
using System.Windows;

namespace BEHGestPro.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
