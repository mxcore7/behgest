using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Paiements;

public partial class PaiementFormViewModel : ObservableObject
{
    private readonly PaiementService _service;
    private readonly CommandeService _commandeService;

    [ObservableProperty] private Paiement _paiement = new();
    [ObservableProperty] private bool _isEdit;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    // Commandes disponibles pour la liaison
    [ObservableProperty] private ObservableCollection<Commande> _commandes = new();
    [ObservableProperty] private Commande? _selectedCommande;

    public string[] ModesPaiement { get; } = { "Espèces", "Mobile Money", "Virement", "Chèque" };
    public string[] TypesSources { get; } = { "formation", "commande" };

    /// <summary>Callback appelé après un enregistrement réussi.</summary>
    public Func<Task>? OnSaved { get; set; }

    public PaiementFormViewModel(PaiementService service, CommandeService commandeService)
    {
        _service = service;
        _commandeService = commandeService;
    }

    partial void OnSelectedCommandeChanged(Commande? value)
    {
        if (value is null) return;
        // Auto-remplissage depuis la commande sélectionnée
        Paiement = new Paiement
        {
            Reference    = Paiement.Reference,
            DatePaiement = Paiement.DatePaiement,
            TypeSource   = "commande",
            SourceId     = value.Id,
            MontantTotal = value.CoutTotal,
            ModePaiement = Paiement.ModePaiement,
            Notes        = Paiement.Notes
        };
    }

    public async Task PrepareNewAsync()
    {
        IsEdit = false;
        Paiement = new Paiement
        {
            Reference    = await _service.GenererReferenceAsync(),
            DatePaiement = System.DateTime.Today
        };
        await LoadCommandesAsync();
    }

    public async Task LoadForEditAsync(int id)
    {
        IsEdit = true;
        var p = await _service.GetByIdAsync(id);
        if (p is not null) Paiement = p;
        await LoadCommandesAsync();
        // Pré-sélectionner la commande si le paiement est lié à une commande
        if (Paiement.TypeSource == "commande")
            SelectedCommande = Commandes.FirstOrDefault(c => c.Id == Paiement.SourceId);
    }

    private async Task LoadCommandesAsync()
    {
        var list = await _commandeService.GetAllAsync();
        Commandes = new ObservableCollection<Commande>(list);
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
            if (OnSaved is not null) await OnSaved.Invoke();
        }
        catch (System.Exception ex) { ErrorMessage = ex.Message; HasError = true; }
        finally { IsLoading = false; }
    }
}
