using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using BEHGestPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Application.Services;

public class FormationService
{
    private readonly FormationRepository _repo;
    private readonly BehGestionDbContext _context;

    public FormationService(FormationRepository repo, BehGestionDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<string> GenererCodeFormationAsync()
    {
        var count = await _context.Formations.IgnoreQueryFilters().CountAsync();
        return $"FOR-{(count + 1):D3}";
    }

    public async Task<string> GenererCodeSessionAsync(int formationId)
    {
        var today = DateTime.Today.ToString("yyyyMMdd");
        var count = await _context.SessionsFormation.IgnoreQueryFilters()
            .CountAsync(s => s.FormationId == formationId);
        return $"SES-{formationId:D3}-{(count + 1):D3}";
    }

    public async Task<IEnumerable<Formation>> GetAllAsync() =>
        await _repo.GetAllWithSessionCountAsync();

    public async Task<IEnumerable<Formation>> SearchAsync(string term) =>
        string.IsNullOrWhiteSpace(term) ? await _repo.GetAllAsync() : await _repo.SearchAsync(term);

    public async Task<Formation?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Formation?> GetWithSessionsAsync(int id) =>
        await _repo.GetWithSessionsAsync(id);

    public async Task<Formation> CreateAsync(Formation formation)
    {
        if (await _repo.CodeExistsAsync(formation.Code))
            throw new InvalidOperationException($"Le code {formation.Code} existe déjà.");
        formation.CreatedAt = DateTime.UtcNow;
        formation.UpdatedAt = DateTime.UtcNow;
        return await _repo.AddAsync(formation);
    }

    public async Task UpdateAsync(Formation formation)
    {
        formation.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(formation);
    }

    public async Task DeleteAsync(int id)
    {
        var formation = await _repo.GetByIdAsync(id);
        if (formation is not null)
        {
            formation.IsDeleted = true;
            formation.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(formation);
        }
    }

    public async Task<SessionFormation> OuvrirSessionAsync(SessionFormation session)
    {
        session.Statut = "ouverte";
        session.CreatedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SessionsFormation.AddAsync(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task FermerSessionAsync(int sessionId)
    {
        var session = await _context.SessionsFormation.FindAsync(sessionId);
        if (session is not null)
        {
            session.Statut = "fermee";
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClotureSessionAsync(int sessionId)
    {
        var session = await _context.SessionsFormation.FindAsync(sessionId);
        if (session is not null)
        {
            session.Statut = "terminee";
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountFormationsActivesAsync() =>
        await _context.SessionsFormation.CountAsync(s => s.Statut == "ouverte");

    public async Task<IEnumerable<SessionFormation>> GetSessionsOuvertesAsync()
    {
        return await _context.SessionsFormation
            .Include(s => s.Formation)
            .Include(s => s.Inscriptions)
            .Where(s => s.Statut == "ouverte")
            .ToListAsync();
    }
}
