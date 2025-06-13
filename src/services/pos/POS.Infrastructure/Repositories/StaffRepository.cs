using Microsoft.EntityFrameworkCore;
using eShop.POS.Domain.AggregatesModel.StaffAggregate;
using eShop.POS.Domain.Repositories;
using eShop.POS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShop.POS.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly POSContext _context;

    public StaffRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Staff?> GetByIdAsync(string id)
    {
        return await _context.Staff.FindAsync(id);
    }

    public async Task<IEnumerable<Staff>> GetAllAsync()
    {
        return await _context.Staff.ToListAsync();
    }

    public async Task AddAsync(Staff staff)
    {
        await _context.Staff.AddAsync(staff);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Staff staff)
    {
        _context.Entry(staff).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Staff staff)
    {
        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Staff.AnyAsync(s => s.Id == id);
    }

    public async Task<Staff?> GetByCodeAsync(string code)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<Staff?> GetByEmailAsync(string email)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Staff?> GetByPhoneAsync(string phone)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.Phone == phone);
    }

    public async Task<IEnumerable<Staff>> GetByStoreIdAsync(string storeId)
    {
        return await _context.Staff
            .Where(s => s.StoreId == storeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Staff>> GetByRoleAsync(string role)
    {
        return await _context.Staff
            .Where(s => s.Role == role)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalTipsAsync(string staffId, DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.StaffId == staffId && 
                       s.TipAmount > 0 && 
                       s.CreatedAt >= startDate && 
                       s.CreatedAt <= endDate)
            .SumAsync(s => s.TipAmount);
    }
} 
