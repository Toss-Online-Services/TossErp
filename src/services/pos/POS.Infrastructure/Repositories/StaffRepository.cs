using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.Common;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly POSContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public StaffRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Staff> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Staff.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Staff>> FindAsync(Expression<Func<Staff, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Staff> AddAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        await _context.Staff.AddAsync(staff, cancellationToken);
        return staff;
    }

    public void Update(Staff staff)
    {
        _context.Entry(staff).State = EntityState.Modified;
    }

    public void Delete(Staff staff)
    {
        _context.Staff.Remove(staff);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Staff.AnyAsync(s => s.Id == id);
    }

    public async Task<Staff?> GetByCodeAsync(string code)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<Staff> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
    }

    public async Task<Staff> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.Phone == phone, cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.Where(s => s.StoreId == storeId).ToListAsync(cancellationToken);
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
