using Collaboration.Domain.Entities;
using Collaboration.Domain.Enums;

namespace Collaboration.Domain.Repositories;

/// <summary>
/// Repository interface for Campaign entities
/// </summary>
public interface ICampaignRepository
{
    Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetByTypeAsync(CampaignType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetByCreatorAsync(Guid createdBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetActiveCampaignsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetExpiredCampaignsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetSuccessfulCampaignsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetCampaignsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task<Campaign> UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCountByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAmountByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default);
}
