using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Repositories;

namespace Crm.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of Opportunity repository for development/testing
/// </summary>
public class InMemoryOpportunityRepository : IOpportunityRepository
{
    private readonly List<Opportunity> _opportunities = new();
    private readonly object _lock = new();

    public Task<Opportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var opportunity = _opportunities.FirstOrDefault(o => o.Id == id);
            return Task.FromResult(opportunity);
        }
    }

    public Task<IEnumerable<Opportunity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_opportunities.AsEnumerable());
        }
    }

    public Task<IEnumerable<Opportunity>> GetByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var opportunities = _opportunities.Where(o => o.Stage == stage);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var opportunities = _opportunities.Where(o => o.CustomerId == customerId);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> GetByAssigneeAsync(string assignedTo, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var opportunities = _opportunities.Where(o => o.AssignedTo == assignedTo);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> GetByValueRangeAsync(decimal minValue, decimal maxValue, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var opportunities = _opportunities.Where(o => 
                o.Value.EstimatedValue.Amount >= minValue && 
                o.Value.EstimatedValue.Amount <= maxValue);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> GetClosingSoonAsync(int daysThreshold = 30, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(daysThreshold);
            var opportunities = _opportunities.Where(o => 
                o.ExpectedCloseDate <= cutoffDate &&
                o.Stage != OpportunityStage.ClosedWon &&
                o.Stage != OpportunityStage.ClosedLost);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var today = DateTime.UtcNow.Date;
            var opportunities = _opportunities.Where(o => 
                o.ExpectedCloseDate.Date < today &&
                o.Stage != OpportunityStage.ClosedWon &&
                o.Stage != OpportunityStage.ClosedLost);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> GetBySourceLeadAsync(Guid? sourceLeadId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var opportunities = _opportunities.Where(o => o.LeadId == sourceLeadId);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var term = searchTerm.ToLowerInvariant();
            var opportunities = _opportunities.Where(o => 
                o.Name.ToLowerInvariant().Contains(term) ||
                (o.Description?.ToLowerInvariant().Contains(term) == true));
            return Task.FromResult(opportunities);
        }
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_opportunities.Count);
        }
    }

    public Task<int> GetCountByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var count = _opportunities.Count(o => o.Stage == stage);
            return Task.FromResult(count);
        }
    }

    public Task<decimal> GetTotalValueAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_opportunities.Sum(o => o.Value.EstimatedValue.Amount));
        }
    }

    public Task<IEnumerable<Opportunity>> GetOpenOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var opportunities = _opportunities.Where(o => 
                o.Stage != OpportunityStage.ClosedWon && 
                o.Stage != OpportunityStage.ClosedLost);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> GetClosedOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var opportunities = _opportunities.Where(o => 
                o.Stage == OpportunityStage.ClosedWon || 
                o.Stage == OpportunityStage.ClosedLost);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> GetOverdueOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var today = DateTime.UtcNow.Date;
            var opportunities = _opportunities.Where(o => 
                o.ExpectedCloseDate.Date < today &&
                o.Stage != OpportunityStage.ClosedWon &&
                o.Stage != OpportunityStage.ClosedLost);
            return Task.FromResult(opportunities);
        }
    }

    public Task<IEnumerable<Opportunity>> GetByCampaignAsync(string campaignName, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var opportunities = _opportunities.Where(o => o.CampaignName == campaignName);
            return Task.FromResult(opportunities);
        }
    }

    public Task<decimal> GetWeightedValueAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var weightedValue = _opportunities
                .Where(o => o.Stage != OpportunityStage.ClosedWon && o.Stage != OpportunityStage.ClosedLost)
                .Sum(o => o.Value.WeightedValue.Amount);
            return Task.FromResult(weightedValue);
        }
    }

    public Task<decimal> GetTotalValueByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var total = _opportunities.Where(o => o.Stage == stage).Sum(o => o.Value.EstimatedValue.Amount);
            return Task.FromResult(total);
        }
    }

    public Task<Opportunity> AddAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _opportunities.Add(opportunity);
            return Task.FromResult(opportunity);
        }
    }

    public Task UpdateAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var existingIndex = _opportunities.FindIndex(o => o.Id == opportunity.Id);
            if (existingIndex >= 0)
            {
                _opportunities[existingIndex] = opportunity;
            }
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _opportunities.RemoveAll(o => o.Id == opportunity.Id);
            return Task.CompletedTask;
        }
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var exists = _opportunities.Any(o => o.Id == id);
            return Task.FromResult(exists);
        }
    }
}
