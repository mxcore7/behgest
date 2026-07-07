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
using BEHGestPro.UI.ViewModels.Utilisateurs;
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

    // ── Infos utilisateur ────────────────────────────────────────────────────
    public string CurrentUserName  => _authService.CurrentUser?.NomComplet ?? "Utilisateur";
    public string CurrentRoleLabel => AuthService.LibelleRole(_authService.CurrentRole);

    // ── Permissions (pour Visibility bindings dans le XAML) ─────────────────
    public bool IsAdmin          => _authService.IsAdmin;
    public bool CanEnregistrer   => _authService.CanEnregistrer;
    public bool CanConsulter     => _authService.CanConsulter;
    public bool CanManageUsers   => _authService.CanManageUsers;
    public bool CanPrintAll      => _authService.CanPrintAll;
    public bool CanPrintJour     => _authService.CanPrintJour;
    public bool CanUpdateTravaux => _authService.CanUpdateTravaux;

    // Modules visibles par rôle
    public bool ShowApprenants   => _authService.IsAdmin || _authService.IsSecretaire || _authService.IsDirecteur;
    public bool ShowStagiaires   => _authService.IsAdmin || _authService.IsSecretaire || _authService.IsDirecteur;
    public bool ShowFormations   => _authService.IsAdmin || _authService.IsSecretaire || _authService.IsDirecteur;
    public bool ShowPaiements    => _authService.IsAdmin || _authService.IsSecretaire || _authService.IsDirecteur;
    public bool ShowCommandes    => true; // Tous les rôles voient les commandes (lecture seule pour certains)
    public bool ShowEmployes     => _authService.IsAdmin || _authService.IsDirecteur;
    public bool ShowSalaires     => _authService.IsAdmin || _authService.IsDirecteur;
    public bool ShowUtilisateurs => _authService.IsAdmin;

    // Commandes : peut créer / modifier / supprimer ?
    public bool CanWriteCommandes => _authService.IsAdmin || _authService.IsSecretaire || _authService.IsDirecteur;

    [RelayCommand]
    private void NavigateTo(string module)
    {
        ActiveModule = module;
        CurrentViewModel = module switch
        {
            "Dashboard"    => App.GetService<DashboardViewModel>(),
            "Apprenants"   => App.GetService<ApprenantListViewModel>(),
            "Stagiaires"   => App.GetService<StagiaireListViewModel>(),
            "Formations"   => App.GetService<FormationListViewModel>(),
            "Paiements"    => App.GetService<PaiementListViewModel>(),
            "Commandes"    => App.GetService<CommandeListViewModel>(),
            "Salaires"     => App.GetService<SalaireListViewModel>(),
            "Employes"     => App.GetService<EmployeListViewModel>(),
            "Utilisateurs" => App.GetService<UtilisateurListViewModel>(),
            _              => App.GetService<DashboardViewModel>()
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
