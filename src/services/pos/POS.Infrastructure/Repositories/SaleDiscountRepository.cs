using System.Linq.Expressions;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class SaleDiscountRepository : ISaleDiscountRepository
{
    private readonly POSContext _context;

    public SaleDiscountRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<SaleDiscount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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

    public async Task<SaleDiscount> AddAsync(SaleDiscount saleDiscount, CancellationToken cancellationToken = default)
    {
        await _context.SaleDiscounts.AddAsync(saleDiscount, cancellationToken);
        return saleDiscount;
    }

    public void Update(SaleDiscount saleDiscount)
    {
        _context.Entry(saleDiscount).State = EntityState.Modified;
    }

    public void Delete(SaleDiscount saleDiscount)
    {
        _context.SaleDiscounts.Remove(saleDiscount);
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
