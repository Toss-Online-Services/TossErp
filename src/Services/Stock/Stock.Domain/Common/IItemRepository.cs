using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Domain.Common;

/// <summary>
/// Repository interface for Item Aggregate
/// </summary>
public interface IItemRepository : IRepository<ItemAggregate>
{
    /// <summary>
    /// Get item by code
    /// </summary>
    Task<ItemAggregate?> GetByCodeAsync(string itemCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items by group
    /// </summary>
    Task<IEnumerable<ItemAggregate>> GetByGroupAsync(string itemGroup, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items by company
    /// </summary>
    Task<IEnumerable<ItemAggregate>> GetByCompanyAsync(string company, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items that need reordering
    /// </summary>
    Task<IEnumerable<ItemAggregate>> GetItemsNeedingReorderAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active items available for sale
    /// </summary>
    Task<IEnumerable<ItemAggregate>> GetAvailableForSaleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active items available for purchase
    /// </summary>
    Task<IEnumerable<ItemAggregate>> GetAvailableForPurchaseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if item code exists
    /// </summary>
    Task<bool> ExistsByCodeAsync(string itemCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items by supplier
    /// </summary>
    Task<IEnumerable<ItemAggregate>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ItemAggregate>> GetByTypeAsync(ItemType itemType, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetByStatusAsync(ItemStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetStockItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetSalesItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetPurchaseItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetManufacturingItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetByBrandAsync(string brand, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetByCustomerAsync(string customer, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetExpiringItemsAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetLowStockItemsAsync(decimal threshold, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetOverstockItemsAsync(decimal threshold, CancellationToken cancellationToken = default);
    Task<long> GetCountByGroupAsync(string itemGroup, CancellationToken cancellationToken = default);
    Task<long> GetCountByTypeAsync(ItemType itemType, CancellationToken cancellationToken = default);
    Task<long> GetCountByStatusAsync(ItemStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetItemsAsync(
        string? itemCode = null,
        string? itemName = null,
        string? itemGroup = null,
        ItemType? itemType = null,
        bool? isStockItem = null,
        bool? disabled = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
} 
