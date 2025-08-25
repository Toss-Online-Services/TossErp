using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;

namespace TossErp.CRM.Domain.Repositories;

/// <summary>
/// Repository interface for Lead aggregate
/// </summary>
public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetByStatusAsync(LeadStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetBySourceAsync(LeadSource source, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetByAssigneeAsync(string assignedTo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetByScoreRangeAsync(int minScore, int maxScore, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetHotLeadsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetStaleLeadsAsync(int daysThreshold = 30, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetByCampaignAsync(string campaignName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountByStatusAsync(LeadStatus status, CancellationToken cancellationToken = default);
    Task<Lead> AddAsync(Lead lead, CancellationToken cancellationToken = default);
    Task UpdateAsync(Lead lead, CancellationToken cancellationToken = default);
    Task DeleteAsync(Lead lead, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
