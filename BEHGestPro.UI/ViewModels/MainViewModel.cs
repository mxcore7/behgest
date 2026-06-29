using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.UI.ViewModels.Apprenants;
using BEHGestPro.UI.ViewModels.Commandes;
using BEHGestPro.UI.ViewModels.Employes;
using BEHGestPro.UI.ViewModels.Formations;
using BEHGestPro.UI.ViewModels.Paiements;
using BEHGestPro.UI.ViewModels.Salaires;
using BEHGestPro.UI.ViewModels.Stagiaires;
using System;

namespace BEHGestPro.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty] private ObservableObject? _currentViewModel;
    [ObservableProperty] private string _activeModule = "Dashboard";
    [ObservableProperty] private bool _isNavExpanded = true;

    public MainViewModel(AuthService authService)
    {
        _authService = authService;
        NavigateTo("Dashboard");
    }

    public string CurrentUserName => _authService.CurrentUser?.NomComplet ?? "Utilisateur";
    public bool IsAdmin => _authService.IsAdmin;

    [RelayCommand]
    private void NavigateTo(string module)
    {
        ActiveModule = module;
        CurrentViewModel = module switch
        {
            "Dashboard"   => App.GetService<DashboardViewModel>(),
            "Apprenants"  => App.GetService<ApprenantListViewModel>(),
            "Stagiaires"  => App.GetService<StagiaireListViewModel>(),
            "Formations"  => App.GetService<FormationListViewModel>(),
            "Paiements"   => App.GetService<PaiementListViewModel>(),
            "Commandes"   => App.GetService<CommandeListViewModel>(),
            "Salaires"    => App.GetService<SalaireListViewModel>(),
            "Employes"    => App.GetService<EmployeListViewModel>(),
            _             => App.GetService<DashboardViewModel>()
        };
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        var loginView = App.GetService<BEHGestPro.UI.Views.LoginView>();
        loginView.Show();
        foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
            if (w is BEHGestPro.UI.Views.MainWindow) { w.Close(); break; }
    }

    [RelayCommand]
    private void ToggleNav() => IsNavExpanded = !IsNavExpanded;
}
