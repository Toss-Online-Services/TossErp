using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Common.Interfaces;

/// <summary>
/// Repository interface for StockValuationSnapshot entities
/// </summary>
public interface IStockValuationSnapshotRepository
{
    Task<StockValuationSnapshot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockValuationSnapshot>> GetByDateAsync(DateTime snapshotDate, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockValuationSnapshot>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockValuationSnapshot>> GetByWarehouseAsync(string warehouseCode, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockValuationSnapshot>> GetByItemCodeAsync(string itemCode, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockValuationSnapshot>> GetByValuationMethodAsync(ValuationMethod method, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockValuationSnapshot>> GetLatestSnapshotsAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<StockValuationSnapshot> AddAsync(StockValuationSnapshot snapshot, CancellationToken cancellationToken = default);
    Task<StockValuationSnapshot> UpdateAsync(StockValuationSnapshot snapshot, CancellationToken cancellationToken = default);
    Task DeleteAsync(StockValuationSnapshot snapshot, CancellationToken cancellationToken = default);
}

