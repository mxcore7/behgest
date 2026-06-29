using BEHGestPro.Domain.Interfaces;
using BEHGestPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BEHGestPro.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly BehGestionDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(BehGestionDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) =>
        await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public virtual async Task<T> AddAsync(T entity)
    {
        var entry = await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
