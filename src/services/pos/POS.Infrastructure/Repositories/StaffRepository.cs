#nullable enable

using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.SeedWork;

namespace POS.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly POSContext _context;

    public StaffRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Staff> AddAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Staff.AddAsync(staff, cancellationToken);
        return entry.Entity;
    }

    public async Task<Staff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Staff.OrderBy(s => s.Name).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetAsync(Specification<Staff> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Staff.AsQueryable();
        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        _context.Entry(staff).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var staff = await _context.Staff.FindAsync(new object[] { id }, cancellationToken);
        if (staff != null)
        {
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(Specification<Staff> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Staff.AsQueryable();
        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
    }

    public async Task<Staff?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .FirstOrDefaultAsync(s => s.Phone == phone, cancellationToken);
    }

    public async Task<Staff?> GetByPINAsync(string pin)
    {
        return await _context.Staff
            .FirstOrDefaultAsync(s => s.PIN == pin);
    }

    public async Task<IEnumerable<Staff>> GetByStoreAsync(string storeId)
    {
        return await _context.Staff
            .Where(s => s.StoreId.ToString() == storeId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Staff>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .Where(s => s.StoreId == storeId)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetActiveStaffAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .Where(s => s.Role == role)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalTipsAsync(string staffId, DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.StaffId.ToString() == staffId && 
                       s.CreatedAt >= startDate && 
                       s.CreatedAt <= endDate)
            .SumAsync(s => s.TipAmount);
    }

    // Overloads without CancellationToken
    public Task<Staff> AddAsync(Staff staff) => AddAsync(staff, default);
    public Task UpdateAsync(Staff staff) => UpdateAsync(staff, default);
    public Task DeleteAsync(Guid id) => DeleteAsync(id, default);
    public Task<Staff?> GetByIdAsync(Guid id) => GetByIdAsync(id, default);
    public Task<Staff?> GetByEmailAsync(string email) => GetByEmailAsync(email, default);
    public Task<Staff?> GetByPhoneAsync(string phone) => GetByPhoneAsync(phone, default);
    public Task<IEnumerable<Staff>> GetByStoreIdAsync(int storeId) => GetByStoreIdAsync(storeId, default);
    public Task<IEnumerable<Staff>> GetActiveStaffAsync() => GetActiveStaffAsync(default);
    public Task<IEnumerable<Staff>> GetByRoleAsync(string role) => GetByRoleAsync(role, default);
} 
