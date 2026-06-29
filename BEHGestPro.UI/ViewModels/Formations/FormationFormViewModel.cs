using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.Services;
using BEHGestPro.Domain.Entities;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels.Formations;

public partial class FormationFormViewModel : ObservableObject
{
    private readonly FormationService _service;

    [ObservableProperty] private Formation _formation = new();
    [ObservableProperty] private bool _isEdit;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;

    public System.Action? OnSaved { get; set; }

    public FormationFormViewModel(FormationService service) => _service = service;

    public async Task PrepareNewAsync()
    {
        IsEdit = false;
        Formation = new Formation { Code = await _service.GenererCodeFormationAsync() };
    }

    public async Task LoadForEditAsync(int id)
    {
        IsEdit = true;
        var f = await _service.GetByIdAsync(id);
        if (f is not null) Formation = f;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Formation.Intitule))
        {
            ErrorMessage = "L'intitulé est obligatoire.";
            HasError = true;
            return;
        }
        IsLoading = true;
        HasError = false;
        try
        {
            if (IsEdit) await _service.UpdateAsync(Formation);
            else await _service.CreateAsync(Formation);
            OnSaved?.Invoke();
        }
        catch (System.Exception ex) { ErrorMessage = ex.Message; HasError = true; }
        finally { IsLoading = false; }
    }
}
