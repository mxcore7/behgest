using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Infrastructure.Repositories;

public class ApprenantRepository : Repository<Apprenant>
{
    public ApprenantRepository(BehGestionDbContext context) : base(context) { }

    public async Task<IEnumerable<Apprenant>> SearchAsync(string term)
    {
        term = term.ToLower();
        return await _context.Apprenants
            .Where(a => a.Nom.ToLower().Contains(term)
                     || a.Prenom.ToLower().Contains(term)
                     || a.Matricule.ToLower().Contains(term)
                     || (a.Email != null && a.Email.ToLower().Contains(term)))
            .OrderBy(a => a.Nom).ThenBy(a => a.Prenom)
            .ToListAsync();
    }

    public async Task<IEnumerable<Apprenant>> GetByFormationAsync(int formationId)
    {
        return await _context.Apprenants
            .Where(a => a.Inscriptions.Any(i => i.Session!.FormationId == formationId))
            .Include(a => a.Inscriptions).ThenInclude(i => i.Session)
            .OrderBy(a => a.Nom)
            .ToListAsync();
    }

    public async Task<Apprenant?> GetWithInscriptionsAsync(int id)
    {
        return await _context.Apprenants
            .Include(a => a.Inscriptions)
                .ThenInclude(i => i.Session)
                .ThenInclude(s => s!.Formation)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> MatriculeExistsAsync(string matricule)
    {
        return await _context.Apprenants.AnyAsync(a => a.Matricule == matricule);
    }
}
