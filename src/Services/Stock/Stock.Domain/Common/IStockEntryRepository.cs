using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Domain.Common;

/// <summary>
/// Repository interface for StockEntry Aggregate
/// </summary>
public interface IStockEntryRepository : IRepository<StockEntryAggregate>
{
    /// <summary>
    /// Get stock entry by entry number
    /// </summary>
    Task<StockEntryAggregate?> GetByEntryNumberAsync(string entryNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get stock entries by date range
    /// </summary>
    Task<IEnumerable<StockEntryAggregate>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get posted stock entries
    /// </summary>
    Task<IEnumerable<StockEntryAggregate>> GetPostedEntriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get unposted stock entries
    /// </summary>
    Task<IEnumerable<StockEntryAggregate>> GetUnpostedEntriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get stock entries by company
    /// </summary>
    Task<IEnumerable<StockEntryAggregate>> GetByCompanyAsync(string company, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get stock entries by item
    /// </summary>
    Task<IEnumerable<StockEntryAggregate>> GetByItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get stock entries by warehouse
    /// </summary>
    Task<IEnumerable<StockEntryAggregate>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if entry number exists
    /// </summary>
    Task<bool> ExistsByEntryNumberAsync(string entryNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get next available entry number
    /// </summary>
    Task<string> GetNextEntryNumberAsync(string company, CancellationToken cancellationToken = default);

    Task<StockEntryAggregate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByTypeAsync(StockEntryType stockEntryType, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByPurposeAsync(StockEntryPurpose purpose, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByStatusAsync(StockEntryStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByFromWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByToWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByWorkOrderAsync(string workOrder, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByPurchaseOrderAsync(string purchaseOrder, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByDeliveryNoteAsync(string deliveryNoteNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetBySalesInvoiceAsync(string salesInvoiceNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByPickListAsync(string pickList, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByPurchaseReceiptAsync(string purchaseReceiptNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByAssetRepairAsync(string assetRepair, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByProjectAsync(string project, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetBySupplierAsync(string supplier, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetOpeningEntriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetReturnEntriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetTransitEntriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByBOMAsync(string bomNo, CancellationToken cancellationToken = default);
    Task<long> GetCountByTypeAsync(StockEntryType stockEntryType, CancellationToken cancellationToken = default);
    Task<long> GetCountByPurposeAsync(StockEntryPurpose purpose, CancellationToken cancellationToken = default);
    Task<long> GetCountByStatusAsync(StockEntryStatus status, CancellationToken cancellationToken = default);
    Task<long> GetCountByCompanyAsync(string company, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueByPurposeAsync(StockEntryPurpose purpose, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryAggregate>> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
    Task<long> GetPostedCountAsync(CancellationToken cancellationToken = default);
    Task<long> GetUnpostedCountAsync(CancellationToken cancellationToken = default);
} 
