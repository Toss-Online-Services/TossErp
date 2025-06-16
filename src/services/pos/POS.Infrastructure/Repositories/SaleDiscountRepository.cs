using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using POS.Domain.Common;
using POS.Domain.SeedWork;

namespace TossErp.POS.Infrastructure.Repositories;

public class SaleDiscountRepository : ISaleDiscountRepository
{
    private readonly POSContext _context;

    public SaleDiscountRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<SaleDiscount?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SaleDiscounts.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<SaleDiscount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaleDiscounts.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SaleDiscount>> FindAsync(Expression<Func<SaleDiscount, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SaleDiscounts.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SaleDiscount saleDiscount, CancellationToken cancellationToken = default)
    {
        await _context.SaleDiscounts.AddAsync(saleDiscount, cancellationToken);
    }

    public async Task UpdateAsync(SaleDiscount saleDiscount, CancellationToken cancellationToken = default)
    {
        _context.Entry(saleDiscount).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(SaleDiscount saleDiscount, CancellationToken cancellationToken = default)
    {
        _context.SaleDiscounts.Remove(saleDiscount);
        await Task.CompletedTask;
    }

    public async Task<SaleDiscount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.SaleDiscounts
            .FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<SaleDiscount>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleDiscounts
            .Where(d => d.StoreId == storeId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SaleDiscount>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.SaleDiscounts
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SaleDiscount>> GetBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleDiscounts.Where(d => d.SaleId == saleId).ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalDiscountBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleDiscounts
            .Where(d => d.SaleId == saleId)
            .SumAsync(d => d.Amount, cancellationToken);
    }
} 
