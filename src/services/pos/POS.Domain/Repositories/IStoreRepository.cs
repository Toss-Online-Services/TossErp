#nullable enable
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.Common;

namespace POS.Domain.Repositories;

public interface IStoreRepository : IRepository<Store>
{
    Task<IEnumerable<Store>> GetByRegionAsync(string region, CancellationToken cancellationToken = default);
    Task<IEnumerable<Store>> GetByStatusAsync(string status);
    Task<Store?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Store?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Store?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Store?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
} 
