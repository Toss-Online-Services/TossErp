using Crm.Domain.Entities;

namespace Crm.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(List<Customer> customers, int totalCount)> GetPagedAsync(
        int page,
        int limit,
        string? search = null,
        string? status = null,
        string? segment = null,
        string? sortBy = "CreatedAt",
        string? sortOrder = "desc",
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetBySegmentAsync(CustomerSegment segment, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetLapsedCustomersAsync(int daysThreshold = 90, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetTopCustomersAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetHighValueCustomersAsync(decimal threshold = 5000, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
