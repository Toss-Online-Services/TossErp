using POS.Domain.AggregatesModel.CustomerAggregate;

namespace POS.Domain.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Customer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
        Task<IEnumerable<Customer>> GetByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Customer>> GetTopCustomersBySpendingAsync(Guid storeId, int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync(Guid storeId, CancellationToken cancellationToken = default);
    }
} 
