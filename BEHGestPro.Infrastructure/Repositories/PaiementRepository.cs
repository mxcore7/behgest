using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Infrastructure.Repositories;

public class PaiementRepository : Repository<Paiement>
{
    public PaiementRepository(BehGestionDbContext context) : base(context) { }

    public async Task<IEnumerable<Paiement>> GetByPeriodeAsync(DateTime debut, DateTime fin)
    {
        var all = await _context.Paiements.ToListAsync();
        return all.Where(p => p.DatePaiement >= debut && p.DatePaiement <= fin)
                  .OrderByDescending(p => p.DatePaiement);
    }

    public async Task<IEnumerable<Paiement>> GetByTypeSourceAsync(string typeSource)
    {
        return await _context.Paiements
            .Where(p => p.TypeSource == typeSource)
            .OrderByDescending(p => p.DatePaiement)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalMoisActuelAsync()
    {
        var now = DateTime.Today;
        return await _context.Paiements
            .Where(p => p.DatePaiement.Year == now.Year && p.DatePaiement.Month == now.Month)
            .SumAsync(p => p.MontantVerse);
    }

    public async Task<bool> ReferenceExistsAsync(string reference)
    {
        return await _context.Paiements.AnyAsync(p => p.Reference == reference);
    }
}
