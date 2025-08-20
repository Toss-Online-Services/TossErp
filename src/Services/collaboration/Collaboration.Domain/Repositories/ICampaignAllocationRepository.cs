using Collaboration.Domain.Entities;
using Collaboration.Domain.Enums;

namespace Collaboration.Domain.Repositories;

/// <summary>
/// Repository interface for CampaignAllocation entities
/// </summary>
public interface ICampaignAllocationRepository
{
    Task<CampaignAllocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignAllocation>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignAllocation>> GetByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignAllocation>> GetByStatusAsync(AllocationStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignAllocation>> GetByTypeAsync(AllocationType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignAllocation>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignAllocation>> GetDeliveredAllocationsAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignAllocation>> GetSettledAllocationsAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<CampaignAllocation> AddAsync(CampaignAllocation allocation, CancellationToken cancellationToken = default);
    Task<CampaignAllocation> UpdateAsync(CampaignAllocation allocation, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetAllocationCountAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAllocatedAmountAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalDiscountAmountAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalFinalAmountAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
