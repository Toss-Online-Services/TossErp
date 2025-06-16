using POS.Domain.AggregatesModel.ClientRequestAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories;

public interface IClientRequestRepository : IRepository<ClientRequest>
{
    Task<ClientRequest?> GetByRequestIdAsync(string requestId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientRequest>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientRequest>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
} 
