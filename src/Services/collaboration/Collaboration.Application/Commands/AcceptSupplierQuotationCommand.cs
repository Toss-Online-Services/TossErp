using Collaboration.Domain.Entities;
using Collaboration.Domain.Repositories;

namespace Collaboration.Application.Commands;

/// <summary>
/// Command to accept a supplier quotation
/// </summary>
public record AcceptSupplierQuotationCommand : IRequest<QuotationResponseDto>
{
    public Guid QuotationId { get; init; }
    public Guid AcceptedBy { get; init; }
    public Guid TenantId { get; init; }
}

/// <summary>
/// Handler for accepting supplier quotations
/// </summary>
public class AcceptSupplierQuotationCommandHandler : IRequestHandler<AcceptSupplierQuotationCommand, QuotationResponseDto>
{
    private readonly ISupplierQuotationRepository _quotationRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly ILogger<AcceptSupplierQuotationCommandHandler> _logger;

    public AcceptSupplierQuotationCommandHandler(
        ISupplierQuotationRepository quotationRepository,
        ICampaignRepository campaignRepository,
        ILogger<AcceptSupplierQuotationCommandHandler> logger)
    {
        _quotationRepository = quotationRepository;
        _campaignRepository = campaignRepository;
        _logger = logger;
    }

    public async Task<QuotationResponseDto> Handle(AcceptSupplierQuotationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the quotation
            var quotation = await _quotationRepository.GetByIdAsync(request.QuotationId, cancellationToken);
            if (quotation == null)
            {
                return new QuotationResponseDto
                {
                    Success = false,
                    Message = "Quotation not found",
                    Errors = new List<string> { $"Quotation with ID {request.QuotationId} does not exist" }
                };
            }

            // Check if quotation can be accepted
            if (!quotation.CanAccept)
            {
                var errorMessage = quotation.IsExpired ? "Quotation has expired" : "Quotation cannot be accepted in its current status";
                return new QuotationResponseDto
                {
                    Success = false,
                    Message = errorMessage,
                    Errors = new List<string> { errorMessage }
                };
            }

            // Accept the quotation
            quotation.Accept(request.AcceptedBy);
            var updatedQuotation = await _quotationRepository.UpdateAsync(quotation, cancellationToken);

            // Update campaign with selected supplier
            var campaign = await _campaignRepository.GetByIdAsync(quotation.CampaignId, cancellationToken);
            if (campaign != null)
            {
                campaign.SelectSupplier(quotation.SupplierId, quotation.SupplierName);
                await _campaignRepository.UpdateAsync(campaign, cancellationToken);
            }

            // Map to DTO
            var quotationDto = new SupplierQuotationDto
            {
                Id = updatedQuotation.Id,
                CampaignId = updatedQuotation.CampaignId,
                SupplierId = updatedQuotation.SupplierId,
                SupplierName = updatedQuotation.SupplierName,
                SupplierEmail = updatedQuotation.SupplierEmail,
                SupplierPhone = updatedQuotation.SupplierPhone,
                Status = updatedQuotation.Status,
                UnitPrice = updatedQuotation.UnitPrice,
                BulkDiscountPercentage = updatedQuotation.BulkDiscountPercentage,
                MinQuantity = updatedQuotation.MinQuantity,
                MaxQuantity = updatedQuotation.MaxQuantity,
                ProductDescription = updatedQuotation.ProductDescription,
                ProductSpecifications = updatedQuotation.ProductSpecifications,
                TermsAndConditions = updatedQuotation.TermsAndConditions,
                ValidUntil = updatedQuotation.ValidUntil,
                AcceptedAt = updatedQuotation.AcceptedAt,
                AcceptedBy = updatedQuotation.AcceptedBy,
                RejectionReason = updatedQuotation.RejectionReason,
                RejectedAt = updatedQuotation.RejectedAt,
                RejectedBy = updatedQuotation.RejectedBy,
                CreatedAt = updatedQuotation.CreatedAt,
                UpdatedAt = updatedQuotation.UpdatedAt,
                IsExpired = updatedQuotation.IsExpired,
                DiscountedPrice = updatedQuotation.CalculateDiscountedPrice(updatedQuotation.MinQuantity)
            };

            _logger.LogInformation("Accepted supplier quotation {QuotationId} for campaign {CampaignId} by {AcceptedBy}",
                updatedQuotation.Id, updatedQuotation.CampaignId, request.AcceptedBy);

            return new QuotationResponseDto
            {
                Success = true,
                Message = "Supplier quotation accepted successfully",
                Quotation = quotationDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting supplier quotation {QuotationId} by {AcceptedBy}",
                request.QuotationId, request.AcceptedBy);

            return new QuotationResponseDto
            {
                Success = false,
                Message = "Failed to accept supplier quotation",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
