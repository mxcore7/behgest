using System;

namespace BEHGestPro.Domain.Entities;

public class InscriptionFormation
{
    public int Id { get; set; }
    public int ApprenantId { get; set; }
    public Apprenant? Apprenant { get; set; }
    public int SessionId { get; set; }
    public SessionFormation? Session { get; set; }
    public DateTime DateInscription { get; set; } = DateTime.Today;
    public string Statut { get; set; } = "inscrit";
    public decimal? NoteFinale { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
