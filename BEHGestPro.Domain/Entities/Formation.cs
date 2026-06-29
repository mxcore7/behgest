using System;

namespace BEHGestPro.Domain.Entities;

public class Formation
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DureeHeures { get; set; }
    public decimal Cout { get; set; }
    public string? Categorie { get; set; }
    public string? Programme { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SessionFormation> Sessions { get; set; } = new List<SessionFormation>();
}
