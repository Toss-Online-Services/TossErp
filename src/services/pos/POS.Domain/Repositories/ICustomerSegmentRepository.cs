using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories
{
    public interface ICustomerSegmentRepository : IRepository<CustomerSegment>
    {
        Task<CustomerSegment?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerSegment>> GetActiveSegmentsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerSegment>> GetExpiredSegmentsAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<int> GetCustomerCountAsync(Guid segmentId, CancellationToken cancellationToken = default);
        Task IncrementCustomerCountAsync(Guid segmentId, CancellationToken cancellationToken = default);
        Task DecrementCustomerCountAsync(Guid segmentId, CancellationToken cancellationToken = default);
    }
} 
