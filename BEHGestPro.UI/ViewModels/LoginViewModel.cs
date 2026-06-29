using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.UI.Views;
using System.Threading.Tasks;
using System.Windows;

namespace BEHGestPro.UI.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _hasError;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync(string? password)
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Veuillez renseigner votre email et mot de passe.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;
        HasError = false;

        var success = await _authService.LoginAsync(Email, password);

        if (success)
        {
            var mainWindow = App.GetService<MainWindow>();
            System.Windows.Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            foreach (Window w in System.Windows.Application.Current.Windows)
                if (w is LoginView) { w.Close(); break; }
        }
        else
        {
            ErrorMessage = "Email ou mot de passe incorrect.";
            HasError = true;
        }

        IsLoading = false;
    }
}
