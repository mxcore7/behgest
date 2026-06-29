using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BEHGestPro.Application.DTOs;
using BEHGestPro.Application.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BEHGestPro.UI.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DashboardService _service;

    [ObservableProperty] private int _totalApprenants;
    [ObservableProperty] private int _formationsActives;
    [ObservableProperty] private int _commandesEnCours;
    [ObservableProperty] private decimal _paiementsMois;
    [ObservableProperty] private int _stagiairesActuels;
    [ObservableProperty] private int _salairesEnAttente;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private ObservableCollection<MoisInscriptionDto> _inscriptionsMensuelles = new();

    public DashboardViewModel(DashboardService service)
    {
        _service = service;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        var stats = await _service.GetStatsAsync();
        TotalApprenants = stats.TotalApprenants;
        FormationsActives = stats.FormationsActives;
        CommandesEnCours = stats.CommandesEnCours;
        PaiementsMois = stats.PaiementsMois;
        StagiairesActuels = stats.StagiairesActuels;
        SalairesEnAttente = stats.SalairesEnAttente;
        InscriptionsMensuelles = new ObservableCollection<MoisInscriptionDto>(stats.InscriptionsMensuelles);
        IsLoading = false;
    }

    public string PaiementsMoisFormatted => $"{PaiementsMois:N0} XAF";
}
