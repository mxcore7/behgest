using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using BEHGestPro.Documents.Attestations;
using BEHGestPro.UI.Services;
using BEHGestPro.UI.Views.Stagiaires;
using BEHGestPro.UI.Views.Shared;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BEHGestPro.UI.ViewModels.Stagiaires;

public partial class StagiaireListViewModel : ObservableObject
{
    private readonly StagiaireService _service;

    [ObservableProperty] private ObservableCollection<Stagiaire> _stagiaires = new();
    [ObservableProperty] private Stagiaire? _selectedStagiaire;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;

    public StagiaireListViewModel(StagiaireService service) => _service = service;

    [RelayCommand]
    private async Task CreateAsync()
    {
        var vm = App.GetService<StagiaireFormViewModel>();
        await vm.PrepareNewAsync();

        var view = new StagiaireFormView { DataContext = vm };
        var dialog = new FormDialog("Nouveau stagiaire", view);
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
    private async Task EditAsync(Stagiaire? stagiaire)
    {
        if (stagiaire is null) return;
        var vm = App.GetService<StagiaireFormViewModel>();
        await vm.LoadForEditAsync(stagiaire.Id);

        var view = new StagiaireFormView { DataContext = vm };
        var dialog = new FormDialog("Modifier stagiaire", view);
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
        await _service.MettreAJourStatutsAsync();
        var data = await _service.GetAllAsync();
        Stagiaires = new ObservableCollection<Stagiaire>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        var data = await _service.SearchAsync(SearchText);
        Stagiaires = new ObservableCollection<Stagiaire>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private void ResetFilters()
    {
        SearchText = string.Empty;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(Stagiaire? stagiaire)
    {
        if (stagiaire is null) return;
        var result = MessageBox.Show(
            $"Supprimer le stagiaire {stagiaire.NomComplet} ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _service.DeleteAsync(stagiaire.Id);
        Stagiaires.Remove(stagiaire);
    }

    [RelayCommand]
    private void PrintAttestation(Stagiaire? stagiaire)
    {
        if (stagiaire is null) return;
        var doc = new AttestationStageDocument(stagiaire, "BEH GESTION");
        PdfService.GenerateAndOpenPdf(doc, $"Attestation_Stage_{stagiaire.Nom}_{stagiaire.Prenom}");
    }
}
