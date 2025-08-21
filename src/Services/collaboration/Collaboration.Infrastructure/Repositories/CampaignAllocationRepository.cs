namespace Collaboration.Infrastructure.Repositories;

public class CampaignAllocationRepository : ICampaignAllocationRepository
{
    private readonly List<CampaignAllocation> _allocations = new();
    private readonly ILogger<CampaignAllocationRepository> _logger;

    public CampaignAllocationRepository(ILogger<CampaignAllocationRepository> logger)
    {
        _logger = logger;
    }

    public async Task<CampaignAllocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting campaign allocation with ID: {AllocationId}", id);
        return await Task.FromResult(_allocations.FirstOrDefault(a => a.Id == id));
    }

    public async Task<IEnumerable<CampaignAllocation>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting allocations for campaign: {CampaignId}", campaignId);
        return await Task.FromResult(_allocations.Where(a => a.CampaignId == campaignId));
    }

    public async Task<IEnumerable<CampaignAllocation>> GetByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting allocations for participant: {ParticipantId}", participantId);
        return await Task.FromResult(_allocations.Where(a => a.ParticipantId == participantId));
    }

    public async Task<IEnumerable<CampaignAllocation>> GetByStatusAsync(AllocationStatus status, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting allocations with status: {Status}", status);
        return await Task.FromResult(_allocations.Where(a => a.Status == status));
    }

    public async Task<IEnumerable<CampaignAllocation>> GetByTypeAsync(AllocationType type, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting allocations with type: {Type}", type);
        return await Task.FromResult(_allocations.Where(a => a.Type == type));
    }

    public async Task<CampaignAllocation> AddAsync(CampaignAllocation allocation, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new campaign allocation: {AllocationId} for campaign: {CampaignId}", allocation.Id, allocation.CampaignId);
        _allocations.Add(allocation);
        return await Task.FromResult(allocation);
    }

    public async Task<CampaignAllocation> UpdateAsync(CampaignAllocation allocation, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating campaign allocation: {AllocationId}", allocation.Id);
        var existingIndex = _allocations.FindIndex(a => a.Id == allocation.Id);
        if (existingIndex >= 0)
        {
            _allocations[existingIndex] = allocation;
        }
        return await Task.FromResult(allocation);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting campaign allocation: {AllocationId}", id);
        var allocation = _allocations.FirstOrDefault(a => a.Id == id);
        if (allocation != null)
        {
            _allocations.Remove(allocation);
        }
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_allocations.Any(a => a.Id == id));
    }

    public async Task<IEnumerable<CampaignAllocation>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting allocations for tenant: {TenantId}", tenantId);
        return await Task.FromResult(_allocations.Where(a => a.TenantId == tenantId));
    }

    public async Task<IEnumerable<CampaignAllocation>> GetDeliveredAllocationsAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting delivered allocations for campaign: {CampaignId}", campaignId);
        return await Task.FromResult(_allocations.Where(a => a.CampaignId == campaignId && a.Status == AllocationStatus.Delivered));
    }

    public async Task<IEnumerable<CampaignAllocation>> GetSettledAllocationsAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting settled allocations for campaign: {CampaignId}", campaignId);
        return await Task.FromResult(_allocations.Where(a => a.CampaignId == campaignId && a.Status == AllocationStatus.Settled));
    }

    public async Task<int> GetAllocationCountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var count = _allocations.Count(a => a.CampaignId == campaignId);
        _logger.LogInformation("Allocation count for campaign {CampaignId}: {Count}", campaignId, count);
        return await Task.FromResult(count);
    }

    public async Task<decimal> GetTotalAllocatedAmountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var total = _allocations
            .Where(a => a.CampaignId == campaignId && a.Status == AllocationStatus.Allocated)
            .Sum(a => a.AllocatedAmount);
        
        _logger.LogInformation("Total allocated amount for campaign {CampaignId}: {TotalAmount}", campaignId, total);
        return await Task.FromResult(total);
    }

    public async Task<decimal> GetTotalDiscountAmountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var total = _allocations
            .Where(a => a.CampaignId == campaignId)
            .Sum(a => a.DiscountAmount);
        
        _logger.LogInformation("Total discount amount for campaign {CampaignId}: {TotalAmount}", campaignId, total);
        return await Task.FromResult(total);
    }

    public async Task<decimal> GetTotalFinalAmountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var total = _allocations
            .Where(a => a.CampaignId == campaignId)
            .Sum(a => a.FinalAmount);
        
        _logger.LogInformation("Total final amount for campaign {CampaignId}: {TotalAmount}", campaignId, total);
        return await Task.FromResult(total);
    }
}
