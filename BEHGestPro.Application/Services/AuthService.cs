using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Application.Services;

public class AuthService
{
    private readonly IDbContextFactory<BehGestionDbContext> _factory;
    private Utilisateur? _currentUser;

    public AuthService(IDbContextFactory<BehGestionDbContext> factory)
    {
        _factory = factory;
    }

    public Utilisateur? CurrentUser => _currentUser;
    public string CurrentRole       => _currentUser?.Role ?? string.Empty;

    public bool IsAuthenticated => _currentUser is not null;

    // Rôles principaux
    public bool IsAdmin        => CurrentRole == "admin";
    public bool IsSecretaire   => CurrentRole == "secretaire";
    public bool IsDirecteur    => CurrentRole == "directeur";
    public bool IsInfographe   => CurrentRole == "infographe";
    public bool IsSerigraphe   => CurrentRole == "serigraphe";

    // Permissions composites
    /// <summary>Peut enregistrer clients, commandes, paiements, apprenants, stagiaires et éditer reçus.</summary>
    public bool CanEnregistrer  => IsAdmin || IsSecretaire || IsDirecteur;
    /// <summary>Peut consulter les commandes et rapports.</summary>
    public bool CanConsulter    => IsAdmin || IsDirecteur || IsSecretaire || IsInfographe || IsSerigraphe;
    /// <summary>Peut gérer les utilisateurs (création, modification de rôles).</summary>
    public bool CanManageUsers  => IsAdmin;
    /// <summary>Peut imprimer des historiques sur tous les modules.</summary>
    public bool CanPrintAll     => IsAdmin || IsDirecteur;
    /// <summary>Peut imprimer uniquement l'historique du jour (commandes).</summary>
    public bool CanPrintJour    => IsAdmin || IsSecretaire || IsDirecteur;
    /// <summary>Peut mettre à jour l'état des travaux (maquettes, impressions).</summary>
    public bool CanUpdateTravaux => IsAdmin || IsInfographe || IsSerigraphe;

    public async Task<bool> LoginAsync(string email, string password)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var user = await ctx.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == email && u.Actif);
        if (user is null) return false;

        if (!BCrypt.Net.BCrypt.Verify(password, user.MotDePasseHash)) return false;

        _currentUser = user;
        return true;
    }

    public void Logout() => _currentUser = null;

    public async Task<Utilisateur> CreateUserAsync(
        string nom, string prenom, string email, string password, string role)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        var user = new Utilisateur
        {
            Nom            = nom,
            Prenom         = prenom,
            Email          = email,
            MotDePasseHash = hash,
            Role           = role,
            Actif          = true,
            CreatedAt      = DateTime.UtcNow,
            UpdatedAt      = DateTime.UtcNow
        };
        await using var ctx = await _factory.CreateDbContextAsync();
        await ctx.Utilisateurs.AddAsync(user);
        await ctx.SaveChangesAsync();
        return user;
    }

    public async Task<bool> HasUsersAsync()
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Utilisateurs.AnyAsync(u => u.Actif);
    }

    public async Task<IEnumerable<Utilisateur>> GetAllUsersAsync()
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Utilisateurs.OrderBy(u => u.Nom).ToListAsync();
    }

    public async Task UpdateUserAsync(Utilisateur user)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var existing = await ctx.Utilisateurs.FindAsync(user.Id);
        if (existing is null) return;
        existing.Nom       = user.Nom;
        existing.Prenom    = user.Prenom;
        existing.Email     = user.Email;
        existing.Role      = user.Role;
        existing.Actif     = user.Actif;
        existing.UpdatedAt = DateTime.UtcNow;
        await ctx.SaveChangesAsync();
    }

    public async Task ResetPasswordAsync(int userId, string newPassword)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var existing = await ctx.Utilisateurs.FindAsync(userId);
        if (existing is null) return;
        existing.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
        existing.UpdatedAt     = DateTime.UtcNow;
        await ctx.SaveChangesAsync();
    }

    public async Task LogActionAsync(string action, string entite, int? entiteId = null)
    {
        if (_currentUser is null) return;
        var log = new AuditLog
        {
            UtilisateurId = _currentUser.Id,
            Action        = action,
            Entite        = entite,
            EntiteId      = entiteId,
            Timestamp     = DateTime.UtcNow
        };
        await using var ctx = await _factory.CreateDbContextAsync();
        await ctx.AuditLogs.AddAsync(log);
        await ctx.SaveChangesAsync();
    }

    public static string[] RolesDisponibles { get; } =
        { "admin", "secretaire", "directeur", "infographe", "serigraphe" };

    public static string LibelleRole(string role) => role switch
    {
        "admin"      => "Administrateur",
        "secretaire" => "Secrétaire",
        "directeur"  => "Directeur",
        "infographe" => "Infographe",
        "serigraphe" => "Sérigraphe",
        _            => role
    };
}
