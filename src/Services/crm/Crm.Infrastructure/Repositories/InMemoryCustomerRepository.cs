using Crm.Domain.Entities;
using Crm.Application.Interfaces;

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

    public Task<IEnumerable<Customer>> GetByStatusAsync(CustomerStatus status, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var customers = _customers.Where(c => c.Status == status);
            return Task.FromResult(customers);
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

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_customers.Count);
        }
    }

    public Task<int> GetCountByStatusAsync(CustomerStatus status, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var count = _customers.Count(c => c.Status == status);
            return Task.FromResult(count);
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
