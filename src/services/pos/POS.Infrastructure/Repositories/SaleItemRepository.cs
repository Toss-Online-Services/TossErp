using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class SaleItemRepository : ISaleItemRepository
{
    private readonly POSContext _context;

    public SaleItemRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<SaleItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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

    public async Task<SaleItem> AddAsync(SaleItem item, CancellationToken cancellationToken = default)
    {
        await _context.SaleItems.AddAsync(item, cancellationToken);
        return item;
    }

    public void Update(SaleItem saleItem)
    {
        _context.Entry(saleItem).State = EntityState.Modified;
    }

    public void Delete(SaleItem saleItem)
    {
        _context.SaleItems.Remove(saleItem);
    }

    public async Task<IEnumerable<SaleItem>> GetBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems.Where(i => i.SaleId == saleId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SaleItem>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .Include(i => i.Product)
            .Where(i => i.ProductId == productId)
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
