#nullable enable
using eShop.POS.Domain.AggregatesModel.StaffAggregate;
using eShop.POS.Domain.Repositories;
namespace POS.Domain.Repositories;

public interface IStaffRepository : IRepository<Staff>
{
    Task<Staff?> GetAsync(string staffId);
    Task<IEnumerable<Staff>> GetByStoreAsync(string storeId);
    Task<Staff?> GetByEmailAsync(string email);
    Task<decimal> GetTotalTipsAsync(string staffId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Staff>> GetByStoreIdAsync(string storeId);
    Task<IEnumerable<Staff>> GetByRoleAsync(string role);
    Task<Staff?> GetByPhoneAsync(string phone);
} 
