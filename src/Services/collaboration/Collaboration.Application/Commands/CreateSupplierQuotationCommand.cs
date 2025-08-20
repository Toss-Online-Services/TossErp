using Collaboration.Domain.Entities;
using Collaboration.Domain.Repositories;

namespace Collaboration.Application.Commands;

/// <summary>
/// Command to create a new supplier quotation
/// </summary>
public record CreateSupplierQuotationCommand : IRequest<QuotationResponseDto>
{
    public Guid CampaignId { get; init; }
    public Guid SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public string SupplierEmail { get; init; } = string.Empty;
    public string SupplierPhone { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public decimal BulkDiscountPercentage { get; init; }
    public int MinQuantity { get; init; }
    public int MaxQuantity { get; init; }
    public string ProductDescription { get; init; } = string.Empty;
    public string? ProductSpecifications { get; init; }
    public string? TermsAndConditions { get; init; }
    public DateTime ValidUntil { get; init; }
    public Guid TenantId { get; init; }
}

/// <summary>
/// Handler for creating supplier quotations
/// </summary>
public class CreateSupplierQuotationCommandHandler : IRequestHandler<CreateSupplierQuotationCommand, QuotationResponseDto>
{
    private readonly ISupplierQuotationRepository _quotationRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly ILogger<CreateSupplierQuotationCommandHandler> _logger;

    public CreateSupplierQuotationCommandHandler(
        ISupplierQuotationRepository quotationRepository,
        ICampaignRepository campaignRepository,
        ILogger<CreateSupplierQuotationCommandHandler> logger)
    {
        _quotationRepository = quotationRepository;
        _campaignRepository = campaignRepository;
        _logger = logger;
    }

    public async Task<QuotationResponseDto> Handle(CreateSupplierQuotationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate campaign exists
            var campaign = await _campaignRepository.GetByIdAsync(request.CampaignId, cancellationToken);
            if (campaign == null)
            {
                return new QuotationResponseDto
                {
                    Success = false,
                    Message = "Campaign not found",
                    Errors = new List<string> { $"Campaign with ID {request.CampaignId} does not exist" }
                };
            }

            // Check if supplier already has a quotation for this campaign
            var existingQuotation = await _quotationRepository.HasSupplierQuotationAsync(request.CampaignId, request.SupplierId, cancellationToken);
            if (existingQuotation)
            {
                return new QuotationResponseDto
                {
                    Success = false,
                    Message = "Supplier already has a quotation for this campaign",
                    Errors = new List<string> { "Supplier has already submitted a quotation for this campaign" }
                };
            }

            // Create the quotation
            var quotation = new SupplierQuotation(
                request.CampaignId,
                request.SupplierId,
                request.SupplierName,
                request.SupplierEmail,
                request.SupplierPhone,
                request.UnitPrice,
                request.BulkDiscountPercentage,
                request.MinQuantity,
                request.MaxQuantity,
                request.ProductDescription,
                request.ProductSpecifications,
                request.TermsAndConditions,
                request.ValidUntil,
                request.TenantId);

            var savedQuotation = await _quotationRepository.AddAsync(quotation, cancellationToken);

            // Map to DTO
            var quotationDto = new SupplierQuotationDto
            {
                Id = savedQuotation.Id,
                CampaignId = savedQuotation.CampaignId,
                SupplierId = savedQuotation.SupplierId,
                SupplierName = savedQuotation.SupplierName,
                SupplierEmail = savedQuotation.SupplierEmail,
                SupplierPhone = savedQuotation.SupplierPhone,
                Status = savedQuotation.Status,
                UnitPrice = savedQuotation.UnitPrice,
                BulkDiscountPercentage = savedQuotation.BulkDiscountPercentage,
                MinQuantity = savedQuotation.MinQuantity,
                MaxQuantity = savedQuotation.MaxQuantity,
                ProductDescription = savedQuotation.ProductDescription,
                ProductSpecifications = savedQuotation.ProductSpecifications,
                TermsAndConditions = savedQuotation.TermsAndConditions,
                ValidUntil = savedQuotation.ValidUntil,
                AcceptedAt = savedQuotation.AcceptedAt,
                AcceptedBy = savedQuotation.AcceptedBy,
                RejectionReason = savedQuotation.RejectionReason,
                RejectedAt = savedQuotation.RejectedAt,
                RejectedBy = savedQuotation.RejectedBy,
                CreatedAt = savedQuotation.CreatedAt,
                UpdatedAt = savedQuotation.UpdatedAt,
                IsExpired = savedQuotation.IsExpired,
                DiscountedPrice = savedQuotation.CalculateDiscountedPrice(savedQuotation.MinQuantity)
            };

            _logger.LogInformation("Created supplier quotation {QuotationId} for campaign {CampaignId} by supplier {SupplierName}",
                savedQuotation.Id, request.CampaignId, request.SupplierName);

            return new QuotationResponseDto
            {
                Success = true,
                Message = "Supplier quotation created successfully",
                Quotation = quotationDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier quotation for campaign {CampaignId} by supplier {SupplierName}",
                request.CampaignId, request.SupplierName);

            return new QuotationResponseDto
            {
                Success = false,
                Message = "Failed to create supplier quotation",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
