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

    public Utilisateur? CurrentUser   => _currentUser;
    public bool IsAuthenticated       => _currentUser is not null;
    public bool IsAdmin               => _currentUser?.Role == "admin";
    public bool CanWrite              => _currentUser?.Role is "admin" or "operateur";

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
}
