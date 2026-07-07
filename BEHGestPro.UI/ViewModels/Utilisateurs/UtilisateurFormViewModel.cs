using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Utilisateurs;

public partial class UtilisateurFormViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty] private Utilisateur _utilisateur = new();
    [ObservableProperty] private string _motDePasse = string.Empty;
    [ObservableProperty] private bool _isEdit;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    public string[] RolesDisponibles => AuthService.RolesDisponibles;

    public System.Func<Task>? OnSaved { get; set; }

    public UtilisateurFormViewModel(AuthService authService) => _authService = authService;

    public void PrepareNew()
    {
        IsEdit = false;
        Utilisateur = new Utilisateur { Role = "secretaire", Actif = true };
        MotDePasse = string.Empty;
    }

    public void LoadForEdit(Utilisateur user)
    {
        IsEdit = true;
        Utilisateur = new Utilisateur
        {
            Id     = user.Id,
            Nom    = user.Nom,
            Prenom = user.Prenom,
            Email  = user.Email,
            Role   = user.Role,
            Actif  = user.Actif
        };
        MotDePasse = string.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Utilisateur.Nom) || string.IsNullOrWhiteSpace(Utilisateur.Email))
        {
            ErrorMessage = "Le nom et l'email sont obligatoires.";
            HasError = true;
            return;
        }
        if (!IsEdit && string.IsNullOrWhiteSpace(MotDePasse))
        {
            ErrorMessage = "Le mot de passe est obligatoire pour un nouvel utilisateur.";
            HasError = true;
            return;
        }

        IsLoading = true;
        HasError  = false;
        try
        {
            if (IsEdit)
            {
                await _authService.UpdateUserAsync(Utilisateur);
                if (!string.IsNullOrWhiteSpace(MotDePasse))
                    await _authService.ResetPasswordAsync(Utilisateur.Id, MotDePasse);
            }
            else
            {
                await _authService.CreateUserAsync(
                    Utilisateur.Nom, Utilisateur.Prenom, Utilisateur.Email,
                    MotDePasse, Utilisateur.Role);
            }
            if (OnSaved is not null) await OnSaved.Invoke();
        }
        catch (System.Exception ex) { ErrorMessage = ex.Message; HasError = true; }
        finally { IsLoading = false; }
    }
}
