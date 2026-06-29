using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Paiements;

public partial class PaiementFormViewModel : ObservableObject
{
    private readonly PaiementService _service;

    [ObservableProperty] private Paiement _paiement = new();
    [ObservableProperty] private bool _isEdit;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    public string[] ModesPaiement { get; } = { "Espèces", "Mobile Money", "Virement", "Chèque" };
    public string[] TypesSources { get; } = { "formation", "commande" };

    public System.Action? OnSaved { get; set; }

    public PaiementFormViewModel(PaiementService service) => _service = service;

    public async Task PrepareNewAsync()
    {
        IsEdit = false;
        Paiement = new Paiement
        {
            Reference = await _service.GenererReferenceAsync(),
            DatePaiement = System.DateTime.Today
        };
    }

    public async Task LoadForEditAsync(int id)
    {
        IsEdit = true;
        var p = await _service.GetByIdAsync(id);
        if (p is not null) Paiement = p;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Paiement.MontantTotal <= 0) { ErrorMessage = "Le montant total doit être positif."; HasError = true; return; }
        IsLoading = true;
        HasError = false;
        try
        {
            if (IsEdit) await _service.UpdateAsync(Paiement);
            else await _service.CreateAsync(Paiement);
            OnSaved?.Invoke();
        }
        catch (System.Exception ex) { ErrorMessage = ex.Message; HasError = true; }
        finally { IsLoading = false; }
    }
}
