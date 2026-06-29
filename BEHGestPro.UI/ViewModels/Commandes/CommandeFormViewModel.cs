using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Commandes;

public partial class CommandeFormViewModel : ObservableObject
{
    private readonly CommandeService _service;
    private readonly EmployeService _employeService;

    [ObservableProperty] private Commande _commande = new();
    [ObservableProperty] private ObservableCollection<Employe> _employes = new();
    [ObservableProperty] private bool _isEdit;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    public string[] TypesService { get; } = { "Formation", "Conseil", "Audit", "Développement", "Autre" };

    public System.Action? OnSaved { get; set; }

    public CommandeFormViewModel(CommandeService service, EmployeService employeService)
    {
        _service = service;
        _employeService = employeService;
    }

    public async Task InitAsync()
    {
        var employes = await _employeService.GetActifsAsync();
        Employes = new ObservableCollection<Employe>(employes);
    }

    public async Task PrepareNewAsync()
    {
        IsEdit = false;
        await InitAsync();
        Commande = new Commande
        {
            Numero = await _service.GenererNumeroAsync(),
            DateCommande = System.DateTime.Today
        };
    }

    public async Task LoadForEditAsync(int id)
    {
        IsEdit = true;
        await InitAsync();
        var c = await _service.GetByIdAsync(id);
        if (c is not null) Commande = c;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Commande.ClientNom))
        {
            ErrorMessage = "Le nom du client est obligatoire.";
            HasError = true;
            return;
        }
        IsLoading = true;
        HasError = false;
        try
        {
            if (IsEdit) await _service.UpdateAsync(Commande);
            else await _service.CreateAsync(Commande);
            OnSaved?.Invoke();
        }
        catch (System.Exception ex) { ErrorMessage = ex.Message; HasError = true; }
        finally { IsLoading = false; }
    }
}
