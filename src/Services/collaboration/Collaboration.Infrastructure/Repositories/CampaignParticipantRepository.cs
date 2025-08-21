namespace Collaboration.Infrastructure.Repositories;

public class CampaignParticipantRepository : ICampaignParticipantRepository
{
    private readonly List<CampaignParticipant> _participants = new();
    private readonly ILogger<CampaignParticipantRepository> _logger;

    public CampaignParticipantRepository(ILogger<CampaignParticipantRepository> logger)
    {
        _logger = logger;
    }

    public async Task<CampaignParticipant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting participant with ID: {ParticipantId}", id);
        return await Task.FromResult(_participants.FirstOrDefault(p => p.Id == id));
    }

    public async Task<IEnumerable<CampaignParticipant>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting participants for campaign: {CampaignId}", campaignId);
        return await Task.FromResult(_participants.Where(p => p.CampaignId == campaignId));
    }

    public async Task<IEnumerable<CampaignParticipant>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting participants for user: {UserId}", userId);
        return await Task.FromResult(_participants.Where(p => p.UserId == userId));
    }

    public async Task<IEnumerable<CampaignParticipant>> GetByStatusAsync(ParticipantStatus status, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting participants with status: {Status}", status);
        return await Task.FromResult(_participants.Where(p => p.Status == status));
    }

    public async Task<CampaignParticipant> AddAsync(CampaignParticipant participant, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new participant: {ParticipantId} for campaign: {CampaignId}", participant.Id, participant.CampaignId);
        _participants.Add(participant);
        return await Task.FromResult(participant);
    }

    public async Task<CampaignParticipant> UpdateAsync(CampaignParticipant participant, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating participant: {ParticipantId}", participant.Id);
        var existingIndex = _participants.FindIndex(p => p.Id == participant.Id);
        if (existingIndex >= 0)
        {
            _participants[existingIndex] = participant;
        }
        return await Task.FromResult(participant);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting participant: {ParticipantId}", id);
        var participant = _participants.FirstOrDefault(p => p.Id == id);
        if (participant != null)
        {
            _participants.Remove(participant);
        }
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_participants.Any(p => p.Id == id));
    }

    public async Task<IEnumerable<CampaignParticipant>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting participants for tenant: {TenantId}", tenantId);
        return await Task.FromResult(_participants.Where(p => p.TenantId == tenantId));
    }

    public async Task<IEnumerable<CampaignParticipant>> GetActiveParticipantsAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active participants for campaign: {CampaignId}", campaignId);
        return await Task.FromResult(_participants.Where(p => p.CampaignId == campaignId && p.Status == ParticipantStatus.Joined));
    }

    public async Task<IEnumerable<CampaignParticipant>> GetParticipantsWithPaymentAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting participants with payment for campaign: {CampaignId}", campaignId);
        return await Task.FromResult(_participants.Where(p => p.CampaignId == campaignId && p.Status == ParticipantStatus.PaymentConfirmed));
    }

    public async Task<bool> IsUserParticipantAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_participants.Any(p => p.UserId == userId && p.CampaignId == campaignId));
    }

    public async Task<int> GetParticipantCountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var count = _participants.Count(p => p.CampaignId == campaignId);
        _logger.LogInformation("Participant count for campaign {CampaignId}: {Count}", campaignId, count);
        return await Task.FromResult(count);
    }

    public async Task<decimal> GetTotalCommittedAmountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var total = _participants.Where(p => p.CampaignId == campaignId).Sum(p => p.CommittedAmount);
        _logger.LogInformation("Total committed amount for campaign {CampaignId}: {Total}", campaignId, total);
        return await Task.FromResult(total);
    }

    public async Task<decimal> GetTotalPaymentAmountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var total = _participants.Where(p => p.CampaignId == campaignId && p.Status == ParticipantStatus.PaymentConfirmed).Sum(p => p.CommittedAmount);
        _logger.LogInformation("Total payment amount for campaign {CampaignId}: {Total}", campaignId, total);
        return await Task.FromResult(total);
    }
}
