#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TossErp.POS.Domain.AggregatesModel.StaffAggregate;
using TossErp.POS.Domain.Common;
using TossErp.POS.Domain.Repositories;
namespace TossErp.POS.Domain.Repositories;

public interface IStaffRepository : IRepository<Staff>
{
    Task<Staff?> GetAsync(string staffId);
    Task<IEnumerable<Staff>> GetByStoreAsync(string storeId);
    Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalTipsAsync(string staffId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Staff>> GetByStoreIdAsync(string storeId);
    Task<IEnumerable<Staff>> GetByRoleAsync(string role);
    Task<Staff?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Staff>> GetAllAsync(CancellationToken cancellationToken = default);
} 
