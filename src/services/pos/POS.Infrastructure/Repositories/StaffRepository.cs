#nullable enable

using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.SeedWork;
using System.Linq.Expressions;

namespace POS.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly POSContext _context;

    public StaffRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Staff> AddAsync(Staff staff)
    {
        return await AddAsync(staff, CancellationToken.None);
    }

    public async Task<Staff> AddAsync(Staff staff, CancellationToken cancellationToken)
    {
        var entry = await _context.Staff.AddAsync(staff, cancellationToken);
        return entry.Entity;
    }

    public async Task<Staff?> GetByIdAsync(Guid id)
    {
        return await GetByIdAsync(id, CancellationToken.None);
    }

    public async Task<Staff?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Staff
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetAsync(Expression<Func<Staff, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Staff, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.CountAsync(predicate, cancellationToken);
    }

    public Task UpdateAsync(Staff staff)
    {
        return UpdateAsync(staff, CancellationToken.None);
    }

    public Task UpdateAsync(Staff staff, CancellationToken cancellationToken)
    {
        _context.Entry(staff).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync(id, CancellationToken.None);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var staff = await GetByIdAsync(id, cancellationToken);
        if (staff != null)
        {
            _context.Staff.Remove(staff);
        }
    }

    public Task DeleteAsync(Staff staff, CancellationToken cancellationToken)
    {
        _context.Staff.Remove(staff);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Staff.AnyAsync(s => s.Id == id, cancellationToken);
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

    public async Task<IEnumerable<Staff>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(storeId, out var storeGuid))
        {
            return Enumerable.Empty<Staff>();
        }
        
        return await _context.Staff
            .Where(s => s.StoreId == storeGuid)
            .ToListAsync(cancellationToken);
    }

    public Task<IEnumerable<Staff>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default)
    {
        // This method seems to be incorrectly typed - storeId should be Guid, not int
        // For now, return empty collection as this method signature doesn't match the domain model
        return Task.FromResult(Enumerable.Empty<Staff>());
    }

    public async Task<IEnumerable<Staff>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .Where(s => s.Role == role)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetActiveStaffAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            //.Include(s => s.Store)
            //.Include(s => s.Roles)
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);
    }

    public Task<decimal> GetTotalTipsAsync(string staffId, DateTime startDate, DateTime endDate)
    {
        // Sale entity does not have a Tips property, so return 0
        // This method would need to be implemented when Tips property is added to Sale entity
        return Task.FromResult(0m);
    }
} 
