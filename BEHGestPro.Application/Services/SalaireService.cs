using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using BEHGestPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Application.Services;

public class SalaireService
{
    private readonly SalaireRepository _repo;
    private readonly BehGestionDbContext _context;

    public SalaireService(SalaireRepository repo, BehGestionDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<Salaire>> GetByPeriodeAsync(int mois, int annee) =>
        await _repo.GetByPeriodeAsync(mois, annee);

    public async Task<IEnumerable<Salaire>> GetByEmployeAsync(int employeId) =>
        await _repo.GetByEmployeAsync(employeId);

    public async Task<Salaire?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Salaire> CreerFicheAsync(int employeId, int mois, int annee)
    {
        var existant = await _repo.GetByEmployePeriodeAsync(employeId, mois, annee);
        if (existant is not null)
            throw new InvalidOperationException($"Une fiche de paie existe déjà pour cette période.");

        var employe = await _context.Employes.FindAsync(employeId)
            ?? throw new InvalidOperationException("Employé introuvable.");

        var avancesNonRemboursees = await _context.AvancesSalaire
            .Where(a => a.EmployeId == employeId && !a.Remboursee)
            .SumAsync(a => a.Montant);

        var salaire = new Salaire
        {
            EmployeId = employeId,
            PeriodeMois = mois,
            PeriodeAnnee = annee,
            SalaireBase = employe.SalaireBase,
            AvanceDeduite = avancesNonRemboursees,
            Statut = "en_attente",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return await _repo.AddAsync(salaire);
    }

    public async Task UpdateAsync(Salaire salaire)
    {
        salaire.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(salaire);
    }

    public async Task ValiderPaiementAsync(int salaireId)
    {
        var salaire = await _repo.GetByIdAsync(salaireId)
            ?? throw new InvalidOperationException("Fiche de paie introuvable.");
        salaire.Statut = "paye";
        salaire.DatePaiement = DateTime.Today;
        salaire.UpdatedAt = DateTime.UtcNow;

        var avances = await _context.AvancesSalaire
            .Where(a => a.EmployeId == salaire.EmployeId && !a.Remboursee)
            .ToListAsync();
        foreach (var avance in avances)
        {
            avance.Remboursee = true;
            avance.SalaireId = salaireId;
            avance.UpdatedAt = DateTime.UtcNow;
        }
        await _repo.UpdateAsync(salaire);
    }

    public async Task EnregistrerAvanceAsync(AvanceSalaire avance)
    {
        avance.CreatedAt = DateTime.UtcNow;
        avance.UpdatedAt = DateTime.UtcNow;
        await _context.AvancesSalaire.AddAsync(avance);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountEnAttenteAsync() =>
        await _repo.CountEnAttenteAsync();

    public async Task DeleteAsync(int id) =>
        await _repo.DeleteAsync(id);
}
