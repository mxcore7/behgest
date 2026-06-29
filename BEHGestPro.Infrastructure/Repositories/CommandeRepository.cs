using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Infrastructure.Repositories;

public class CommandeRepository : Repository<Commande>
{
    public CommandeRepository(BehGestionDbContext context) : base(context) { }

    public async Task<IEnumerable<Commande>> SearchAsync(string term)
    {
        term = term.ToLower();
        return await _context.Commandes
            .Include(c => c.Employe)
            .Where(c => c.ClientNom.ToLower().Contains(term)
                     || c.Numero.ToLower().Contains(term)
                     || c.TypeService.ToLower().Contains(term))
            .OrderByDescending(c => c.DateCommande)
            .ToListAsync();
    }

    public async Task<IEnumerable<Commande>> GetAllWithEmployeAsync()
    {
        return await _context.Commandes
            .Include(c => c.Employe)
            .OrderByDescending(c => c.DateCommande)
            .ToListAsync();
    }

    public async Task<IEnumerable<Commande>> GetRecentAsync(int count = 5)
    {
        return await _context.Commandes
            .Include(c => c.Employe)
            .OrderByDescending(c => c.DateCommande)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> NumeroExistsAsync(string numero)
    {
        return await _context.Commandes.AnyAsync(c => c.Numero == numero);
    }
}
