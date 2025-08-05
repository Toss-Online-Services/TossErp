using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Common;

/// <summary>
/// Repository interface for Warehouse Aggregate
/// </summary>
public interface IWarehouseRepository : IRepository<WarehouseAggregate>
{
    /// <summary>
    /// Get warehouse by code
    /// </summary>
    Task<WarehouseAggregate?> GetByCodeAsync(string warehouseCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get warehouses by company
    /// </summary>
    Task<IEnumerable<WarehouseAggregate>> GetByCompanyAsync(string company, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active warehouses
    /// </summary>
    Task<IEnumerable<WarehouseAggregate>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get group warehouses
    /// </summary>
    Task<IEnumerable<WarehouseAggregate>> GetGroupsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get leaf warehouses (non-groups)
    /// </summary>
    Task<IEnumerable<WarehouseAggregate>> GetLeavesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get root warehouses
    /// </summary>
    Task<IEnumerable<WarehouseAggregate>> GetRootsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get child warehouses
    /// </summary>
    Task<IEnumerable<WarehouseAggregate>> GetChildrenAsync(Guid parentWarehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get warehouses by type
    /// </summary>
    Task<IEnumerable<WarehouseAggregate>> GetByTypeAsync(string warehouseType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get rejected warehouses
    /// </summary>
    Task<IEnumerable<WarehouseAggregate>> GetRejectedWarehousesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if warehouse code exists
    /// </summary>
    Task<bool> ExistsByCodeAsync(string warehouseCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get warehouses that can accept stock
    /// </summary>
    Task<IEnumerable<WarehouseAggregate>> GetCanAcceptStockAsync(CancellationToken cancellationToken = default);

    Task<WarehouseAggregate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<WarehouseAggregate>> GetInactiveAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsActiveAsync(string warehouseCode, CancellationToken cancellationToken = default);
    Task<bool> IsDisabledAsync(string warehouseCode, CancellationToken cancellationToken = default);
    Task<long> GetCountByCompanyAsync(string company, CancellationToken cancellationToken = default);
    Task<long> GetActiveCountAsync(CancellationToken cancellationToken = default);
    Task<long> GetInactiveCountAsync(CancellationToken cancellationToken = default);
    Task<long> GetCountByTypeAsync(string warehouseType, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalStockValueAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalStockValueAsync(string warehouseCode, CancellationToken cancellationToken = default);
    
    // Additional methods for Application layer queries
    Task<List<WarehouseAggregate>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);
    Task<List<WarehouseAggregate>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<List<WarehouseAggregate>> GetByCapacityRangeAsync(decimal? minCapacity, decimal? maxCapacity, CancellationToken cancellationToken = default);
    Task<List<WarehouseAggregate>> GetByUtilizationRangeAsync(decimal? minUtilization, decimal? maxUtilization, CancellationToken cancellationToken = default);
    Task<List<WarehouseAggregate>> GetByValueRangeAsync(decimal? minValue, decimal? maxValue, CancellationToken cancellationToken = default);
    Task<List<WarehouseAggregate>> GetByItemAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<List<WarehouseAggregate>> GetByBinAsync(Guid binId, CancellationToken cancellationToken = default);
    Task<List<WarehouseAggregate>> GetByMovementAsync(Guid movementId, CancellationToken cancellationToken = default);
    
    // Analytics and reporting methods
    Task<object?> GetAnalyticsAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<object?> GetReportAsync(Guid? warehouseId, string? reportType, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
} 
