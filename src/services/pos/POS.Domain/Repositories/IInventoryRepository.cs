using POS.Domain.AggregatesModel.InventoryAggregate;

namespace POS.Domain.Repositories
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<Inventory?> GetByProductAndStoreAsync(Guid productId, Guid storeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Inventory>> GetByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Inventory>> GetLowStockAsync(Guid storeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryMovement>> GetMovementsByProductAsync(Guid productId, Guid storeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryMovement>> GetMovementsByDateRangeAsync(Guid storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
} 
