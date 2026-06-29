using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using BEHGestPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Application.Services;

public class StagiaireService
{
    private readonly StagiaireRepository _repo;
    private readonly BehGestionDbContext _context;

    public StagiaireService(StagiaireRepository repo, BehGestionDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<string> GenererMatriculeAsync()
    {
        var today = DateTime.Today.ToString("yyyyMMdd");
        var count = await _context.Stagiaires.IgnoreQueryFilters()
            .CountAsync(s => s.Matricule.StartsWith($"STG-{today}"));
        return $"STG-{today}-{(count + 1):D3}";
    }

    public async Task<IEnumerable<Stagiaire>> GetAllAsync() =>
        await _repo.GetAllWithEncadrantAsync();

    public async Task<IEnumerable<Stagiaire>> SearchAsync(string term) =>
        string.IsNullOrWhiteSpace(term) ? await _repo.GetAllWithEncadrantAsync() : await _repo.SearchAsync(term);

    public async Task<Stagiaire?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Stagiaire> CreateAsync(Stagiaire stagiaire)
    {
        if (await _repo.MatriculeExistsAsync(stagiaire.Matricule))
            throw new InvalidOperationException($"Le matricule {stagiaire.Matricule} existe déjà.");
        stagiaire.CreatedAt = DateTime.UtcNow;
        stagiaire.UpdatedAt = DateTime.UtcNow;
        return await _repo.AddAsync(stagiaire);
    }

    public async Task UpdateAsync(Stagiaire stagiaire)
    {
        stagiaire.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(stagiaire);
    }

    public async Task DeleteAsync(int id)
    {
        var stagiaire = await _repo.GetByIdAsync(id);
        if (stagiaire is not null)
        {
            stagiaire.IsDeleted = true;
            stagiaire.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(stagiaire);
        }
    }

    public async Task MettreAJourStatutsAsync()
    {
        var termines = await _context.Stagiaires
            .Where(s => s.Statut == "en_cours" && s.DateFin < DateTime.Today)
            .ToListAsync();

        foreach (var s in termines)
        {
            s.Statut = "termine";
            s.UpdatedAt = DateTime.UtcNow;
        }
        if (termines.Any()) await _context.SaveChangesAsync();
    }

    public async Task<int> CountActifsAsync() =>
        await _context.Stagiaires.CountAsync(s => s.Statut == "en_cours");
}
