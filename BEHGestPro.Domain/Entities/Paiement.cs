using System;

namespace BEHGestPro.Domain.Entities;

public class Paiement
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string TypeSource { get; set; } = string.Empty;
    public int SourceId { get; set; }
    public decimal MontantTotal { get; set; }
    public decimal MontantVerse { get; set; }
    public decimal MontantRestant => MontantTotal - MontantVerse;
    public string? ModePaiement { get; set; }
    public string Statut { get; set; } = "partiel";
    public DateTime DatePaiement { get; set; } = DateTime.Today;
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
