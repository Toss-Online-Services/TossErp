using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public record CreateUserConsentCommand(CreateUserConsentDto Dto) : IRequest<UserConsentDto>;

public class CreateUserConsentCommandHandler : IRequestHandler<CreateUserConsentCommand, UserConsentDto>
{
    private readonly IUserConsentRepository _consentRepository;
    private readonly ILogger<CreateUserConsentCommandHandler> _logger;

    public CreateUserConsentCommandHandler(
        IUserConsentRepository consentRepository,
        ILogger<CreateUserConsentCommandHandler> logger)
    {
        _consentRepository = consentRepository;
        _logger = logger;
    }

    public async Task<UserConsentDto> Handle(CreateUserConsentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Check if user already has an active consent for this type
        var existingConsent = await _consentRepository.GetActiveConsentAsync(
            dto.UserId, dto.ConsentType, dto.TenantId, cancellationToken);

        if (existingConsent != null)
        {
            _logger.LogWarning("User {UserId} already has active consent for type {ConsentType}", 
                dto.UserId, dto.ConsentType);
            return MapToDto(existingConsent);
        }

        // Create new consent
        var consent = new UserConsent(
            dto.UserId,
            dto.ConsentType,
            dto.IpAddress,
            dto.UserAgent,
            dto.TenantId,
            dto.ExpiresAt);

        await _consentRepository.AddAsync(consent, cancellationToken);

        _logger.LogInformation("Created consent for user {UserId}, type {ConsentType}", 
            dto.UserId, dto.ConsentType);

        return MapToDto(consent);
    }

    private static UserConsentDto MapToDto(UserConsent consent)
    {
        return new UserConsentDto(
            consent.Id,
            consent.UserId,
            consent.ConsentType,
            consent.IsGranted,
            consent.GrantedAt,
            consent.RevokedAt,
            consent.RevokedBy,
            consent.RevocationReason,
            consent.IpAddress,
            consent.UserAgent,
            consent.TenantId,
            consent.ExpiresAt,
            consent.IsActive);
    }
}
