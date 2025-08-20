using Collaboration.Domain.Entities;
using Collaboration.Domain.Enums;

namespace Collaboration.Domain.Repositories;

/// <summary>
/// Repository interface for CampaignParticipant entities
/// </summary>
public interface ICampaignParticipantRepository
{
    Task<CampaignParticipant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignParticipant>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignParticipant>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignParticipant>> GetByStatusAsync(ParticipantStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignParticipant>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignParticipant>> GetActiveParticipantsAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignParticipant>> GetParticipantsWithPaymentAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<CampaignParticipant> AddAsync(CampaignParticipant participant, CancellationToken cancellationToken = default);
    Task<CampaignParticipant> UpdateAsync(CampaignParticipant participant, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsUserParticipantAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetParticipantCountAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalCommittedAmountAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaymentAmountAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
