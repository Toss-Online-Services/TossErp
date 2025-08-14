using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Domain.Common;

public interface IStockMovementRepository : IRepository<StockMovement>
{
    Task<IEnumerable<StockMovement>> GetByItemAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovement>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovement>> GetByItemAndWarehouseAsync(Guid itemId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovement>> GetByMovementTypeAsync(MovementType movementType, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovement>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovement>> GetByReferenceAsync(string referenceNumber, string referenceType, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovement>> GetByBatchAsync(Guid batchId, CancellationToken cancellationToken = default);
}
