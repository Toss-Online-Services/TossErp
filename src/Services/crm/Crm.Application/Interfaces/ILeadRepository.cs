using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;

namespace Crm.Application.Interfaces;

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Lead?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetByStatusAsync(LeadStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetBySourceAsync(LeadSource source, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetByScoreRangeAsync(int minScore, int maxScore, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetQualifiedLeadsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetHotLeadsAsync(int scoreThreshold = 75, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetByIndustryAsync(string industry, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> GetByCampaignAsync(string campaignName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lead>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task AddAsync(Lead lead, CancellationToken cancellationToken = default);
    Task UpdateAsync(Lead lead, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
