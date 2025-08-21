using Crm.Domain.Entities;
using Crm.Domain.Repositories;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Infrastructure.Persistence.Repositories;

public class CustomerInteractionRepository : ICustomerInteractionRepository
{
    private readonly CrmDbContext _context;
    private readonly ILogger<CustomerInteractionRepository> _logger;

    public CustomerInteractionRepository(CrmDbContext context, ILogger<CustomerInteractionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CustomerInteraction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CustomerInteractions
                .FirstOrDefaultAsync(ci => ci.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer interaction with ID: {InteractionId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<CustomerInteraction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CustomerInteractions
                .Where(ci => ci.CustomerId == customerId)
                .OrderByDescending(ci => ci.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving interactions for customer: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<IEnumerable<CustomerInteraction>> GetByStatusAsync(InteractionStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CustomerInteractions
                .Where(ci => ci.Status == status)
                .OrderByDescending(ci => ci.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving interactions with status: {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<CustomerInteraction>> GetByTypeAsync(InteractionType type, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CustomerInteractions
                .Where(ci => ci.Type == type)
                .OrderByDescending(ci => ci.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving interactions with type: {Type}", type);
            throw;
        }
    }

    public async Task<IEnumerable<CustomerInteraction>> GetFollowUpRequiredAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            return await _context.CustomerInteractions
                .Where(ci => ci.FollowUpDate.HasValue && ci.FollowUpDate.Value.Date <= today && ci.Status != InteractionStatus.Closed)
                .OrderBy(ci => ci.FollowUpDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving interactions requiring follow-up");
            throw;
        }
    }

    public async Task AddAsync(CustomerInteraction interaction, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.CustomerInteractions.Add(interaction);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Customer interaction added successfully with ID: {InteractionId}", interaction.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding customer interaction with ID: {InteractionId}", interaction.Id);
            throw;
        }
    }

    public async Task UpdateAsync(CustomerInteraction interaction, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.CustomerInteractions.Update(interaction);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Customer interaction updated successfully with ID: {InteractionId}", interaction.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer interaction with ID: {InteractionId}", interaction.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var interaction = await _context.CustomerInteractions.FindAsync(new object[] { id }, cancellationToken);
            if (interaction != null)
            {
                _context.CustomerInteractions.Remove(interaction);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Customer interaction deleted successfully with ID: {InteractionId}", id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent customer interaction with ID: {InteractionId}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer interaction with ID: {InteractionId}", id);
            throw;
        }
    }
}
