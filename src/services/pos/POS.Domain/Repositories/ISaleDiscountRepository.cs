using POS.Domain.AggregatesModel.SaleAggregate;

namespace POS.Domain.Repositories;

public interface ISaleDiscountRepository : IRepository<SaleDiscount>
{
    Task<SaleDiscount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<SaleDiscount>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SaleDiscount>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
} 
