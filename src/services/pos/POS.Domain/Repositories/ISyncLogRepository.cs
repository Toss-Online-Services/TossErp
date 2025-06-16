using POS.Domain.AggregatesModel.SyncAggregate;
using POS.Domain.AggregatesModel.SyncLogAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories;

public interface ISyncLogRepository : IRepository<SyncLog>
{
    Task<SyncLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SyncLog?> GetByEntityIdAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
} 
