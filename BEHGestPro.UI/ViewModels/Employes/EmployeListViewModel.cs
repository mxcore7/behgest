using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using BEHGestPro.Documents.Badges;
using BEHGestPro.UI.Services;
using BEHGestPro.UI.Views.Employes;
using BEHGestPro.UI.Views.Shared;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BEHGestPro.UI.ViewModels.Employes;

public partial class EmployeListViewModel : ObservableObject
{
    private readonly EmployeService _service;

    [ObservableProperty] private ObservableCollection<Employe> _employes = new();
    [ObservableProperty] private Employe? _selectedEmploye;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;

    public EmployeListViewModel(EmployeService service) => _service = service;

    [RelayCommand]
    private async Task CreateAsync()
    {
        var vm = App.GetService<EmployeFormViewModel>();
        await vm.PrepareNewAsync();

        var view = new EmployeFormView { DataContext = vm };
        var dialog = new FormDialog("Nouvel employé", view, height: 650);
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
    private async Task EditAsync(Employe? employe)
    {
        if (employe is null) return;
        var vm = App.GetService<EmployeFormViewModel>();
        await vm.LoadForEditAsync(employe.Id);

        var view = new EmployeFormView { DataContext = vm };
        var dialog = new FormDialog("Modifier employé", view, height: 650);
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
        Employes = new ObservableCollection<Employe>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        var data = await _service.SearchAsync(SearchText);
        Employes = new ObservableCollection<Employe>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private void ResetFilters() { SearchText = string.Empty; _ = LoadAsync(); }

    [RelayCommand]
    private async Task DeleteAsync(Employe? e)
    {
        if (e is null) return;
        var result = MessageBox.Show(
            $"Supprimer l'employé {e.NomComplet} ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _service.DeleteAsync(e.Id);
        Employes.Remove(e);
    }

    [RelayCommand]
    private void PrintBadge(Employe? e)
    {
        if (e is null) return;
        var doc = new BadgeEmployeDocument(e, "BEH GESTION");
        PdfService.GenerateAndOpenPdf(doc, $"Badge_{e.Matricule}");
    }
}
