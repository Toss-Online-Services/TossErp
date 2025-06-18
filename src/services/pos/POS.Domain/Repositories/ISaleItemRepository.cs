using POS.Domain.AggregatesModel.SaleAggregate;

namespace POS.Domain.Repositories;

public interface ISaleItemRepository : IRepository<SaleItem>
{
    Task<IEnumerable<SaleItem>> GetBySaleAsync(int saleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SaleItem>> GetByProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SaleItem>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default);
} 
