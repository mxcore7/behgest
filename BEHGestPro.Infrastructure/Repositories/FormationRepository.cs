using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Infrastructure.Repositories;

public class FormationRepository : Repository<Formation>
{
    public FormationRepository(BehGestionDbContext context) : base(context) { }

    public async Task<IEnumerable<Formation>> SearchAsync(string term)
    {
        term = term.ToLower();
        return await _context.Formations
            .Where(f => f.Intitule.ToLower().Contains(term)
                     || f.Code.ToLower().Contains(term)
                     || (f.Categorie != null && f.Categorie.ToLower().Contains(term)))
            .OrderBy(f => f.Intitule)
            .ToListAsync();
    }

    public async Task<Formation?> GetWithSessionsAsync(int id)
    {
        return await _context.Formations
            .Include(f => f.Sessions.Where(s => !s.IsDeleted))
                .ThenInclude(s => s.Inscriptions.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.Apprenant)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Formation>> GetAllWithSessionCountAsync()
    {
        return await _context.Formations
            .Include(f => f.Sessions.Where(s => !s.IsDeleted))
            .OrderBy(f => f.Intitule)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _context.Formations.AnyAsync(f => f.Code == code);
    }
}
