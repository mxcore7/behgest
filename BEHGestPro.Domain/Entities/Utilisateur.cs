using System;

namespace BEHGestPro.Domain.Entities;

public class Utilisateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty;
    public string Role { get; set; } = "operateur";
    public bool Actif { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string NomComplet => $"{Prenom} {Nom}";
}
