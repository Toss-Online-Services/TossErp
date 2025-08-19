using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;

namespace TossErp.Procurement.Application.Common.Interfaces;

/// <summary>
/// Repository interface for managing supplier price lists
/// </summary>
public interface ISupplierPriceListRepository : IRepository<SupplierPriceList, Guid>
{
    /// <summary>
    /// Get active price lists for a supplier
    /// </summary>
    Task<IEnumerable<SupplierPriceList>> GetActiveBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the most current effective price list for a supplier
    /// </summary>
    Task<SupplierPriceList?> GetCurrentEffectiveBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get price for a specific item from a supplier's current price list
    /// </summary>
    Task<decimal?> GetCurrentPriceForItemAsync(Guid supplierId, Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get lead time for a specific item from a supplier's current price list
    /// </summary>
    Task<int?> GetCurrentLeadTimeForItemAsync(Guid supplierId, Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search price lists by name
    /// </summary>
    Task<IEnumerable<SupplierPriceList>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all price lists for a supplier (active and inactive)
    /// </summary>
    Task<IEnumerable<SupplierPriceList>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a price list name already exists for a supplier
    /// </summary>
    Task<bool> NameExistsForSupplierAsync(Guid supplierId, string name, Guid? excludePriceListId = null, CancellationToken cancellationToken = default);
}
