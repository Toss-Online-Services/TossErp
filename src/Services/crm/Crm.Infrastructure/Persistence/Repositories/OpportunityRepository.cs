using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Repositories;
using TossErp.CRM.Domain.ValueObjects;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Infrastructure.Persistence.Repositories;

public class OpportunityRepository : IOpportunityRepository
{
    private readonly CrmDbContext _context;
    private readonly ILogger<OpportunityRepository> _logger;

    public OpportunityRepository(CrmDbContext context, ILogger<OpportunityRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Opportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunity with ID: {OpportunityId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all opportunities");
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.Stage == stage && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities with stage: {Stage}", stage);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.CustomerId == customerId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities for customer: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByAssigneeAsync(string assignedTo, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.AssignedTo == assignedTo && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities assigned to: {AssignedTo}", assignedTo);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetOpenOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.Stage != OpportunityStage.ClosedWon 
                           && o.Stage != OpportunityStage.ClosedLost 
                           && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving open opportunities");
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetClosedOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => (o.Stage == OpportunityStage.ClosedWon || o.Stage == OpportunityStage.ClosedLost) 
                           && !o.IsDeleted)
                .OrderByDescending(o => o.ActualCloseDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving closed opportunities");
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetOverdueOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Opportunities
                .Where(o => o.ExpectedCloseDate < today 
                           && o.Stage != OpportunityStage.ClosedWon 
                           && o.Stage != OpportunityStage.ClosedLost 
                           && !o.IsDeleted)
                .OrderBy(o => o.ExpectedCloseDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue opportunities");
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetClosingSoonAsync(int daysThreshold = 7, CancellationToken cancellationToken = default)
    {
        try
        {
            var thresholdDate = DateTime.UtcNow.Date.AddDays(daysThreshold);
            return await _context.Opportunities
                .Where(o => o.ExpectedCloseDate <= thresholdDate 
                           && o.ExpectedCloseDate >= DateTime.UtcNow.Date
                           && o.Stage != OpportunityStage.ClosedWon 
                           && o.Stage != OpportunityStage.ClosedLost 
                           && !o.IsDeleted)
                .OrderBy(o => o.ExpectedCloseDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities closing soon within {Days} days", daysThreshold);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByCampaignAsync(string campaignName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.CampaignName == campaignName && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities for campaign: {CampaignName}", campaignName);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await _context.Opportunities
                .Where(o => !o.IsDeleted && (
                    o.Title.ToLower().Contains(lowerSearchTerm) ||
                    o.Description.ToLower().Contains(lowerSearchTerm) ||
                    o.AssignedTo.ToLower().Contains(lowerSearchTerm) ||
                    (o.CampaignName != null && o.CampaignName.ToLower().Contains(lowerSearchTerm))
                ))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching opportunities with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .CountAsync(o => !o.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opportunities count");
            throw;
        }
    }

    public async Task<int> GetCountByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .CountAsync(o => o.Stage == stage && !o.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opportunities count for stage: {Stage}", stage);
            throw;
        }
    }

    public async Task<decimal> GetTotalValueAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var opportunities = await _context.Opportunities
                .Where(o => !o.IsDeleted)
                .ToListAsync(cancellationToken);

            return opportunities.Sum(o => o.Value.EstimatedValue.Amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total value for opportunities");
            throw;
        }
    }

    public async Task<decimal> GetWeightedValueAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var opportunities = await _context.Opportunities
                .Where(o => o.Stage != OpportunityStage.ClosedWon 
                           && o.Stage != OpportunityStage.ClosedLost 
                           && !o.IsDeleted)
                .ToListAsync(cancellationToken);

            return opportunities.Sum(o => o.Value.WeightedValue.Amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating weighted value for opportunities");
            throw;
        }
    }

    public async Task<decimal> GetTotalValueByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default)
    {
        try
        {
            var opportunities = await _context.Opportunities
                .Where(o => o.Stage == stage && !o.IsDeleted)
                .ToListAsync(cancellationToken);

            return opportunities.Sum(o => o.Value.EstimatedValue.Amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total value for opportunities in stage: {Stage}", stage);
            throw;
        }
    }

    public async Task<Opportunity> AddAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Opportunities.AddAsync(opportunity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return opportunity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding opportunity with ID: {OpportunityId}", opportunity.Id);
            throw;
        }
    }

    public async Task UpdateAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Opportunities.Update(opportunity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating opportunity with ID: {OpportunityId}", opportunity.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        try
        {
            opportunity.Delete("System"); // Soft delete
            _context.Opportunities.Update(opportunity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opportunity with ID: {OpportunityId}", opportunity.Id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .AnyAsync(o => o.Id == id && !o.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if opportunity exists with ID: {OpportunityId}", id);
            throw;
        }
    }
}
