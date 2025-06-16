using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using POS.Domain.Common;
using POS.Domain.SeedWork;

namespace TossErp.POS.Infrastructure.Repositories;

public class SaleItemRepository : ISaleItemRepository
{
    private readonly POSContext _context;

    public SaleItemRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<SaleItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<SaleItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SaleItem>> FindAsync(Expression<Func<SaleItem, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SaleItem saleItem, CancellationToken cancellationToken = default)
    {
        await _context.SaleItems.AddAsync(saleItem, cancellationToken);
    }

    public async Task UpdateAsync(SaleItem saleItem, CancellationToken cancellationToken = default)
    {
        _context.Entry(saleItem).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(SaleItem saleItem, CancellationToken cancellationToken = default)
    {
        _context.SaleItems.Remove(saleItem);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<SaleItem>> GetBySaleAsync(int saleId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .Include(i => i.Product)
            .Where(i => i.SaleId == saleId)
            .OrderBy(i => i.Product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SaleItem>> GetByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .Include(i => i.Product)
            .Where(i => i.ProductId == productId)
            .OrderBy(i => i.Product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SaleItem>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .Include(i => i.Product)
            .Where(i => i.StoreId == storeId)
            .OrderBy(i => i.Product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalQuantityByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .Where(i => i.ProductId == productId)
            .SumAsync(i => i.Quantity, cancellationToken);
    }

    public async Task<decimal> GetTotalAmountByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .Where(i => i.ProductId == productId)
            .SumAsync(i => i.TotalAmount, cancellationToken);
    }
} 
