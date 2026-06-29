using System;

namespace BEHGestPro.Domain.Entities;

public class Stagiaire
{
    public int Id { get; set; }
    public string Matricule { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public DateTime? DateNaissance { get; set; }
    public char? Sexe { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string? Etablissement { get; set; }
    public string? NiveauEtude { get; set; }
    public string? ServiceAccueil { get; set; }
    public int? EncadrantId { get; set; }
    public Employe? Encadrant { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Statut { get; set; } = "en_cours";
    public string? PhotoPath { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string NomComplet => $"{Prenom} {Nom}";
    public int DureeJours => (int)(DateFin - DateDebut).TotalDays;
    public int DureeSemaines => DureeJours / 7;
    public double ProgressionPct => DateDebut >= DateTime.Today ? 0
        : Math.Min(100, (DateTime.Today - DateDebut).TotalDays / Math.Max(1, (DateFin - DateDebut).TotalDays) * 100);
}
