using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;

namespace TossErp.CRM.Domain.Repositories;

/// <summary>
/// Repository interface for Opportunity aggregate
/// </summary>
public interface IOpportunityRepository
{
    Task<Opportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByAssigneeAsync(string assignedTo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByPriorityAsync(OpportunityPriority priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByTypeAsync(OpportunityType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetActiveOpportunitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetOpenOpportunitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetClosedOpportunitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetOverdueOpportunitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetClosingSoonAsync(int daysThreshold = 7, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByCampaignAsync(string campaignName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetWeightedValueAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default);
    Task<Opportunity> AddAsync(Opportunity opportunity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Opportunity opportunity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Opportunity opportunity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
