using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using BEHGestPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Application.Services;

public class ApprenantService
{
    private readonly ApprenantRepository _repo;
    private readonly BehGestionDbContext _context;

    public ApprenantService(ApprenantRepository repo, BehGestionDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<string> GenererMatriculeAsync()
    {
        var today = DateTime.Today.ToString("yyyyMMdd");
        var count = await _context.Apprenants.IgnoreQueryFilters()
            .CountAsync(a => a.Matricule.StartsWith($"APP-{today}"));
        return $"APP-{today}-{(count + 1):D3}";
    }

    public async Task<IEnumerable<Apprenant>> GetAllAsync() =>
        await _repo.GetAllAsync();

    public async Task<IEnumerable<Apprenant>> SearchAsync(string term) =>
        string.IsNullOrWhiteSpace(term) ? await _repo.GetAllAsync() : await _repo.SearchAsync(term);

    public async Task<Apprenant?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Apprenant?> GetWithInscriptionsAsync(int id) =>
        await _repo.GetWithInscriptionsAsync(id);

    public async Task<Apprenant> CreateAsync(Apprenant apprenant)
    {
        if (await _repo.MatriculeExistsAsync(apprenant.Matricule))
            throw new InvalidOperationException($"Le matricule {apprenant.Matricule} existe déjà.");
        apprenant.CreatedAt = DateTime.UtcNow;
        apprenant.UpdatedAt = DateTime.UtcNow;
        return await _repo.AddAsync(apprenant);
    }

    public async Task UpdateAsync(Apprenant apprenant)
    {
        apprenant.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(apprenant);
    }

    public async Task DeleteAsync(int id)
    {
        var apprenant = await _repo.GetByIdAsync(id);
        if (apprenant is not null)
        {
            apprenant.IsDeleted = true;
            apprenant.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(apprenant);
        }
    }

    public async Task<int> CountAsync() =>
        await _context.Apprenants.CountAsync();

    public async Task<bool> InscrireAsync(int apprenantId, int sessionId)
    {
        var doublon = await _context.InscriptionsFormation
            .AnyAsync(i => i.ApprenantId == apprenantId && i.SessionId == sessionId);
        if (doublon) return false;

        var session = await _context.SessionsFormation
            .Include(s => s.Inscriptions)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null || session.Statut != "ouverte" || session.EstComplete) return false;

        var inscription = new InscriptionFormation
        {
            ApprenantId = apprenantId,
            SessionId = sessionId,
            DateInscription = DateTime.Today,
            Statut = "inscrit",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.InscriptionsFormation.AddAsync(inscription);
        await _context.SaveChangesAsync();
        return true;
    }
}
