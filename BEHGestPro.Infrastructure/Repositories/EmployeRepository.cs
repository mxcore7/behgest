using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Infrastructure.Repositories;

public class EmployeRepository : Repository<Employe>
{
    public EmployeRepository(BehGestionDbContext context) : base(context) { }

    public async Task<IEnumerable<Employe>> SearchAsync(string term)
    {
        term = term.ToLower();
        return await _context.Employes
            .Where(e => e.Nom.ToLower().Contains(term)
                     || e.Prenom.ToLower().Contains(term)
                     || e.Matricule.ToLower().Contains(term)
                     || (e.Poste != null && e.Poste.ToLower().Contains(term)))
            .OrderBy(e => e.Nom)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employe>> GetActifsAsync()
    {
        return await _context.Employes
            .Where(e => e.Actif)
            .OrderBy(e => e.Nom).ThenBy(e => e.Prenom)
            .ToListAsync();
    }

    public async Task<bool> MatriculeExistsAsync(string matricule)
    {
        return await _context.Employes.AnyAsync(e => e.Matricule == matricule);
    }
}
