using System;

namespace BEHGestPro.Domain.Entities;

public class Employe
{
    public int Id { get; set; }
    public string Matricule { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Poste { get; set; }
    public string? Departement { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public DateTime? DateEmbauche { get; set; }
    public decimal SalaireBase { get; set; }
    public string? PhotoPath { get; set; }
    public bool Actif { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Salaire> Salaires { get; set; } = new List<Salaire>();
    public ICollection<AvanceSalaire> Avances { get; set; } = new List<AvanceSalaire>();
    public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
    public ICollection<Stagiaire> Stagiaires { get; set; } = new List<Stagiaire>();

    public string NomComplet => $"{Prenom} {Nom}";
}
