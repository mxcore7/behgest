using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using BEHGestPro.UI.Views.Apprenants;
using BEHGestPro.UI.Views.Shared;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BEHGestPro.UI.ViewModels.Apprenants;

public partial class ApprenantListViewModel : ObservableObject
{
    private readonly ApprenantService _service;

    [ObservableProperty] private ObservableCollection<Apprenant> _apprenants = new();
    [ObservableProperty] private Apprenant? _selectedApprenant;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _notification = string.Empty;
    [ObservableProperty] private bool _showNotification;

    public ApprenantListViewModel(ApprenantService service)
    {
        _service = service;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        var data = await _service.GetAllAsync();
        Apprenants = new ObservableCollection<Apprenant>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        var vm   = App.GetService<ApprenantFormViewModel>();
        await vm.PrepareNewAsync();

        var view   = new ApprenantFormView { DataContext = vm };
        var dialog = new FormDialog("Nouvel apprenant", view);
        var mainWin = System.Windows.Application.Current.MainWindow;
        if (mainWin != null && mainWin != dialog) dialog.Owner = mainWin;
        vm.OnSaved = async () =>
        {
            dialog.CloseWithSuccess();
            await LoadAsync();
            ShowSuccessMessage("Apprenant créé avec succès.");
        };
        dialog.ShowDialog();
    }

    [RelayCommand]
    private async Task EditAsync(Apprenant? apprenant)
    {
        if (apprenant is null) return;
        var vm   = App.GetService<ApprenantFormViewModel>();
        await vm.LoadForEditAsync(apprenant.Id);

        var view   = new ApprenantFormView { DataContext = vm };
        var dialog = new FormDialog("Modifier apprenant", view);
        var mainWin = System.Windows.Application.Current.MainWindow;
        if (mainWin != null && mainWin != dialog) dialog.Owner = mainWin;
        vm.OnSaved = async () =>
        {
            dialog.CloseWithSuccess();
            await LoadAsync();
            ShowSuccessMessage("Apprenant mis à jour.");
        };
        dialog.ShowDialog();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        var data = await _service.SearchAsync(SearchText);
        Apprenants = new ObservableCollection<Apprenant>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private void ResetFilters()
    {
        SearchText = string.Empty;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(Apprenant? apprenant)
    {
        if (apprenant is null) return;
        var result = MessageBox.Show(
            $"Supprimer l'apprenant {apprenant.NomComplet} ?",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _service.DeleteAsync(apprenant.Id);
        Apprenants.Remove(apprenant);
        ShowSuccessMessage($"Apprenant supprimé.");
    }

    private void ShowSuccessMessage(string msg)
    {
        Notification = msg;
        ShowNotification = true;
        Task.Delay(3000).ContinueWith(_ => ShowNotification = false);
    }
}
