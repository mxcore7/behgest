using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using BEHGestPro.Documents.Fiches;
using BEHGestPro.UI.Services;
using BEHGestPro.UI.Views.Salaires;
using BEHGestPro.UI.Views.Shared;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BEHGestPro.UI.ViewModels.Salaires;

public partial class SalaireListViewModel : ObservableObject
{
    private readonly SalaireService _service;

    [ObservableProperty] private ObservableCollection<Salaire> _salaires = new();
    [ObservableProperty] private int _moisSelectionne = System.DateTime.Today.Month;
    [ObservableProperty] private int _anneeSelectionnee = System.DateTime.Today.Year;
    [ObservableProperty] private bool _isLoading;

    public SalaireListViewModel(SalaireService service) => _service = service;

    [RelayCommand]
    private async Task CreateAsync()
    {
        var vm = App.GetService<SalaireFormViewModel>();
        await vm.InitAsync();

        var view = new SalaireFormView { DataContext = vm };
        var dialog = new FormDialog("Générer fiche de salaire", view);
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
        var data = await _service.GetByPeriodeAsync(MoisSelectionne, AnneeSelectionnee);
        Salaires = new ObservableCollection<Salaire>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task ValiderAsync(Salaire? s)
    {
        if (s is null) return;
        await _service.ValiderPaiementAsync(s.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(Salaire? s)
    {
        if (s is null) return;
        var result = MessageBox.Show(
            $"Supprimer la fiche de paie de {s.Employe?.NomComplet} ({s.PeriodeLibelle}) ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _service.DeleteAsync(s.Id);
        Salaires.Remove(s);
    }

    [RelayCommand]
    private void PrintFiche(Salaire? s)
    {
        if (s is null) return;
        var doc = new FicheDePayeDocument(s, "BEH GESTION", "Direction des Ressources Humaines");
        PdfService.GenerateAndOpenPdf(doc, $"Fiche_Paie_{s.Employe?.Nom}_{s.PeriodeMois}_{s.PeriodeAnnee}");
    }
}
