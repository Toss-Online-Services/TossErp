using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.ValueObjects;

namespace Crm.Infrastructure.Interfaces;

public interface IOpportunityRepository
{
    Task<Opportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByStageAsync(OpportunityStage stage, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByValueRangeAsync(OpportunityValue minValue, OpportunityValue maxValue, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByExpectedCloseDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> GetByAssignedUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Opportunity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task AddAsync(Opportunity opportunity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Opportunity opportunity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
