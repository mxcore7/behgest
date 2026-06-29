using BEHGestPro.UI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace BEHGestPro.UI.Views;

public partial class LoginView : Window
{
    private readonly LoginViewModel _vm;

    public LoginView(LoginViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
        Resources["InvertBool"] = new BEHGestPro.UI.Converters.InvertBoolConverter();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
        DragMove();

    private void CloseBtn_Click(object sender, RoutedEventArgs e) =>
        System.Windows.Application.Current.Shutdown();

    private async void BtnLogin_Click(object sender, RoutedEventArgs e) =>
        await _vm.LoginCommand.ExecuteAsync(TxtPassword.Password);
}
