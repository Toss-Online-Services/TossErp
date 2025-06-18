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
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetAsync(Expression<Func<Sale, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(predicate)
            .OrderBy(s => s.CreatedAt)
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

    public async Task<int> CountAsync(Expression<Func<Sale, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.CountAsync(predicate, cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Sale> AddAsync(Sale sale) => AddAsync(sale, default);
    public Task UpdateAsync(Sale sale) => UpdateAsync(sale, default);
    public Task DeleteAsync(Sale sale) => DeleteAsync(sale, default);

    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber);

        return sale;
    }

    public async Task<IEnumerable<Sale>> GetByCustomerAsync(Guid customerId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByStoreAsync(Guid storeId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.StoreId == storeId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByStatusAsync(SaleStatus status)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public void Update(Sale sale)
    {
        _context.Entry(sale).State = EntityState.Modified;
    }

    public void Delete(Sale sale)
    {
        _context.Sales.Remove(sale);
    }
} 
