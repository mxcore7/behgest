namespace BEHGestPro.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalApprenants { get; set; }
    public int FormationsActives { get; set; }
    public int CommandesEnCours { get; set; }
    public decimal PaiementsMois { get; set; }
    public int StagiairesActuels { get; set; }
    public int SalairesEnAttente { get; set; }
    public List<MoisInscriptionDto> InscriptionsMensuelles { get; set; } = new();
}

public class MoisInscriptionDto
{
    public int Mois { get; set; }
    public int Annee { get; set; }
    public int NbInscriptions { get; set; }
    public string LibelleMois => $"{Annee}-{Mois:D2}";
}
