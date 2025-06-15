using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.StoreAggregate
{
    public interface IStoreRepository : IRepository<Store>
    {
        Task<Store?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Store?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    }
} 
