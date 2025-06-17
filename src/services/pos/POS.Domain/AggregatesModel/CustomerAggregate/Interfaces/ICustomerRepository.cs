using POS.Domain.Common;
using POS.Domain.Specifications;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<Customer>> GetBySegmentAsync(Guid segmentId);
        Task<IEnumerable<Customer>> GetBySpecificationAsync(Specification<Customer> specification);
        Task<IEnumerable<Customer>> GetHighValueCustomersAsync();
        Task<IEnumerable<Customer>> GetLoyalCustomersAsync();
        Task<IEnumerable<Customer>> GetAtRiskCustomersAsync();
        Task<Customer> AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);
    }
} 
