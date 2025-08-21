using MediatR;
using Collaboration.Application.Commands;
using Collaboration.Application.DTOs;
using Collaboration.Domain.Entities;
using Collaboration.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Collaboration.Application.Handlers;

/// <summary>
/// Handler for creating a new campaign
/// </summary>
public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ILogger<CreateCampaignCommandHandler> _logger;

    public CreateCampaignCommandHandler(
        ICampaignRepository campaignRepository,
        ILogger<CreateCampaignCommandHandler> logger)
    {
        _campaignRepository = campaignRepository;
        _logger = logger;
    }

    public async Task<CampaignDto> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating campaign: {CampaignName} for tenant: {TenantId}", 
            request.Name, request.TenantId);

        try
        {
            // Create the campaign entity
            var campaign = new Campaign(
                request.Name,
                request.Description,
                request.Type,
                request.StartDate,
                request.EndDate,
                request.MinParticipants,
                request.MaxParticipants,
                request.TargetAmount,
                request.DiscountPercentage,
                request.CreatedBy,
                request.TenantId
            );

            // Save to repository
            await _campaignRepository.AddAsync(campaign, cancellationToken);

            _logger.LogInformation("Campaign created successfully with ID: {CampaignId}", campaign.Id);

            // Return DTO
            return new CampaignDto
            {
                Id = campaign.Id,
                Name = campaign.Name,
                Description = campaign.Description,
                Status = campaign.Status,
                Type = campaign.Type,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                ActualEndDate = campaign.ActualEndDate,
                MinParticipants = campaign.MinParticipants,
                MaxParticipants = campaign.MaxParticipants,
                CurrentParticipants = campaign.CurrentParticipants,
                TargetAmount = campaign.TargetAmount,
                CurrentAmount = campaign.CurrentAmount,
                DiscountPercentage = campaign.DiscountPercentage,
                CreatedBy = campaign.CreatedBy,
                SupplierId = campaign.SupplierId,
                SupplierName = campaign.SupplierName,
                TenantId = campaign.TenantId,
                CreatedAt = campaign.CreatedAt,
                UpdatedAt = campaign.UpdatedAt,
                ProgressPercentage = campaign.ProgressPercentage,
                CanJoin = campaign.CanJoin,
                IsSuccessful = campaign.IsSuccessful,
                IsExpired = campaign.IsExpired
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating campaign: {CampaignName}", request.Name);
            throw;
        }
    }
}
