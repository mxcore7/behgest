using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Infrastructure.Repositories;

public class StagiaireRepository : Repository<Stagiaire>
{
    public StagiaireRepository(BehGestionDbContext context) : base(context) { }

    public async Task<IEnumerable<Stagiaire>> SearchAsync(string term)
    {
        term = term.ToLower();
        return await _context.Stagiaires
            .Include(s => s.Encadrant)
            .Where(s => s.Nom.ToLower().Contains(term)
                     || s.Prenom.ToLower().Contains(term)
                     || s.Matricule.ToLower().Contains(term)
                     || (s.Etablissement != null && s.Etablissement.ToLower().Contains(term)))
            .OrderBy(s => s.Nom)
            .ToListAsync();
    }

    public async Task<IEnumerable<Stagiaire>> GetAllWithEncadrantAsync()
    {
        return await _context.Stagiaires
            .Include(s => s.Encadrant)
            .OrderBy(s => s.Nom).ThenBy(s => s.Prenom)
            .ToListAsync();
    }

    public async Task<bool> MatriculeExistsAsync(string matricule)
    {
        return await _context.Stagiaires.AnyAsync(s => s.Matricule == matricule);
    }
}
