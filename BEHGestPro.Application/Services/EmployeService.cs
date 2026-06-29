using BEHGestPro.Domain.Entities;
using BEHGestPro.Infrastructure.Data;
using BEHGestPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Application.Services;

public class EmployeService
{
    private readonly EmployeRepository _repo;
    private readonly BehGestionDbContext _context;

    public EmployeService(EmployeRepository repo, BehGestionDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<string> GenererMatriculeAsync()
    {
        var today = DateTime.Today.ToString("yyyyMMdd");
        var count = await _context.Employes.IgnoreQueryFilters()
            .CountAsync(e => e.Matricule.StartsWith($"EMP-{today}"));
        return $"EMP-{today}-{(count + 1):D3}";
    }

    public async Task<IEnumerable<Employe>> GetAllAsync() => await _repo.GetAllAsync();
    public async Task<IEnumerable<Employe>> GetActifsAsync() => await _repo.GetActifsAsync();
    public async Task<IEnumerable<Employe>> SearchAsync(string term) =>
        string.IsNullOrWhiteSpace(term) ? await _repo.GetAllAsync() : await _repo.SearchAsync(term);
    public async Task<Employe?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Employe> CreateAsync(Employe employe)
    {
        if (await _repo.MatriculeExistsAsync(employe.Matricule))
            throw new InvalidOperationException($"Le matricule {employe.Matricule} existe déjà.");
        employe.CreatedAt = DateTime.UtcNow;
        employe.UpdatedAt = DateTime.UtcNow;
        return await _repo.AddAsync(employe);
    }

    public async Task UpdateAsync(Employe employe)
    {
        employe.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(employe);
    }

    public async Task DeleteAsync(int id)
    {
        var employe = await _repo.GetByIdAsync(id);
        if (employe is not null)
        {
            employe.IsDeleted = true;
            employe.Actif = false;
            employe.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(employe);
        }
    }
}
