using Crm.Domain.Entities;
using Crm.Domain.Repositories;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Infrastructure.Persistence.Repositories;

public class LoyaltyTransactionRepository : ILoyaltyTransactionRepository
{
    private readonly CrmDbContext _context;
    private readonly ILogger<LoyaltyTransactionRepository> _logger;

    public LoyaltyTransactionRepository(CrmDbContext context, ILogger<LoyaltyTransactionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LoyaltyTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoyaltyTransactions
                .FirstOrDefaultAsync(lt => lt.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving loyalty transaction with ID: {TransactionId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<LoyaltyTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoyaltyTransactions
                .Where(lt => lt.CustomerId == customerId)
                .OrderByDescending(lt => lt.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving loyalty transactions for customer: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<IEnumerable<LoyaltyTransaction>> GetByTypeAsync(LoyaltyTransactionType type, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoyaltyTransactions
                .Where(lt => lt.Type == type)
                .OrderByDescending(lt => lt.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving loyalty transactions with type: {Type}", type);
            throw;
        }
    }

    public async Task<IEnumerable<LoyaltyTransaction>> GetExpiredTransactionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.UtcNow;
            return await _context.LoyaltyTransactions
                .Where(lt => lt.ExpiryDate.HasValue && lt.ExpiryDate.Value < today && lt.Type == LoyaltyTransactionType.Earned)
                .OrderBy(lt => lt.ExpiryDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expired loyalty transactions");
            throw;
        }
    }

    public async Task<int> GetCustomerBalanceAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var balance = await _context.LoyaltyTransactions
                .Where(lt => lt.CustomerId == customerId)
                .SumAsync(lt => lt.Points, cancellationToken);

            return balance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating loyalty balance for customer: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task AddAsync(LoyaltyTransaction transaction, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.LoyaltyTransactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Loyalty transaction added successfully with ID: {TransactionId}", transaction.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding loyalty transaction with ID: {TransactionId}", transaction.Id);
            throw;
        }
    }

    public async Task UpdateAsync(LoyaltyTransaction transaction, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.LoyaltyTransactions.Update(transaction);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Loyalty transaction updated successfully with ID: {TransactionId}", transaction.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating loyalty transaction with ID: {TransactionId}", transaction.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await _context.LoyaltyTransactions.FindAsync(new object[] { id }, cancellationToken);
            if (transaction != null)
            {
                _context.LoyaltyTransactions.Remove(transaction);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Loyalty transaction deleted successfully with ID: {TransactionId}", id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent loyalty transaction with ID: {TransactionId}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting loyalty transaction with ID: {TransactionId}", id);
            throw;
        }
    }
}
