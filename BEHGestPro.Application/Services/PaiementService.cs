using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using BEHGestPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Application.Services;

public class PaiementService
{
    private readonly PaiementRepository _repo;
    private readonly BehGestionDbContext _context;

    public PaiementService(PaiementRepository repo, BehGestionDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<string> GenererReferenceAsync()
    {
        var annee = DateTime.Today.Year;
        var count = await _context.Paiements.IgnoreQueryFilters()
            .CountAsync(p => p.Reference.StartsWith($"PAY-{annee}"));
        return $"PAY-{annee}-{(count + 1):D5}";
    }

    public async Task<IEnumerable<Paiement>> GetAllAsync() =>
        await _repo.GetAllAsync();

    public async Task<Paiement?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Paiement> CreateAsync(Paiement paiement)
    {
        if (await _repo.ReferenceExistsAsync(paiement.Reference))
            throw new InvalidOperationException($"La référence {paiement.Reference} existe déjà.");
        paiement.Statut = paiement.MontantVerse >= paiement.MontantTotal ? "complet" : "partiel";
        paiement.CreatedAt = DateTime.UtcNow;
        paiement.UpdatedAt = DateTime.UtcNow;
        return await _repo.AddAsync(paiement);
    }

    public async Task UpdateAsync(Paiement paiement)
    {
        paiement.Statut = paiement.MontantVerse >= paiement.MontantTotal ? "complet" : "partiel";
        paiement.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(paiement);
    }

    public async Task AjouterVersementAsync(int paiementId, decimal montantSupplementaire)
    {
        var paiement = await _repo.GetByIdAsync(paiementId)
            ?? throw new InvalidOperationException("Paiement introuvable.");
        paiement.MontantVerse += montantSupplementaire;
        if (paiement.MontantVerse >= paiement.MontantTotal)
        {
            paiement.MontantVerse = paiement.MontantTotal;
            paiement.Statut = "complet";
        }
        paiement.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(paiement);
    }

    public async Task DeleteAsync(int id)
    {
        var paiement = await _repo.GetByIdAsync(id);
        if (paiement is not null)
        {
            paiement.IsDeleted = true;
            paiement.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(paiement);
        }
    }

    public async Task<decimal> GetTotalMoisActuelAsync() =>
        await _repo.GetTotalMoisActuelAsync();

    public async Task<int> CountPartielsAsync() =>
        await _context.Paiements.CountAsync(p => p.Statut == "partiel");
}
