using System;

namespace BEHGestPro.Domain.Entities;

public class Salaire
{
    public int Id { get; set; }
    public int EmployeId { get; set; }
    public Employe? Employe { get; set; }
    public int PeriodeMois { get; set; }
    public int PeriodeAnnee { get; set; }
    public decimal SalaireBase { get; set; }
    public decimal PrimeTotal { get; set; } = 0;
    public decimal AvanceDeduite { get; set; } = 0;
    public decimal Retenues { get; set; } = 0;
    public decimal NetAPayer => SalaireBase + PrimeTotal - AvanceDeduite - Retenues;
    public DateTime? DatePaiement { get; set; }
    public string Statut { get; set; } = "en_attente";
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string PeriodeLibelle => $"{PeriodeMois:D2}/{PeriodeAnnee}";
}
