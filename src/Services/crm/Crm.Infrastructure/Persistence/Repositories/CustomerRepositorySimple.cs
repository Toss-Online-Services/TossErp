using Crm.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;

namespace Crm.Infrastructure.Persistence.Repositories;

public class CustomerRepositorySimple : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Customer?>(null);
    }

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Customer?>(null);
    }

    public Task<Customer?> GetByCustomerNumberAsync(string customerNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Customer?>(null);
    }

    public Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Enumerable.Empty<Customer>());
    }

    public Task<IEnumerable<Customer>> GetByStatusAsync(CustomerStatus status, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Enumerable.Empty<Customer>());
    }

    public Task<IEnumerable<Customer>> GetByTypeAsync(CustomerType type, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Enumerable.Empty<Customer>());
    }

    public Task<IEnumerable<Customer>> GetByTierAsync(CustomerTier tier, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Enumerable.Empty<Customer>());
    }

    public Task<IEnumerable<Customer>> GetByIndustryAsync(string industry, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Enumerable.Empty<Customer>());
    }

    public Task<IEnumerable<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Enumerable.Empty<Customer>());
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> CustomerNumberExistsAsync(string customerNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }
}
