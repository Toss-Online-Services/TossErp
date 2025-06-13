#nullable enable
using eShop.POS.Domain.AggregatesModel.StoreAggregate;
using eShop.POS.Domain.Repositories;

namespace eShop.POS.Domain.Repositories;

public interface IStoreRepository : IRepository<Store>
{
    Task<IEnumerable<Store>> GetByRegionAsync(string region);
    Task<IEnumerable<Store>> GetByStatusAsync(string status);
    Task<Store?> GetByCodeAsync(string code);
    Task<Store?> GetByPhoneAsync(string phone);
} 
