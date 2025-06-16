using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.StaffAggregate;

public interface IStaffRepository : IRepository<Staff>
{
    Task<Staff?> GetByIdAsync(Guid id);
    Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Staff?> GetByPINAsync(string pin);
    Task<Staff?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<IEnumerable<Staff>> GetByStoreAsync(string storeId);
    Task<IEnumerable<Staff>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Staff>> GetByRoleAsync(string role);
    Task<IEnumerable<Staff>> GetActiveStaffAsync();
    Task<decimal> GetTotalTipsAsync(string staffId, DateTime startDate, DateTime endDate);
    Task<Staff> AddAsync(Staff staff);
    Task UpdateAsync(Staff staff);
    Task DeleteAsync(Guid id);
} 
