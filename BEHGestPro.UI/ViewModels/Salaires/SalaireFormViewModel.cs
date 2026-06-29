using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Salaires;

public partial class SalaireFormViewModel : ObservableObject
{
    private readonly SalaireService _service;
    private readonly EmployeService _employeService;

    [ObservableProperty] private ObservableCollection<Employe> _employes = new();
    [ObservableProperty] private Employe? _selectedEmploye;
    [ObservableProperty] private Salaire _salaire = new();
    [ObservableProperty] private int _mois = System.DateTime.Today.Month;
    [ObservableProperty] private int _annee = System.DateTime.Today.Year;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    public System.Action? OnSaved { get; set; }

    public SalaireFormViewModel(SalaireService service, EmployeService employeService)
    {
        _service = service;
        _employeService = employeService;
    }

    [RelayCommand]
    public async Task InitAsync()
    {
        var employes = await _employeService.GetActifsAsync();
        Employes = new ObservableCollection<Employe>(employes);
    }

    [RelayCommand]
    private async Task CreerFicheAsync()
    {
        if (SelectedEmploye is null) { ErrorMessage = "Sélectionnez un employé."; HasError = true; return; }
        IsLoading = true;
        HasError = false;
        try
        {
            Salaire = await _service.CreerFicheAsync(SelectedEmploye.Id, Mois, Annee);
            OnSaved?.Invoke();
        }
        catch (System.Exception ex) { ErrorMessage = ex.Message; HasError = true; }
        finally { IsLoading = false; }
    }
}
