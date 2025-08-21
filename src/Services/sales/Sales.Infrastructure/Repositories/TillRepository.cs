using Microsoft.EntityFrameworkCore;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Infrastructure.Data;

namespace TossErp.Sales.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Till aggregates
/// </summary>
public class TillRepository : ITillRepository
{
    private readonly SalesDbContext _context;

    public TillRepository(SalesDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Till?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tills
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Till?> GetByTillNumberAsync(string tillNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Tills
            .FirstOrDefaultAsync(t => t.TillNumber == tillNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Till>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tills
            .OrderBy(t => t.TillNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Till>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tills
            .Where(t => t.Status == Domain.Enums.TillStatus.Open || t.Status == Domain.Enums.TillStatus.Idle)
            .OrderBy(t => t.TillNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Till>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        return await _context.Tills
            .Where(t => t.Location == location)
            .OrderBy(t => t.TillNumber)
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
        var till = await GetByIdAsync(id, cancellationToken);
        if (till != null)
        {
            _context.Tills.Remove(till);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tills.AnyAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<bool> TillNumberExistsAsync(string tillNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Tills
            .AnyAsync(t => t.TillNumber == tillNumber, cancellationToken);
    }

    public async Task<string> GetNextTillNumberAsync(CancellationToken cancellationToken = default)
    {
        var lastTillNumber = await _context.Tills
            .OrderByDescending(t => EF.Property<DateTime>(t, "CreatedDate"))
            .Select(t => t.TillNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(lastTillNumber))
        {
            return "TILL-001";
        }

        // Extract number part and increment
        var numberPart = lastTillNumber.Split('-').LastOrDefault();
        if (int.TryParse(numberPart, out var number))
        {
            return $"TILL-{(number + 1):D3}";
        }

        return "TILL-001";
    }

    public IQueryable<Till> Query()
    {
        return _context.Tills;
    }
}
