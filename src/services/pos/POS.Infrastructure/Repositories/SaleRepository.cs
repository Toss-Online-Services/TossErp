#nullable enable

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.SeedWork;
using POS.Infrastructure.EntityConfigurations;

namespace POS.Infrastructure.Repositories;

public class SaleRepository : IRepository<Sale>
{
    private readonly POSContext _context;

    public SaleRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Sales.AddAsync(sale, cancellationToken);
        return entry.Entity;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.SaleItems)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetAsync(Specification<Sale> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.OrderByDescending(s => s.SaleDate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetListAsync(Expression<Func<Sale, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.SaleItems)
            .Where(predicate)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Entry(sale).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Remove(sale);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales.FindAsync(new object[] { id }, cancellationToken);
        if (sale != null)
        {
            _context.Sales.Remove(sale);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(Specification<Sale> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.CountAsync(cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Sale> AddAsync(Sale sale) => AddAsync(sale, default);
    public Task UpdateAsync(Sale sale) => UpdateAsync(sale, default);
    public Task DeleteAsync(Sale sale) => DeleteAsync(sale, default);
} 
