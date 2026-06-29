using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Employes;

public partial class EmployeFormViewModel : ObservableObject
{
    private readonly EmployeService _service;

    [ObservableProperty] private Employe _employe = new();
    [ObservableProperty] private bool _isEdit;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    public System.Action? OnSaved { get; set; }

    public EmployeFormViewModel(EmployeService service) => _service = service;

    public async Task PrepareNewAsync()
    {
        IsEdit = false;
        Employe = new Employe { Matricule = await _service.GenererMatriculeAsync(), Actif = true };
    }

    public async Task LoadForEditAsync(int id)
    {
        IsEdit = true;
        var e = await _service.GetByIdAsync(id);
        if (e is not null) Employe = e;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Employe.Nom) || string.IsNullOrWhiteSpace(Employe.Prenom))
        {
            ErrorMessage = "Nom et prénom obligatoires.";
            HasError = true;
            return;
        }
        IsLoading = true;
        HasError = false;
        try
        {
            if (IsEdit) await _service.UpdateAsync(Employe);
            else await _service.CreateAsync(Employe);
            OnSaved?.Invoke();
        }
        catch (System.Exception ex) { ErrorMessage = ex.Message; HasError = true; }
        finally { IsLoading = false; }
    }
}
