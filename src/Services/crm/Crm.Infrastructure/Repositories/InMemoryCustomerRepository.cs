using Crm.Domain.Entities;
using Crm.Domain.Repositories;

namespace Crm.Infrastructure.Repositories;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = new();
    private readonly object _lock = new();

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(customer);
        }
    }

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var customer = _customers.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(customer);
        }
    }

    public Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_customers.AsEnumerable());
        }
    }

    public Task<(List<Customer> customers, int totalCount)> GetPagedAsync(
        int page,
        int limit,
        string? search = null,
        string? status = null,
        string? segment = null,
        string? sortBy = "CreatedAt",
        string? sortOrder = "desc",
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _customers.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => 
                    c.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Phone.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<CustomerStatus>(status, true, out var statusEnum))
            {
                query = query.Where(c => c.Status == statusEnum);
            }

            // Apply segment filter
            if (!string.IsNullOrWhiteSpace(segment) && Enum.TryParse<CustomerSegment>(segment, true, out var segmentEnum))
            {
                query = query.Where(c => c.Segment == segmentEnum);
            }

            // Get total count before pagination
            var totalCount = query.Count();

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "firstname" => sortOrder?.ToLower() == "asc" 
                    ? query.OrderBy(c => c.FirstName) 
                    : query.OrderByDescending(c => c.FirstName),
                "lastname" => sortOrder?.ToLower() == "asc" 
                    ? query.OrderBy(c => c.LastName) 
                    : query.OrderByDescending(c => c.LastName),
                "email" => sortOrder?.ToLower() == "asc" 
                    ? query.OrderBy(c => c.Email) 
                    : query.OrderByDescending(c => c.Email),
                "totalspent" => sortOrder?.ToLower() == "asc" 
                    ? query.OrderBy(c => c.TotalSpent) 
                    : query.OrderByDescending(c => c.TotalSpent),
                "purchasecount" => sortOrder?.ToLower() == "asc" 
                    ? query.OrderBy(c => c.PurchaseCount) 
                    : query.OrderByDescending(c => c.PurchaseCount),
                "lastpurchasedate" => sortOrder?.ToLower() == "asc" 
                    ? query.OrderBy(c => c.LastPurchaseDate) 
                    : query.OrderByDescending(c => c.LastPurchaseDate),
                _ => sortOrder?.ToLower() == "asc" 
                    ? query.OrderBy(c => c.CreatedAt) 
                    : query.OrderByDescending(c => c.CreatedAt)
            };

            // Apply pagination
            var customers = query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return Task.FromResult((customers, totalCount));
        }
    }

    public Task<IEnumerable<Customer>> GetBySegmentAsync(CustomerSegment segment, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var customers = _customers.Where(c => c.Segment == segment);
            return Task.FromResult(customers);
        }
    }

    public Task<IEnumerable<Customer>> GetLapsedCustomersAsync(int daysThreshold = 90, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysThreshold);
            var customers = _customers.Where(c => c.LastPurchaseDate.HasValue && c.LastPurchaseDate.Value < cutoffDate);
            return Task.FromResult(customers);
        }
    }

    public Task<IEnumerable<Customer>> GetTopCustomersAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var customers = _customers
                .OrderByDescending(c => c.TotalSpent)
                .Take(count);
            return Task.FromResult(customers);
        }
    }

    public Task<IEnumerable<Customer>> GetHighValueCustomersAsync(decimal threshold = 5000, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var customers = _customers.Where(c => c.TotalSpent >= threshold);
            return Task.FromResult(customers);
        }
    }

    public Task<IEnumerable<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var customers = _customers.Where(c => 
                c.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                c.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(customers);
        }
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _customers.Add(customer);
            return Task.CompletedTask;
        }
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                var index = _customers.IndexOf(existingCustomer);
                _customers[index] = customer;
            }
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
            }
            return Task.CompletedTask;
        }
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var exists = _customers.Any(c => c.Id == id);
            return Task.FromResult(exists);
        }
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var exists = _customers.Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }
    }
}
