#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TossErp.POS.Domain.AggregatesModel.StoreAggregate;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.Repositories;

public interface IStoreRepository : IRepository<Store>
{
    Task<IEnumerable<Store>> GetByRegionAsync(string region);
    Task<IEnumerable<Store>> GetByStatusAsync(string status);
    Task<Store?> GetByCodeAsync(string code);
    Task<Store?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Store?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Store?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Store?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Store>> GetAllAsync(CancellationToken cancellationToken = default);
} 
