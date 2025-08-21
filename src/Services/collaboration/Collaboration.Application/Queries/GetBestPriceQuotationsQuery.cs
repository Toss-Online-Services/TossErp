using Collaboration.Domain.Entities;
using Collaboration.Domain.Repositories;

namespace Collaboration.Application.Queries;

/// <summary>
/// Query to get best-price quotations for a campaign
/// </summary>
public record GetBestPriceQuotationsQuery : IRequest<QuotationComparisonDto>
{
    public Guid CampaignId { get; init; }
    public int RequestedQuantity { get; init; }
    public Guid TenantId { get; init; }
}

/// <summary>
/// Handler for getting best-price quotations
/// </summary>
public class GetBestPriceQuotationsQueryHandler : IRequestHandler<GetBestPriceQuotationsQuery, QuotationComparisonDto>
{
    private readonly ISupplierQuotationRepository _quotationRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly ILogger<GetBestPriceQuotationsQueryHandler> _logger;

    public GetBestPriceQuotationsQueryHandler(
        ISupplierQuotationRepository quotationRepository,
        ICampaignRepository campaignRepository,
        ILogger<GetBestPriceQuotationsQueryHandler> logger)
    {
        _quotationRepository = quotationRepository;
        _campaignRepository = campaignRepository;
        _logger = logger;
    }

    public async Task<QuotationComparisonDto> Handle(GetBestPriceQuotationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate campaign exists
            var campaign = await _campaignRepository.GetByIdAsync(request.CampaignId, cancellationToken);
            if (campaign == null)
            {
                _logger.LogWarning("Campaign {CampaignId} not found for quotation comparison", request.CampaignId);
                return new QuotationComparisonDto
                {
                    CampaignId = request.CampaignId,
                    RequestedQuantity = request.RequestedQuantity
                };
            }

            // Get all active quotations for the campaign
            var quotations = await _quotationRepository.GetActiveQuotationsAsync(request.CampaignId, cancellationToken);
            var validQuotations = quotations.Where(q => !q.IsExpired).ToList();

            if (!validQuotations.Any())
            {
                _logger.LogInformation("No valid quotations found for campaign {CampaignId}", request.CampaignId);
                return new QuotationComparisonDto
                {
                    CampaignId = request.CampaignId,
                    RequestedQuantity = request.RequestedQuantity
                };
            }

            // Filter quotations that can handle the requested quantity
            var suitableQuotations = validQuotations
                .Where(q => q.MinQuantity <= request.RequestedQuantity && q.MaxQuantity >= request.RequestedQuantity)
                .ToList();

            if (!suitableQuotations.Any())
            {
                _logger.LogInformation("No quotations can handle requested quantity {Quantity} for campaign {CampaignId}", 
                    request.RequestedQuantity, request.CampaignId);
                return new QuotationComparisonDto
                {
                    CampaignId = request.CampaignId,
                    RequestedQuantity = request.RequestedQuantity
                };
            }

            // Calculate prices for each quotation
            var quotationDtos = new List<SupplierQuotationDto>();
            var totalPrices = new List<decimal>();
            var unitPrices = new List<decimal>();
            var discounts = new List<decimal>();

            foreach (var quotation in suitableQuotations)
            {
                var totalPrice = quotation.CalculateTotalPrice(request.RequestedQuantity);
                var discountedUnitPrice = quotation.CalculateDiscountedPrice(request.RequestedQuantity);

                totalPrices.Add(totalPrice);
                unitPrices.Add(discountedUnitPrice);
                discounts.Add(quotation.BulkDiscountPercentage);

                var quotationDto = new SupplierQuotationDto
                {
                    Id = quotation.Id,
                    CampaignId = quotation.CampaignId,
                    SupplierId = quotation.SupplierId,
                    SupplierName = quotation.SupplierName,
                    SupplierEmail = quotation.SupplierEmail,
                    SupplierPhone = quotation.SupplierPhone,
                    Status = quotation.Status,
                    UnitPrice = quotation.UnitPrice,
                    BulkDiscountPercentage = quotation.BulkDiscountPercentage,
                    MinQuantity = quotation.MinQuantity,
                    MaxQuantity = quotation.MaxQuantity,
                    ProductDescription = quotation.ProductDescription,
                    ProductSpecifications = quotation.ProductSpecifications,
                    TermsAndConditions = quotation.TermsAndConditions,
                    ValidUntil = quotation.ValidUntil,
                    AcceptedAt = quotation.AcceptedAt,
                    AcceptedBy = quotation.AcceptedBy,
                    RejectionReason = quotation.RejectionReason,
                    RejectedAt = quotation.RejectedAt,
                    RejectedBy = quotation.RejectedBy,
                    CreatedAt = quotation.CreatedAt,
                    UpdatedAt = quotation.UpdatedAt,
                    IsExpired = quotation.IsExpired,
                    DiscountedPrice = discountedUnitPrice
                };

                quotationDtos.Add(quotationDto);
            }

            // Find the best price quotation
            var bestPriceIndex = totalPrices.IndexOf(totalPrices.Min());
            var bestPriceQuotation = quotationDtos[bestPriceIndex];
            var totalBestPrice = totalPrices[bestPriceIndex];

            // Calculate savings compared to average price
            var averageUnitPrice = unitPrices.Average();
            var totalSavings = (averageUnitPrice - bestPriceQuotation.DiscountedPrice) * request.RequestedQuantity;
            var highestDiscount = discounts.Max();

            var result = new QuotationComparisonDto
            {
                CampaignId = request.CampaignId,
                RequestedQuantity = request.RequestedQuantity,
                Quotations = quotationDtos,
                BestPriceQuotation = bestPriceQuotation,
                TotalBestPrice = totalBestPrice,
                TotalSavings = totalSavings,
                AverageUnitPrice = averageUnitPrice,
                HighestDiscount = highestDiscount
            };

            _logger.LogInformation("Found {Count} suitable quotations for campaign {CampaignId}, best price: {BestPrice:C}",
                suitableQuotations.Count, request.CampaignId, totalBestPrice);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting best price quotations for campaign {CampaignId}", request.CampaignId);
            
            return new QuotationComparisonDto
            {
                CampaignId = request.CampaignId,
                RequestedQuantity = request.RequestedQuantity
            };
        }
    }
}
