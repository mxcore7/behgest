using System;

namespace BEHGestPro.Domain.Entities;

public class SessionFormation
{
    public int Id { get; set; }
    public int FormationId { get; set; }
    public Formation? Formation { get; set; }
    public string CodeSession { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string? Lieu { get; set; }
    public string? Formateur { get; set; }
    public int CapaciteMax { get; set; } = 30;
    public string Statut { get; set; } = "ouverte";
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<InscriptionFormation> Inscriptions { get; set; } = new List<InscriptionFormation>();
    public int NbInscrits => Inscriptions.Count;
    public bool EstComplete => NbInscrits >= CapaciteMax;
}
