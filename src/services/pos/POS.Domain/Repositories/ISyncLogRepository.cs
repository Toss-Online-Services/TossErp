namespace eShop.POS.Domain.Repositories;

public interface ISyncLogRepository
{
    Task RecordSync(
        int saleId,
        string storeId,
        DateTime syncedAt,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SyncLog>> GetPendingSyncsAsync(
        string storeId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SyncLog>> GetFailedSyncsAsync(
        string storeId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<DateTime?> GetLastSyncTimeAsync(
        string storeId,
        CancellationToken cancellationToken = default);

    Task<int> GetPendingSyncCountAsync(
        string storeId,
        CancellationToken cancellationToken = default);
}

public class SyncLog
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public string StoreId { get; set; }
    public DateTime SyncedAt { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
} 
