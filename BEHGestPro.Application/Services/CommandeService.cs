using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using BEHGestPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Application.Services;

public class CommandeService
{
    private readonly CommandeRepository _repo;
    private readonly BehGestionDbContext _context;

    public CommandeService(CommandeRepository repo, BehGestionDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<string> GenererNumeroAsync()
    {
        var today = DateTime.Today.ToString("yyyyMMdd");
        var count = await _context.Commandes.IgnoreQueryFilters()
            .CountAsync(c => c.Numero.StartsWith($"CMD-{today}"));
        return $"CMD-{today}-{(count + 1):D3}";
    }

    public async Task<IEnumerable<Commande>> GetAllAsync() =>
        await _repo.GetAllWithEmployeAsync();

    public async Task<IEnumerable<Commande>> SearchAsync(string term) =>
        string.IsNullOrWhiteSpace(term) ? await _repo.GetAllWithEmployeAsync() : await _repo.SearchAsync(term);

    public async Task<Commande?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<IEnumerable<Commande>> GetRecentAsync(int count = 5) =>
        await _repo.GetRecentAsync(count);

    public async Task<Commande> CreateAsync(Commande commande)
    {
        if (await _repo.NumeroExistsAsync(commande.Numero))
            throw new InvalidOperationException($"Le numéro {commande.Numero} existe déjà.");
        commande.Statut = "en_attente";
        commande.CreatedAt = DateTime.UtcNow;
        commande.UpdatedAt = DateTime.UtcNow;
        return await _repo.AddAsync(commande);
    }

    public async Task UpdateAsync(Commande commande)
    {
        commande.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(commande);
    }

    public async Task ChangerStatutAsync(int commandeId, string nouveauStatut)
    {
        var commande = await _repo.GetByIdAsync(commandeId)
            ?? throw new InvalidOperationException("Commande introuvable.");
        commande.Statut = nouveauStatut;
        if (nouveauStatut == "livree") commande.DateLivraisonReelle = DateTime.Today;
        commande.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(commande);
    }

    public async Task DeleteAsync(int id)
    {
        var commande = await _repo.GetByIdAsync(id);
        if (commande is not null)
        {
            commande.IsDeleted = true;
            commande.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(commande);
        }
    }

    public async Task<int> CountEnCoursAsync() =>
        await _context.Commandes.CountAsync(c => c.Statut == "en_cours" || c.Statut == "en_attente");
}
