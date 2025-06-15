using System.Linq.Expressions;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly POSContext _context;

    public SaleRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> FindAsync(Expression<Func<Sale, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        return sale;
    }

    public void Update(Sale sale)
    {
        _context.Entry(sale).State = EntityState.Modified;
    }

    public void Delete(Sale sale)
    {
        _context.Sales.Remove(sale);
    }

    public async Task<Sale?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Buyer)
            .Include(s => s.Staff)
            .Include(s => s.PaymentMethod)
            .Include(s => s.Address)
            .Include(s => s.CardType)
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Include(s => s.Discounts)
            .FirstOrDefaultAsync(s => s.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.Where(s => s.StoreId == storeId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.Where(s => s.StaffId == staffId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.Where(s => s.CustomerId == customerId).ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalSalesByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.StoreId == storeId)
            .SumAsync(s => s.TotalAmount, cancellationToken);
    }

    public async Task<decimal> GetTotalSalesByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.StaffId == staffId)
            .SumAsync(s => s.TotalAmount, cancellationToken);
    }

    public async Task<decimal> GetTotalSalesByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.BuyerId == customerId)
            .SumAsync(s => s.TotalAmount, cancellationToken);
    }
} 
