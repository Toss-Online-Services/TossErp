using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Infrastructure.Data;

namespace TossErp.Stock.Infrastructure.Repositories;

public class UnitOfMeasureRepository : IUnitOfMeasureRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<UnitOfMeasure> _dbSet;

    public UnitOfMeasureRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<UnitOfMeasure>();
    }

    public async Task<UnitOfMeasure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<UnitOfMeasure>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public async Task<UnitOfMeasure> AddAsync(UnitOfMeasure entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(UnitOfMeasure entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(UnitOfMeasure entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await GetByIdAsync(id, cancellationToken) != null;

    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        => await _dbSet.LongCountAsync(cancellationToken);

    public async Task<UnitOfMeasure?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
} 
