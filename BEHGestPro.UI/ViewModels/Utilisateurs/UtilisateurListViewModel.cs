using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using BEHGestPro.UI.Views.Shared;
using BEHGestPro.UI.Views.Utilisateurs;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BEHGestPro.UI.ViewModels.Utilisateurs;

public partial class UtilisateurListViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty] private ObservableCollection<Utilisateur> _utilisateurs = new();
    [ObservableProperty] private bool _isLoading;

    public UtilisateurListViewModel(AuthService authService) => _authService = authService;

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        var data = await _authService.GetAllUsersAsync();
        Utilisateurs = new ObservableCollection<Utilisateur>(data);
        IsLoading = false;
    }

    [RelayCommand]
    private void Create()
    {
        var vm = App.GetService<UtilisateurFormViewModel>();
        vm.PrepareNew();

        var view = new UtilisateurFormView { DataContext = vm };
        var dialog = new FormDialog("Nouvel utilisateur", view);
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
    private void Edit(Utilisateur? user)
    {
        if (user is null) return;
        var vm = App.GetService<UtilisateurFormViewModel>();
        vm.LoadForEdit(user);

        var view = new UtilisateurFormView { DataContext = vm };
        var dialog = new FormDialog("Modifier utilisateur", view);
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
    private async Task ToggleActifAsync(Utilisateur? user)
    {
        if (user is null) return;
        user.Actif = !user.Actif;
        await _authService.UpdateUserAsync(user);
        await LoadAsync();
    }
}
