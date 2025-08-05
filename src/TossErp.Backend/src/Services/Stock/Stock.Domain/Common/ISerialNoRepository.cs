using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Domain.Common;

public interface ISerialNoRepository : IRepository<SerialNo>
{
    Task<SerialNo?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByItemAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByWarehouseAsync(string warehouse, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByBatchNoAsync(string batchNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetBySupplierAsync(string supplier, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByCustomerAsync(string customer, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByPurchaseDocumentAsync(string purchaseDocumentType, string purchaseDocumentNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetBySalesDocumentAsync(string salesDocumentType, string salesDocumentNo, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByProjectAsync(string project, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByAssetAsync(string asset, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByMaintenanceStatusAsync(string maintenanceStatus, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByWarrantyExpiryRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetByAMCExpiryRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetExpiringWarrantyAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetExpiringAMCAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetExpiredWarrantyAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SerialNo>> GetExpiredAMCAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<long> GetCountByItemAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByWarehouseAsync(string warehouse, CancellationToken cancellationToken = default);
    Task<long> GetCountBySupplierAsync(string supplier, CancellationToken cancellationToken = default);
    Task<long> GetCountByCustomerAsync(string customer, CancellationToken cancellationToken = default);
    Task<long> GetExpiringWarrantyCountAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<long> GetExpiringAMCCountAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<long> GetExpiredWarrantyCountAsync(CancellationToken cancellationToken = default);
    Task<long> GetExpiredAMCCountAsync(CancellationToken cancellationToken = default);
} 
