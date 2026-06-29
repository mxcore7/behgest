using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Apprenants;

public partial class ApprenantFormViewModel : ObservableObject
{
    private readonly ApprenantService _service;

    [ObservableProperty] private Apprenant _apprenant = new();
    [ObservableProperty] private bool _isEdit;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    public System.Action? OnSaved { get; set; }

    public ApprenantFormViewModel(ApprenantService service)
    {
        _service = service;
    }

    public async Task LoadForEditAsync(int id)
    {
        IsEdit = true;
        var a = await _service.GetByIdAsync(id);
        if (a is not null) Apprenant = a;
    }

    public async Task PrepareNewAsync()
    {
        IsEdit = false;
        Apprenant = new Apprenant
        {
            Matricule = await _service.GenererMatriculeAsync()
        };
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!Validate()) return;
        IsLoading = true;
        HasError = false;

        try
        {
            if (IsEdit)
                await _service.UpdateAsync(Apprenant);
            else
                await _service.CreateAsync(Apprenant);

            OnSaved?.Invoke();
        }
        catch (System.Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
        finally { IsLoading = false; }
    }

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(Apprenant.Nom))
        {
            ErrorMessage = "Le nom est obligatoire.";
            HasError = true;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Apprenant.Prenom))
        {
            ErrorMessage = "Le prénom est obligatoire.";
            HasError = true;
            return false;
        }
        return true;
    }
}
