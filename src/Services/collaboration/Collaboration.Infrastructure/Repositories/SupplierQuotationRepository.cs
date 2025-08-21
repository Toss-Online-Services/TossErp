namespace Collaboration.Infrastructure.Repositories;

public class SupplierQuotationRepository : ISupplierQuotationRepository
{
    private readonly List<SupplierQuotation> _quotations = new();
    private readonly ILogger<SupplierQuotationRepository> _logger;

    public SupplierQuotationRepository(ILogger<SupplierQuotationRepository> logger)
    {
        _logger = logger;
    }

    public async Task<SupplierQuotation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting supplier quotation with ID: {QuotationId}", id);
        return await Task.FromResult(_quotations.FirstOrDefault(q => q.Id == id));
    }

    public async Task<IEnumerable<SupplierQuotation>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting quotations for campaign: {CampaignId}", campaignId);
        return await Task.FromResult(_quotations.Where(q => q.CampaignId == campaignId));
    }

    public async Task<IEnumerable<SupplierQuotation>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting quotations for supplier: {SupplierId}", supplierId);
        return await Task.FromResult(_quotations.Where(q => q.SupplierId == supplierId));
    }

    public async Task<IEnumerable<SupplierQuotation>> GetByStatusAsync(QuotationStatus status, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting quotations with status: {Status}", status);
        return await Task.FromResult(_quotations.Where(q => q.Status == status));
    }

    public async Task<IEnumerable<SupplierQuotation>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting quotations for tenant: {TenantId}", tenantId);
        return await Task.FromResult(_quotations.Where(q => q.TenantId == tenantId));
    }

    public async Task<IEnumerable<SupplierQuotation>> GetActiveQuotationsAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active quotations for campaign: {CampaignId}", campaignId);
        return await Task.FromResult(_quotations.Where(q => q.CampaignId == campaignId && q.Status == QuotationStatus.Submitted && q.ValidUntil > DateTime.UtcNow));
    }

    public async Task<IEnumerable<SupplierQuotation>> GetExpiredQuotationsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting expired quotations");
        return await Task.FromResult(_quotations.Where(q => q.ValidUntil <= DateTime.UtcNow || q.Status == QuotationStatus.Expired));
    }

    public async Task<IEnumerable<SupplierQuotation>> GetAcceptedQuotationsAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting accepted quotations for campaign: {CampaignId}", campaignId);
        return await Task.FromResult(_quotations.Where(q => q.CampaignId == campaignId && q.Status == QuotationStatus.Accepted));
    }

    public async Task<SupplierQuotation> AddAsync(SupplierQuotation quotation, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new supplier quotation: {QuotationId} for campaign: {CampaignId}", quotation.Id, quotation.CampaignId);
        _quotations.Add(quotation);
        return await Task.FromResult(quotation);
    }

    public async Task<SupplierQuotation> UpdateAsync(SupplierQuotation quotation, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating supplier quotation: {QuotationId}", quotation.Id);
        var existingIndex = _quotations.FindIndex(q => q.Id == quotation.Id);
        if (existingIndex >= 0)
        {
            _quotations[existingIndex] = quotation;
        }
        return await Task.FromResult(quotation);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting supplier quotation: {QuotationId}", id);
        var quotation = _quotations.FirstOrDefault(q => q.Id == id);
        if (quotation != null)
        {
            _quotations.Remove(quotation);
        }
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_quotations.Any(q => q.Id == id));
    }

    public async Task<bool> HasSupplierQuotationAsync(Guid campaignId, Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_quotations.Any(q => q.SupplierId == supplierId && q.CampaignId == campaignId));
    }

    public async Task<int> GetQuotationCountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var count = _quotations.Count(q => q.CampaignId == campaignId);
        _logger.LogInformation("Quotation count for campaign {CampaignId}: {Count}", campaignId, count);
        return await Task.FromResult(count);
    }

    public async Task<decimal> GetLowestUnitPriceAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var activeQuotations = _quotations.Where(q => q.CampaignId == campaignId && q.Status == QuotationStatus.Submitted);
        if (!activeQuotations.Any())
        {
            _logger.LogInformation("No active quotations found for campaign {CampaignId}", campaignId);
            return 0;
        }
        
        var lowestPrice = activeQuotations.Min(q => q.UnitPrice);
        _logger.LogInformation("Lowest unit price for campaign {CampaignId}: {Price}", campaignId, lowestPrice);
        return await Task.FromResult(lowestPrice);
    }

    public async Task<decimal> GetHighestDiscountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var activeQuotations = _quotations.Where(q => q.CampaignId == campaignId && q.Status == QuotationStatus.Submitted);
        if (!activeQuotations.Any())
        {
            _logger.LogInformation("No active quotations found for campaign {CampaignId}", campaignId);
            return 0;
        }
        
        var highestDiscount = activeQuotations.Max(q => q.BulkDiscountPercentage);
        _logger.LogInformation("Highest discount for campaign {CampaignId}: {Discount}%", campaignId, highestDiscount);
        return await Task.FromResult(highestDiscount);
    }

    public async Task<IEnumerable<SupplierQuotation>> GetBestPriceQuotationsAsync(Guid campaignId, int quantity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting best price quotations for campaign: {CampaignId} with quantity: {Quantity}", campaignId, quantity);
        var activeQuotations = _quotations
            .Where(q => q.CampaignId == campaignId && 
                       q.Status == QuotationStatus.Submitted && 
                       q.ValidUntil > DateTime.UtcNow &&
                       q.MinQuantity <= quantity && 
                       q.MaxQuantity >= quantity)
            .OrderBy(q => q.UnitPrice)
            .ToList();

        return await Task.FromResult(activeQuotations);
    }
}
