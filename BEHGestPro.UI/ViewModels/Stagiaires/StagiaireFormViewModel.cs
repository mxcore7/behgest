using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Stagiaires;

public partial class StagiaireFormViewModel : ObservableObject
{
    private readonly StagiaireService _service;
    private readonly EmployeService _employeService;

    [ObservableProperty] private Stagiaire _stagiaire = new();
    [ObservableProperty] private ObservableCollection<Employe> _encadrants = new();
    [ObservableProperty] private bool _isEdit;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    public System.Action? OnSaved { get; set; }

    public StagiaireFormViewModel(StagiaireService service, EmployeService employeService)
    {
        _service = service;
        _employeService = employeService;
    }

    public async Task InitAsync()
    {
        var employes = await _employeService.GetActifsAsync();
        Encadrants = new ObservableCollection<Employe>(employes);
    }

    public async Task LoadForEditAsync(int id)
    {
        IsEdit = true;
        await InitAsync();
        var s = await _service.GetByIdAsync(id);
        if (s is not null) Stagiaire = s;
    }

    public async Task PrepareNewAsync()
    {
        IsEdit = false;
        await InitAsync();
        Stagiaire = new Stagiaire
        {
            Matricule = await _service.GenererMatriculeAsync(),
            DateDebut = System.DateTime.Today,
            DateFin = System.DateTime.Today.AddMonths(3)
        };
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Stagiaire.Nom) || string.IsNullOrWhiteSpace(Stagiaire.Prenom))
        {
            ErrorMessage = "Nom et prénom obligatoires.";
            HasError = true;
            return;
        }
        IsLoading = true;
        HasError = false;
        try
        {
            if (IsEdit) await _service.UpdateAsync(Stagiaire);
            else await _service.CreateAsync(Stagiaire);
            OnSaved?.Invoke();
        }
        catch (System.Exception ex) { ErrorMessage = ex.Message; HasError = true; }
        finally { IsLoading = false; }
    }
}
