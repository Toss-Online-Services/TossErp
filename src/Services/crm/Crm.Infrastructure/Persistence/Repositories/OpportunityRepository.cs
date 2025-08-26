using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.ValueObjects;
using TossErp.CRM.Domain.Repositories;
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

    public async Task<IEnumerable<Opportunity>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
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
            _logger.LogError(ex, "Error retrieving opportunities by stage: {Stage}", stage);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByPriorityAsync(OpportunityPriority priority, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.Priority == priority && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities by priority: {Priority}", priority);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByTypeAsync(OpportunityType type, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.Type == type && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities by type: {Type}", type);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetActiveOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.Stage != OpportunityStage.ClosedWon && 
                           o.Stage != OpportunityStage.ClosedLost && 
                           !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active opportunities");
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetClosedOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => (o.Stage == OpportunityStage.ClosedWon || o.Stage == OpportunityStage.ClosedLost) && 
                           !o.IsDeleted)
                .OrderByDescending(o => o.ModifiedAt ?? o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving closed opportunities");
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetClosingSoonAsync(int daysThreshold = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
            return await _context.Opportunities
                .Where(o => o.ExpectedCloseDate <= thresholdDate && 
                           o.Stage != OpportunityStage.ClosedWon && 
                           o.Stage != OpportunityStage.ClosedLost && 
                           !o.IsDeleted)
                .OrderBy(o => o.ExpectedCloseDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities closing soon: {Days}", daysThreshold);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByValueRangeAsync(decimal minValue, decimal maxValue, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.Value.EstimatedValue.Amount >= minValue && 
                           o.Value.EstimatedValue.Amount <= maxValue && 
                           !o.IsDeleted)
                .OrderByDescending(o => o.Value.EstimatedValue.Amount)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities by value range: {MinValue} - {MaxValue}", minValue, maxValue);
            throw;
        }
    }

    public async Task<decimal> GetTotalValueByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.Stage == stage && !o.IsDeleted)
                .SumAsync(o => o.Value.EstimatedValue.Amount, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total value by stage: {Stage}", stage);
            throw;
        }
    }

    public async Task<decimal> GetWeightedValueAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.Stage != OpportunityStage.ClosedWon && 
                           o.Stage != OpportunityStage.ClosedLost && 
                           !o.IsDeleted)
                .SumAsync(o => o.Value.EstimatedValue.Amount * (o.Value.Probability / 100m), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating weighted value");
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var term = searchTerm.ToLower();
            return await _context.Opportunities
                .Where(o => !o.IsDeleted && 
                    (o.Name.ToLower().Contains(term) ||
                     (o.Description != null && o.Description.ToLower().Contains(term))))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching opportunities with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<Opportunity> AddAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync(cancellationToken);
            return opportunity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding opportunity: {OpportunityId}", opportunity.Id);
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
            _logger.LogError(ex, "Error updating opportunity: {OpportunityId}", opportunity.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var opportunity = await GetByIdAsync(id, cancellationToken);
            if (opportunity != null)
            {
                opportunity.Delete("System");
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opportunity: {OpportunityId}", id);
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
            _logger.LogError(ex, "Error checking if opportunity exists: {OpportunityId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByValueRangeAsync(OpportunityValue minValue, OpportunityValue maxValue, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.Value.EstimatedValue.Amount >= minValue.EstimatedValue.Amount && 
                           o.Value.EstimatedValue.Amount <= maxValue.EstimatedValue.Amount && 
                           !o.IsDeleted)
                .OrderBy(o => o.Value.EstimatedValue.Amount)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities by value range: {MinValue} - {MaxValue}", minValue.EstimatedValue.Amount, maxValue.EstimatedValue.Amount);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByExpectedCloseDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.ExpectedCloseDate >= startDate && 
                           o.ExpectedCloseDate <= endDate && 
                           !o.IsDeleted)
                .OrderBy(o => o.ExpectedCloseDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities by expected close date range: {StartDate} - {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByAssignedUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.AssignedTo == userId && !o.IsDeleted)
                .OrderByDescending(o => o.Value.EstimatedValue.Amount)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities by assigned user: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetOverdueOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Opportunities
                .Where(o => o.ExpectedCloseDate < today && 
                           o.Stage != OpportunityStage.ClosedWon && 
                           o.Stage != OpportunityStage.ClosedLost && 
                           !o.IsDeleted)
                .OrderBy(o => o.ExpectedCloseDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue opportunities");
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
            _logger.LogError(ex, "Error getting opportunity count");
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
            _logger.LogError(ex, "Error getting opportunity count by stage: {Stage}", stage);
            throw;
        }
    }

    public async Task<decimal> GetTotalValueAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => !o.IsDeleted)
                .SumAsync(o => o.Value.EstimatedValue.Amount, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total opportunity value");
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByAssigneeAsync(string assignedTo, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.AssignedTo == assignedTo && !o.IsDeleted)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opportunities by assignee: {AssignedTo}", assignedTo);
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetOpenOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var openStages = new List<OpportunityStage>
            {
                OpportunityStage.Prospecting,
                OpportunityStage.Qualification,
                OpportunityStage.NeedsAnalysis,
                OpportunityStage.Proposal,
                OpportunityStage.Negotiation
            };

            return await _context.Opportunities
                .Where(o => openStages.Contains(o.Stage) && !o.IsDeleted)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting open opportunities");
            throw;
        }
    }

    public async Task<IEnumerable<Opportunity>> GetByCampaignAsync(string campaignName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Opportunities
                .Where(o => o.CampaignName == campaignName && !o.IsDeleted)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opportunities by campaign: {CampaignName}", campaignName);
            throw;
        }
    }

    public async Task DeleteAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        try
        {
            opportunity.Delete("system");
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opportunity: {OpportunityId}", opportunity.Id);
            throw;
        }
    }
}
