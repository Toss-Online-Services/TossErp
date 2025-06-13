#nullable enable
using POS.Domain.AggregatesModel.StoreAggregate;

namespace POS.Domain.Repositories;

public interface IStoreRepository : IRepository<Store>
{
    Task<Store?> GetByCodeAsync(string code);
    Task<Store> GetByNameAsync(string name);
    Task<IEnumerable<Store>> GetByRegionAsync(string region);
} 
