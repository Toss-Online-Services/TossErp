using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using Crm.Infrastructure.Interfaces;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Customer = TossErp.CRM.Domain.Aggregates.Customer;

namespace Crm.Infrastructure.Persistence.Repositories;

public class CustomerRepositoryNew : ICustomerRepository
{
    private readonly CrmDbContext _context;
    private readonly ILogger<CustomerRepositoryNew> _logger;

    public CustomerRepositoryNew(CrmDbContext context, ILogger<CustomerRepositoryNew> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.Status != CustomerStatus.Churned, cancellationToken);
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
                .FirstOrDefaultAsync(c => c.PrimaryEmail!.Value == email && c.Status != CustomerStatus.Churned, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with email: {Email}", email);
            throw;
        }
    }

    public async Task<Customer?> GetByCustomerNumberAsync(string customerNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerNumber.Value == customerNumber && c.Status != CustomerStatus.Churned, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with number: {CustomerNumber}", customerNumber);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.Status != CustomerStatus.Churned)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all customers");
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetByStatusAsync(CustomerStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.Status == status)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers by status: {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetByTypeAsync(CustomerType type, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.Type == type && c.Status != CustomerStatus.Churned)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers by type: {Type}", type);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetByTierAsync(CustomerTier tier, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.Tier == tier && c.Status != CustomerStatus.Churned)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers by tier: {Tier}", tier);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetByIndustryAsync(string industry, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.Industry == industry && c.Status != CustomerStatus.Churned)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers by industry: {Industry}", industry);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var term = searchTerm.ToLower();
            return await _context.Customers
                .Where(c => c.Status != CustomerStatus.Churned && 
                    (c.Name.ToLower().Contains(term) ||
                     c.CustomerNumber.Value.ToLower().Contains(term) ||
                     (c.PrimaryEmail != null && c.PrimaryEmail.Value.ToLower().Contains(term)) ||
                     (c.Industry != null && c.Industry.ToLower().Contains(term))))
                .OrderBy(c => c.Name)
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding customer: {CustomerId}", customer.Id);
            throw;
        }
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer: {CustomerId}", customer.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await GetByIdAsync(id, cancellationToken);
            if (customer != null)
            {
                // Mark customer as churned instead of actually deleting
                // customer.ChangeStatus(CustomerStatus.Churned, "Deleted by system"); // This would need to be implemented in the domain
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer: {CustomerId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .AnyAsync(c => c.Id == id && c.Status != CustomerStatus.Churned, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if customer exists: {CustomerId}", id);
            throw;
        }
    }

    public async Task<bool> CustomerNumberExistsAsync(string customerNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .AnyAsync(c => c.CustomerNumber.Value == customerNumber && c.Status != CustomerStatus.Churned, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if customer number exists: {CustomerNumber}", customerNumber);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .AnyAsync(c => c.PrimaryEmail!.Value == email && c.Status != CustomerStatus.Churned, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if email exists: {Email}", email);
            throw;
        }
    }
}
