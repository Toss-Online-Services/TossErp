using Microsoft.EntityFrameworkCore;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Infrastructure.Persistence;

namespace TossErp.Sales.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework implementation of ITillRepository
/// </summary>
public class TillRepository : ITillRepository
{
    private readonly SalesDbContext _context;

    public TillRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Till?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tills
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Till>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tills
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Till>> GetByStatusAsync(Domain.Enums.TillStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Tills
            .Where(t => t.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<Till> AddAsync(Till till, CancellationToken cancellationToken = default)
    {
        _context.Tills.Add(till);
        await _context.SaveChangesAsync(cancellationToken);
        return till;
    }

    public async Task<Till> UpdateAsync(Till till, CancellationToken cancellationToken = default)
    {
        _context.Tills.Update(till);
        await _context.SaveChangesAsync(cancellationToken);
        return till;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var till = await _context.Tills.FindAsync(new object[] { id }, cancellationToken);
        if (till != null)
        {
            _context.Tills.Remove(till);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
