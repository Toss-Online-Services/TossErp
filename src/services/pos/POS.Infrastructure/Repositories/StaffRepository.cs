using System.Linq.Expressions;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using IStaffRepository = POS.Domain.Repositories.IStaffRepository;

namespace TossErp.POS.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly POSContext _context;

    public StaffRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Staff?> GetAsync(string staffId)
    {
        return await _context.Staff
            .FirstOrDefaultAsync(s => s.StaffId == staffId);
    }

    public async Task<IEnumerable<Staff>> GetByStoreAsync(string storeId)
    {
        return await _context.Staff
            .Where(s => s.StoreId == storeId)
            .ToListAsync();
    }

    public async Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
    }

    public async Task<decimal> GetTotalTipsAsync(string staffId, DateTime startDate, DateTime endDate)
    {
        return await _context.Staff
            .Where(s => s.StaffId == staffId)
            .SelectMany(s => s.Tips)
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .SumAsync(t => t.Amount);
    }

    public async Task<IEnumerable<Staff>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .Where(s => s.StoreId == storeId.ToString())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetByRoleAsync(string role)
    {
        return await _context.Staff
            .Where(s => s.Role == role)
            .ToListAsync();
    }

    public async Task<Staff?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .FirstOrDefaultAsync(s => s.Phone == phone, cancellationToken);
    }

    public async Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .ToListAsync(cancellationToken);
    }

    public async Task<Staff?> FindAsync(Expression<Func<Staff, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(Staff entity, CancellationToken cancellationToken = default)
    {
        await _context.Staff.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Staff entity, CancellationToken cancellationToken = default)
    {
        _context.Staff.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var staff = await _context.Staff.FindAsync(new object[] { id }, cancellationToken);
        if (staff != null)
        {
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
} 
