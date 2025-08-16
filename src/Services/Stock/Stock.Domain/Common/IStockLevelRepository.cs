using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Domain.Common;

public interface IStockLevelRepository : IRepository<StockLevel>
{
    Task<StockLevel?> GetByItemAndWarehouseAsync(Guid itemId, Guid warehouseId, Guid? binId = null, CancellationToken cancellationToken = default);
    Task<StockLevel?> GetByItemAndLocationAsync(Guid itemId, Guid warehouseId, Guid binId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLevel>> GetByItemAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLevel>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockLevel>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalQuantityAsync(Guid itemId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<bool> HasStockAsync(Guid itemId, Guid warehouseId, decimal quantity, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalStockByItemAsync(Guid itemId, CancellationToken cancellationToken = default);
}
