using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.ValueObjects;

namespace Crm.Application.Interfaces;

public interface IOpportunityRepository
{
    Task<Opportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByPriorityAsync(OpportunityPriority priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByTypeAsync(OpportunityType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetActiveOpportunitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetClosedOpportunitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetClosingSoonAsync(int daysThreshold = 30, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByValueRangeAsync(decimal minValue, decimal maxValue, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalValueByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default);
    Task<decimal> GetWeightedValueAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task AddAsync(Opportunity opportunity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Opportunity opportunity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
