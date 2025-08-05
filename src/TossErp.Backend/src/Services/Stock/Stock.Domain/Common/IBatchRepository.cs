using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Domain.Common;

public interface IBatchRepository : IRepository<Batch>
{
    Task<Batch?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetByItemAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetBySupplierAsync(string supplier, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetByReferenceDocumentAsync(string referenceDocumentType, string referenceDocumentNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetExpiringBatchesAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetExpiredBatchesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetByExpiryDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetByManufacturingDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<long> GetCountByItemAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<long> GetCountBySupplierAsync(string supplier, CancellationToken cancellationToken = default);
    Task<long> GetExpiringCountAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<long> GetExpiredCountAsync(CancellationToken cancellationToken = default);
} 
