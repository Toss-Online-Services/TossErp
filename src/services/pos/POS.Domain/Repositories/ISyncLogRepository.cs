#nullable enable

namespace POS.Domain.Repositories;

public interface ISyncLogRepository : IRepository<SyncLog>
{
    new Task<SyncLog> AddAsync(SyncLog syncLog);
    new Task<SyncLog?> GetByIdAsync(string id);
    Task<IEnumerable<SyncLog>> GetByStoreIdAsync(string storeId);
    Task<IEnumerable<SyncLog>> GetPendingSyncsAsync();
    new Task UpdateAsync(SyncLog syncLog);
}

public class SyncLog : Entity
{
    public required string StoreId { get; set; }
    public DateTime SyncDate { get; set; }
    public SyncStatus Status { get; set; }
    public required string ErrorMessage { get; set; }
}

public enum SyncStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
} 
