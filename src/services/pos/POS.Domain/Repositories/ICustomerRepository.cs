using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<IEnumerable<Customer>> GetBySegmentAsync(Guid segmentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Customer>> GetHighValueCustomersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Customer>> GetLoyalCustomersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Customer>> GetAtRiskCustomersAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    }
} 
