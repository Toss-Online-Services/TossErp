using Crm.Domain.Entities;
using Crm.Domain.Repositories;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CrmDbContext _context;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(CrmDbContext context, ILogger<CustomerRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Include(c => c.Interactions)
                .Include(c => c.LoyaltyTransactions)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with ID: {CustomerId}", id);
            throw;
        }
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Include(c => c.Interactions)
                .Include(c => c.LoyaltyTransactions)
                .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with email: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Include(c => c.Interactions)
                .Include(c => c.LoyaltyTransactions)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all customers");
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetBySegmentAsync(CustomerSegment segment, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.Segment == segment)
                .OrderByDescending(c => c.TotalSpent)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers by segment: {Segment}", segment);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetLapsedCustomersAsync(int daysThreshold = 90, CancellationToken cancellationToken = default)
    {
        try
        {
            var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);
            return await _context.Customers
                .Where(c => c.LastPurchaseDate.HasValue && c.LastPurchaseDate.Value < thresholdDate)
                .OrderBy(c => c.LastPurchaseDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lapsed customers with threshold: {DaysThreshold}", daysThreshold);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetTopCustomersAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .OrderByDescending(c => c.TotalSpent)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top {Count} customers", count);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetHighValueCustomersAsync(decimal threshold = 5000, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.TotalSpent >= threshold)
                .OrderByDescending(c => c.TotalSpent)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving high value customers with threshold: {Threshold}", threshold);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Customer>();

            var lowerSearchTerm = searchTerm.ToLower();

            return await _context.Customers
                .Where(c => 
                    c.FirstName.ToLower().Contains(lowerSearchTerm) ||
                    c.LastName.ToLower().Contains(lowerSearchTerm) ||
                    c.Email.ToLower().Contains(lowerSearchTerm) ||
                    c.Phone.Contains(searchTerm))
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customers with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Customer added successfully with ID: {CustomerId}", customer.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding customer with ID: {CustomerId}", customer.Id);
            throw;
        }
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Customer updated successfully with ID: {CustomerId}", customer.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer with ID: {CustomerId}", customer.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(new object[] { id }, cancellationToken);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Customer deleted successfully with ID: {CustomerId}", id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent customer with ID: {CustomerId}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers.AnyAsync(c => c.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of customer with ID: {CustomerId}", id);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers.AnyAsync(c => c.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of customer with email: {Email}", email);
            throw;
        }
    }
}
