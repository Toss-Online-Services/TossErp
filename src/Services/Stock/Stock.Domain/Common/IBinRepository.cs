using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Common;

public interface IBinRepository : IRepository<Bin>
{
    Task<Bin?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Bin?> GetByItemAndWarehouseAsync(ItemCode itemCode, WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<Bin?> GetByItemAndWarehouseAsync(string itemCode, string warehouseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetByItemAsync(ItemCode itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetByItemAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetByWarehouseAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetByWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetByCompanyAsync(string company, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetWithStockAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetWithReservedStockAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetWithOrderedStockAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetLowStockBinsAsync(decimal threshold, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetOverstockBinsAsync(decimal threshold, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bin>> GetExpiringStockBinsAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByItemAndWarehouseAsync(ItemCode itemCode, WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsByItemAndWarehouseAsync(string itemCode, string warehouseCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByItemAsync(ItemCode itemCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByItemAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByWarehouseAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByCompanyAsync(string company, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalStockValueByWarehouseAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalStockValueByWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalStockValueByItemAsync(ItemCode itemCode, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalStockValueByItemAsync(string itemCode, CancellationToken cancellationToken = default);
} 
