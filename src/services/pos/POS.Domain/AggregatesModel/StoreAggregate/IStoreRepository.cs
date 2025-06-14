using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.StoreAggregate
{
    public interface IStoreRepository : IRepository<Store>
    {
        Task<Store?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Store?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    }
} 
