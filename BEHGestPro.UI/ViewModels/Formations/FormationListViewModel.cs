using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using BEHGestPro.Documents.Attestations;
using BEHGestPro.UI.Services;
using BEHGestPro.UI.Views.Formations;
using BEHGestPro.UI.Views.Shared;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BEHGestPro.UI.ViewModels.Formations;

public partial class FormationListViewModel : ObservableObject
{
    private readonly FormationService _service;

    [ObservableProperty] private ObservableCollection<Formation> _formations = new();
    [ObservableProperty] private Formation? _selectedFormation;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;

    public FormationListViewModel(FormationService service) => _service = service;

    [RelayCommand]
    private async Task CreateAsync()
    {
        var vm = App.GetService<FormationFormViewModel>();
        await vm.PrepareNewAsync();

        var view = new FormationFormView { DataContext = vm };
        var dialog = new FormDialog("Nouvelle formation", view);
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
    private async Task EditAsync(Formation? formation)
    {
        if (formation is null) return;
        var vm = App.GetService<FormationFormViewModel>();
        await vm.LoadForEditAsync(formation.Id);

        var view = new FormationFormView { DataContext = vm };
        var dialog = new FormDialog("Modifier formation", view);
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
        Formations = new ObservableCollection<Formation>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        var data = await _service.SearchAsync(SearchText);
        Formations = new ObservableCollection<Formation>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private void ResetFilters() { SearchText = string.Empty; _ = LoadAsync(); }

    [RelayCommand]
    private async Task DeleteAsync(Formation? f)
    {
        if (f is null) return;
        var result = MessageBox.Show(
            $"Supprimer la formation {f.Intitule} ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _service.DeleteAsync(f.Id);
        Formations.Remove(f);
    }

    [RelayCommand]
    private void PrintAttestation(Formation? f)
    {
        if (f is null) return;
        var dummyApprenant = new Apprenant { Nom = "MODÈLE", Prenom = "APPRENANT" };
        var dummyInscription = new InscriptionFormation
        {
            Session = new SessionFormation
            {
                Formation = f,
                DateDebut = DateTime.Today,
                DateFin = DateTime.Today.AddDays(f.DureeHeures > 0 ? f.DureeHeures / 7 : 30),
                Lieu = "Centre BEH GESTION",
                Formateur = "Formateur Principal"
            }
        };
        var doc = new AttestationFormationDocument(dummyApprenant, dummyInscription, "BEH GESTION");
        PdfService.GenerateAndOpenPdf(doc, $"Attestation_Formation_{f.Code}");
    }
}
