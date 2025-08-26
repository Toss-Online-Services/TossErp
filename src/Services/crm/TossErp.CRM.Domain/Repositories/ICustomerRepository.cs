using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;

namespace TossErp.CRM.Domain.Repositories;

/// <summary>
/// Repository interface for Customer aggregate
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Customer?> GetByCustomerNumberAsync(string customerNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetByStatusAsync(CustomerStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetByTypeAsync(CustomerType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetByTierAsync(CustomerTier tier, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetByIndustryAsync(string industry, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CustomerNumberExistsAsync(string customerNumber, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
