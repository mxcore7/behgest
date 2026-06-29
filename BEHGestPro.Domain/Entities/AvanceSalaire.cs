using System;

namespace BEHGestPro.Domain.Entities;

public class AvanceSalaire
{
    public int Id { get; set; }
    public int EmployeId { get; set; }
    public Employe? Employe { get; set; }
    public decimal Montant { get; set; }
    public DateTime DateAvance { get; set; } = DateTime.Today;
    public bool Remboursee { get; set; } = false;
    public int? SalaireId { get; set; }
    public Salaire? Salaire { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
