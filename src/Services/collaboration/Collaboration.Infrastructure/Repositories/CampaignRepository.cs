namespace Collaboration.Infrastructure.Repositories;

public class CampaignRepository : ICampaignRepository
{
    private readonly List<Campaign> _campaigns = new();
    private readonly ILogger<CampaignRepository> _logger;

    public CampaignRepository(ILogger<CampaignRepository> logger)
    {
        _logger = logger;
    }

    public async Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting campaign with ID: {CampaignId}", id);
        return await Task.FromResult(_campaigns.FirstOrDefault(c => c.Id == id));
    }

    public async Task<IEnumerable<Campaign>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all campaigns");
        return await Task.FromResult(_campaigns.AsEnumerable());
    }

    public async Task<IEnumerable<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting campaigns with status: {Status}", status);
        return await Task.FromResult(_campaigns.Where(c => c.Status == status));
    }

    public async Task<IEnumerable<Campaign>> GetByTypeAsync(CampaignType type, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting campaigns with type: {Type}", type);
        return await Task.FromResult(_campaigns.Where(c => c.Type == type));
    }

    public async Task<IEnumerable<Campaign>> GetByCreatorAsync(Guid createdBy, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting campaigns by creator: {CreatedBy}", createdBy);
        return await Task.FromResult(_campaigns.Where(c => c.CreatedBy == createdBy));
    }

    public async Task<IEnumerable<Campaign>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting campaigns by tenant: {TenantId}", tenantId);
        return await Task.FromResult(_campaigns.Where(c => c.TenantId == tenantId));
    }

    public async Task<IEnumerable<Campaign>> GetActiveCampaignsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active campaigns");
        return await Task.FromResult(_campaigns.Where(c => c.Status == CampaignStatus.Active));
    }

    public async Task<IEnumerable<Campaign>> GetExpiredCampaignsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting expired campaigns");
        return await Task.FromResult(_campaigns.Where(c => c.Status == CampaignStatus.Expired));
    }

    public async Task<IEnumerable<Campaign>> GetSuccessfulCampaignsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting successful campaigns");
        return await Task.FromResult(_campaigns.Where(c => c.Status == CampaignStatus.Completed));
    }

    public async Task<IEnumerable<Campaign>> GetCampaignsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting campaigns by date range: {StartDate} to {EndDate}", startDate, endDate);
        return await Task.FromResult(_campaigns.Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate));
    }

    public async Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new campaign: {CampaignId}", campaign.Id);
        _campaigns.Add(campaign);
        return await Task.FromResult(campaign);
    }

    public async Task<Campaign> UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating campaign: {CampaignId}", campaign.Id);
        var existingIndex = _campaigns.FindIndex(c => c.Id == campaign.Id);
        if (existingIndex >= 0)
        {
            _campaigns[existingIndex] = campaign;
        }
        return await Task.FromResult(campaign);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting campaign: {CampaignId}", id);
        var campaign = _campaigns.FirstOrDefault(c => c.Id == id);
        if (campaign != null)
        {
            _campaigns.Remove(campaign);
        }
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_campaigns.Any(c => c.Id == id));
    }

    public async Task<int> GetCountByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default)
    {
        var count = _campaigns.Count(c => c.Status == status);
        _logger.LogInformation("Count of campaigns with status {Status}: {Count}", status, count);
        return await Task.FromResult(count);
    }

    public async Task<decimal> GetTotalAmountByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default)
    {
        var total = _campaigns.Where(c => c.Status == status).Sum(c => c.TargetAmount);
        _logger.LogInformation("Total amount of campaigns with status {Status}: {Total}", status, total);
        return await Task.FromResult(total);
    }
}
