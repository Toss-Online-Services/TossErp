using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Domain.Common;

public interface IStockEntryTypeRepository : IRepository<StockEntryType>
{
    Task<StockEntryType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryType>> GetByPurposeAsync(StockEntryPurpose purpose, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryType>> GetByAddToTransitAsync(bool addToTransit, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockEntryType>> GetByAllowNegativeStockAsync(bool allowNegativeStock, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsDisabledAsync(string name, CancellationToken cancellationToken = default);
    Task<long> GetCountByPurposeAsync(StockEntryPurpose purpose, CancellationToken cancellationToken = default);
} 
