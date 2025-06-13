using eShop.POS.Domain.AggregatesModel.SyncLogAggregate;
using eShop.POS.Domain.Repositories;

namespace eShop.POS.Domain.Repositories;

public interface ISyncLogRepository : IRepository<SyncLog>
{
    Task<IEnumerable<SyncLog>> GetByStoreIdAsync(string storeId);
    Task<IEnumerable<SyncLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<SyncLog>> GetByStatusAsync(SyncStatus status);
} 
