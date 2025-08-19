using TossErp.Sales.Application.Common.DTOs;

namespace TossErp.Sales.Application.Common.Interfaces;

/// <summary>
/// Service for inventory operations
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Update stock levels for items in a sale
    /// </summary>
    /// <param name="saleId">Sale ID</param>
    /// <param name="items">Items with quantities to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateStockLevelsAsync(Guid saleId, IEnumerable<SaleItemDto> items, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reserve stock for items in a sale
    /// </summary>
    /// <param name="saleId">Sale ID</param>
    /// <param name="items">Items with quantities to reserve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ReserveStockAsync(Guid saleId, IEnumerable<SaleItemDto> items, CancellationToken cancellationToken = default);

    /// <summary>
    /// Release reserved stock for a cancelled sale
    /// </summary>
    /// <param name="saleId">Sale ID</param>
    /// <param name="items">Items with quantities to release</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ReleaseReservedStockAsync(Guid saleId, IEnumerable<SaleItemDto> items, CancellationToken cancellationToken = default);
}
