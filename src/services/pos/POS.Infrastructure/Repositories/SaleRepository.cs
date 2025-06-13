using ISaleRepository = TossErp.POS.Domain.Repositories.ISaleRepository;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Common;
using TossErp.POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TossErp.POS.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly POSContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public SaleRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Sale?> GetByIdAsync(string id)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Include(s => s.Discounts)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sale>> GetByStoreIdAsync(string storeId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Include(s => s.Discounts)
            .Where(s => s.StoreId == storeId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByStaffIdAsync(string staffId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Include(s => s.Discounts)
            .Where(s => s.StaffId == staffId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(string customerId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Include(s => s.Discounts)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Include(s => s.Discounts)
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
    }

    public void Update(Sale sale)
    {
        _context.Sales.Update(sale);
    }

    public void Delete(Sale sale)
    {
        _context.Sales.Remove(sale);
    }

    public async Task<IEnumerable<Sale>> GetByStoreAsync(string storeId, DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Include(s => s.Discounts)
            .Where(s => s.StoreId == storeId && s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByStaffAsync(string staffId, DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Include(s => s.Discounts)
            .Where(s => s.StaffId == staffId && s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetOfflineSalesAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Include(s => s.Discounts)
            .Where(s => s.IsOffline && !s.IsSynced)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Sales.AnyAsync(s => s.Id == id);
    }

    public async Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .SumAsync(s => s.TotalAmount);
    }

    public async Task<decimal> GetTotalSalesByStoreAsync(string storeId)
    {
        return await _context.Sales
            .Where(s => s.StoreId == storeId)
            .SumAsync(s => s.TotalAmount);
    }

    public async Task<decimal> GetTotalSalesByStaffAsync(string staffId)
    {
        return await _context.Sales
            .Where(s => s.StaffId == staffId)
            .SumAsync(s => s.TotalAmount);
    }
} 
