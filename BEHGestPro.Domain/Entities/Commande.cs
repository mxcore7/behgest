using System;

namespace BEHGestPro.Domain.Entities;

public class Commande
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string ClientNom { get; set; } = string.Empty;
    public string? ClientTelephone { get; set; }
    public string? ClientEmail { get; set; }
    public string TypeService { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal CoutTotal { get; set; }
    public string Statut { get; set; } = "en_attente";
    public int? EmployeId { get; set; }
    public Employe? Employe { get; set; }
    public DateTime DateCommande { get; set; } = DateTime.Today;
    public DateTime? DateLivraisonPrevue { get; set; }
    public DateTime? DateLivraisonReelle { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
