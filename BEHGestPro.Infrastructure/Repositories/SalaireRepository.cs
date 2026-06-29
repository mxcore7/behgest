using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Infrastructure.Repositories;

public class SalaireRepository : Repository<Salaire>
{
    public SalaireRepository(BehGestionDbContext context) : base(context) { }

    public async Task<IEnumerable<Salaire>> GetByPeriodeAsync(int mois, int annee)
    {
        return await _context.Salaires
            .Include(s => s.Employe)
            .Where(s => s.PeriodeMois == mois && s.PeriodeAnnee == annee)
            .OrderBy(s => s.Employe!.Nom)
            .ToListAsync();
    }

    public async Task<IEnumerable<Salaire>> GetByEmployeAsync(int employeId)
    {
        return await _context.Salaires
            .Include(s => s.Employe)
            .Where(s => s.EmployeId == employeId)
            .OrderByDescending(s => s.PeriodeAnnee).ThenByDescending(s => s.PeriodeMois)
            .ToListAsync();
    }

    public async Task<Salaire?> GetByEmployePeriodeAsync(int employeId, int mois, int annee)
    {
        return await _context.Salaires
            .Include(s => s.Employe)
            .FirstOrDefaultAsync(s => s.EmployeId == employeId && s.PeriodeMois == mois && s.PeriodeAnnee == annee);
    }

    public async Task<int> CountEnAttenteAsync()
    {
        return await _context.Salaires.CountAsync(s => s.Statut == "en_attente");
    }
}
