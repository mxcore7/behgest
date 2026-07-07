using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Employes;

public partial class EmployeFormViewModel : ObservableObject
{
    private readonly EmployeService _service;

    [ObservableProperty] private Employe _employe = new();
    [ObservableProperty] private bool _isEdit;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    public System.Action? OnSaved { get; set; }

    public EmployeFormViewModel(EmployeService service) => _service = service;

    public async Task PrepareNewAsync()
    {
        IsEdit = false;
        Employe = new Employe { Matricule = await _service.GenererMatriculeAsync(), Actif = true };
    }

    public async Task LoadForEditAsync(int id)
    {
        IsEdit = true;
        var e = await _service.GetByIdAsync(id);
        if (e is not null) Employe = e;
    }

    [RelayCommand]
    private void SelectPhoto()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Sélectionner une photo",
            Filter = "Fichiers images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
        };
        if (dialog.ShowDialog() == true)
        {
            var photosDir = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Photos", "Employes");
            if (!System.IO.Directory.Exists(photosDir))
                System.IO.Directory.CreateDirectory(photosDir);

            var ext = System.IO.Path.GetExtension(dialog.FileName);
            var fileName = $"{Employe.Matricule}_{System.Guid.NewGuid():N}{ext}";
            var destPath = System.IO.Path.Combine(photosDir, fileName);

            System.IO.File.Copy(dialog.FileName, destPath, true);

            // Stocker le chemin RELATIF pour la portabilité entre machines
            Employe.PhotoPath = System.IO.Path.Combine("Photos", "Employes", fileName);
            OnPropertyChanged(nameof(Employe));
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Employe.Nom) || string.IsNullOrWhiteSpace(Employe.Prenom))
        {
            ErrorMessage = "Nom et prénom obligatoires.";
            HasError = true;
            return;
        }
        IsLoading = true;
        HasError = false;
        try
        {
            if (IsEdit) await _service.UpdateAsync(Employe);
            else await _service.CreateAsync(Employe);
            OnSaved?.Invoke();
        }
        catch (System.Exception ex) { ErrorMessage = ex.Message; HasError = true; }
        finally { IsLoading = false; }
    }
}
