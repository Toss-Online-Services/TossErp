using Crm.Domain.Entities;
using Crm.Application.DTOs;

namespace Crm.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<(List<Customer> customers, int totalCount)> GetPagedAsync(
        int page,
        int limit,
        string? search = null,
        string? status = null,
        string? segment = null,
        string? sortBy = "CreatedAt",
        string? sortOrder = "desc",
        CancellationToken cancellationToken = default);
    Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<List<Customer>> GetBySegmentAsync(CustomerSegment segment, CancellationToken cancellationToken = default);
    Task<List<Customer>> GetLapsedCustomersAsync(int daysThreshold = 90, CancellationToken cancellationToken = default);
    Task<List<Customer>> GetHighValueCustomersAsync(decimal threshold = 5000, CancellationToken cancellationToken = default);
}
