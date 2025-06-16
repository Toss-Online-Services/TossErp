using System.Linq.Expressions;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ISaleRepository = POS.Domain.Repositories.ISaleRepository;
using POS.Domain.Common;
using POS.Domain.SeedWork;

namespace TossErp.POS.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly POSContext _context;

    public SaleRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sale>> GetByStoreIdAsync(Guid storeId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.StoreId == storeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByStaffIdAsync(Guid staffId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.StaffId == staffId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .ToListAsync();
    }

    public async Task AddAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
    }

    public void Update(Sale sale)
    {
        _context.Entry(sale).State = EntityState.Modified;
    }

    public void Delete(Sale sale)
    {
        _context.Sales.Remove(sale);
    }

    public async Task<IEnumerable<Sale>> GetByStoreAsync(Guid storeId, DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.StoreId == storeId && s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByStaffAsync(Guid staffId, DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.StaffId == staffId && s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetOfflineSalesAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.IsOffline)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Sales.AnyAsync(s => s.Id == id);
    }

    public async Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .SumAsync(s => s.Total);
    }

    public async Task<decimal> GetTotalSalesByStoreAsync(Guid storeId)
    {
        return await _context.Sales
            .Where(s => s.StoreId == storeId)
            .SumAsync(s => s.Total);
    }

    public async Task<decimal> GetTotalSalesByStaffAsync(Guid staffId)
    {
        return await _context.Sales
            .Where(s => s.StaffId == staffId)
            .SumAsync(s => s.Total);
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

    public async Task<IEnumerable<Sale>> GetByBuyerIdAsync(int buyerId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.BuyerId == buyerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetOfflineSalesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => !s.IsOnline)
            .ToListAsync(cancellationToken);
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
