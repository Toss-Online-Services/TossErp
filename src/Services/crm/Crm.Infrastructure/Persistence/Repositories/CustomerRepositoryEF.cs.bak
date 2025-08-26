using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Repositories;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Customer = TossErp.CRM.Domain.Aggregates.Customer;

namespace Crm.Infrastructure.Persistence.Repositories;

public class CustomerRepositoryEF : ICustomerRepository
{
    private readonly CrmDbContext _context;
    private readonly ILogger<CustomerRepositoryEF> _logger;

    public CustomerRepositoryEF(CrmDbContext context, ILogger<CustomerRepositoryEF> logger)
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
                .Where(c => c.Status == status && c.Status != CustomerStatus.Churned)
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
                // Use domain method to change status to Churned instead of hard delete
                customer.ChangeStatus(CustomerStatus.Churned, "System");
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

    public async Task<IEnumerable<Customer>> GetLapsedCustomersAsync(int daysThreshold = 90, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysThreshold);
            return await _context.Customers
                .Where(c => c.Status != CustomerStatus.Churned)
                .Where(c => c.ModifiedAt.HasValue && c.ModifiedAt.Value < cutoffDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lapsed customers with threshold {DaysThreshold}", daysThreshold);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetTopCustomersAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement proper sorting by annual revenue once Money property mapping is resolved
            return await _context.Customers
                .Where(c => c.Status != CustomerStatus.Churned)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top {Count} customers", count);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetHighValueCustomersAsync(decimal threshold = 5000, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement proper filtering by annual revenue once Money property mapping is resolved
            return await _context.Customers
                .Where(c => c.Status != CustomerStatus.Churned)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting high value customers with threshold {Threshold}", threshold);
            throw;
        }
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.Status != CustomerStatus.Churned)
                .CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer count");
            throw;
        }
    }

    public async Task<int> GetCountByStatusAsync(CustomerStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.Status == status)
                .CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer count by status {Status}", status);
            throw;
        }
    }
}
