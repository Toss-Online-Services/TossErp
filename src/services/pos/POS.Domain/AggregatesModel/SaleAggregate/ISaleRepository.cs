using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Sale>> GetByStaffIdAsync(int staffId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Sale>> GetByBuyerIdAsync(int buyerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Sale>> GetOfflineSalesAsync(CancellationToken cancellationToken = default);
    }
} 
