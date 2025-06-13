#nullable enable
using eShop.POS.Domain.AggregatesModel.StaffAggregate;

namespace eShop.POS.Domain.Repositories;

public interface IStaffRepository : IRepository<Staff>
{
    Task<Staff?> GetAsync(string staffId);
    Task<IEnumerable<Staff>> GetByStoreAsync(string storeId);
    Task<Staff?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string staffId);
    Task<decimal> GetTotalTipsAsync(string staffId, DateTime startDate, DateTime endDate);
} 
