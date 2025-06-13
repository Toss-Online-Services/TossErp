using Microsoft.EntityFrameworkCore;
using eShop.POS.Domain.AggregatesModel.SaleAggregate;
using eShop.POS.Domain.Repositories;
using eShop.POS.Infrastructure.Data;
using ISaleRepository = eShop.POS.Domain.Repositories.ISaleRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShop.POS.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly POSContext _context;

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
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Sale sale)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Sale sale)
    {
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();
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

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Sales.AnyAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sale>> GetByStoreAsync(int storeId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.StoreId == storeId.ToString())
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByStaffAsync(int staffId)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.StaffId == staffId.ToString())
            .ToListAsync();
    }

    public async Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .SumAsync(s => s.TotalAmount);
    }

    public async Task<decimal> GetTotalSalesByStoreAsync(int storeId)
    {
        return await _context.Sales
            .Where(s => s.StoreId == storeId.ToString())
            .SumAsync(s => s.TotalAmount);
    }

    public async Task<decimal> GetTotalSalesByStaffAsync(int staffId)
    {
        return await _context.Sales
            .Where(s => s.StaffId == staffId.ToString())
            .SumAsync(s => s.TotalAmount);
    }

    public async Task DeleteAsync(int saleId)
    {
        var sale = await _context.Sales.FindAsync(saleId);
        if (sale != null)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }
    }
} 
