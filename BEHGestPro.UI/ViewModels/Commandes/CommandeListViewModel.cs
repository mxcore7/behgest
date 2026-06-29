using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using BEHGestPro.Documents.Commandes;
using BEHGestPro.UI.Services;
using BEHGestPro.UI.Views.Commandes;
using BEHGestPro.UI.Views.Shared;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BEHGestPro.UI.ViewModels.Commandes;

public partial class CommandeListViewModel : ObservableObject
{
    private readonly CommandeService _service;

    [ObservableProperty] private ObservableCollection<Commande> _commandes = new();
    [ObservableProperty] private Commande? _selectedCommande;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;

    public CommandeListViewModel(CommandeService service) => _service = service;

    [RelayCommand]
    private async Task CreateAsync()
    {
        var vm = App.GetService<CommandeFormViewModel>();
        await vm.PrepareNewAsync();

        var view = new CommandeFormView { DataContext = vm };
        var dialog = new FormDialog("Nouvelle commande", view);
        var mainWin = System.Windows.Application.Current.MainWindow;
        if (mainWin != null && mainWin != dialog) dialog.Owner = mainWin;
        vm.OnSaved = async () =>
        {
            dialog.CloseWithSuccess();
            await LoadAsync();
        };
        dialog.ShowDialog();
    }

    [RelayCommand]
    private async Task EditAsync(Commande? commande)
    {
        if (commande is null) return;
        var vm = App.GetService<CommandeFormViewModel>();
        await vm.LoadForEditAsync(commande.Id);

        var view = new CommandeFormView { DataContext = vm };
        var dialog = new FormDialog("Modifier commande", view);
        var mainWin = System.Windows.Application.Current.MainWindow;
        if (mainWin != null && mainWin != dialog) dialog.Owner = mainWin;
        vm.OnSaved = async () =>
        {
            dialog.CloseWithSuccess();
            await LoadAsync();
        };
        dialog.ShowDialog();
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        var data = await _service.GetAllAsync();
        Commandes = new ObservableCollection<Commande>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        var data = await _service.SearchAsync(SearchText);
        Commandes = new ObservableCollection<Commande>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private void ResetFilters() { SearchText = string.Empty; _ = LoadAsync(); }

    [RelayCommand]
    private async Task ChangerStatutAsync((int commandeId, string statut) args)
    {
        await _service.ChangerStatutAsync(args.commandeId, args.statut);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(Commande? c)
    {
        if (c is null) return;
        var result = MessageBox.Show(
            $"Supprimer la commande {c.Numero} ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _service.DeleteAsync(c.Id);
        Commandes.Remove(c);
    }

    [RelayCommand]
    private void PrintBon(Commande? c)
    {
        if (c is null) return;
        var doc = new BonCommandeDocument(c, "BEH GESTION");
        PdfService.GenerateAndOpenPdf(doc, $"Bon_Commande_{c.Numero}");
    }
}
