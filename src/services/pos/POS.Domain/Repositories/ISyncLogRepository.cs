using System.Threading;
using System.Threading.Tasks;
using TossErp.POS.Domain.AggregatesModel.SyncLogAggregate;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.Repositories;

public interface ISyncLogRepository : IRepository<SyncLog>
{
    new Task<SyncLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SyncLog?> GetByEntityIdAsync(string entityType, int entityId, CancellationToken cancellationToken = default);
} 
