using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using BEHGestPro.Documents.Recus;
using BEHGestPro.UI.Services;
using BEHGestPro.UI.Views.Paiements;
using BEHGestPro.UI.Views.Shared;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BEHGestPro.UI.ViewModels.Paiements;

public partial class PaiementListViewModel : ObservableObject
{
    private readonly PaiementService _service;

    [ObservableProperty] private ObservableCollection<Paiement> _paiements = new();
    [ObservableProperty] private Paiement? _selectedPaiement;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;

    public PaiementListViewModel(PaiementService service) => _service = service;

    [RelayCommand]
    private async Task CreateAsync()
    {
        var vm = App.GetService<PaiementFormViewModel>();
        await vm.PrepareNewAsync();

        var view = new PaiementFormView { DataContext = vm };
        var dialog = new FormDialog("Nouveau paiement", view, height: 700);
        var mainWin = System.Windows.Application.Current.MainWindow;
        if (mainWin != null && mainWin != dialog) dialog.Owner = mainWin;
        vm.OnSaved = async () =>
        {
            dialog.CloseWithSuccess();
            await LoadAsync();
        };
        await System.Windows.Threading.Dispatcher.CurrentDispatcher
            .InvokeAsync(() => dialog.ShowDialog()).Task;
    }

    [RelayCommand]
    private async Task EditAsync(Paiement? paiement)
    {
        if (paiement is null) return;
        var vm = App.GetService<PaiementFormViewModel>();
        await vm.LoadForEditAsync(paiement.Id);

        var view = new PaiementFormView { DataContext = vm };
        var dialog = new FormDialog("Modifier paiement", view, height: 650);
        var mainWin = System.Windows.Application.Current.MainWindow;
        if (mainWin != null && mainWin != dialog) dialog.Owner = mainWin;
        vm.OnSaved = async () =>
        {
            dialog.CloseWithSuccess();
            await LoadAsync();
        };
        await System.Windows.Threading.Dispatcher.CurrentDispatcher
            .InvokeAsync(() => dialog.ShowDialog()).Task;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        var data = await _service.GetAllAsync();
        Paiements = new ObservableCollection<Paiement>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private void ResetFilters() { SearchText = string.Empty; _ = LoadAsync(); }

    [RelayCommand]
    private async Task DeleteAsync(Paiement? p)
    {
        if (p is null) return;
        var result = MessageBox.Show(
            $"Supprimer le paiement {p.Reference} ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _service.DeleteAsync(p.Id);
        Paiements.Remove(p);
    }

    [RelayCommand]
    private void PrintRecu(Paiement? p)
    {
        if (p is null) return;
        var beneficiaire = $"Client / Source #{p.SourceId}";
        var objet = $"Règlement de prestation ({p.TypeSource})";
        var doc = new RecuPaiementDocument(p, beneficiaire, objet, "BEH GESTION");
        PdfService.GenerateAndOpenPdf(doc, $"Recu_{p.Reference}");
    }
}
