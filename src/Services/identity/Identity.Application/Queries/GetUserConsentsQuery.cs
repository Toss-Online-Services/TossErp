using Identity.Application.DTOs;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public record GetUserConsentsQuery(Guid UserId, string TenantId) : IRequest<IEnumerable<UserConsentDto>>;

public class GetUserConsentsQueryHandler : IRequestHandler<GetUserConsentsQuery, IEnumerable<UserConsentDto>>
{
    private readonly IUserConsentRepository _consentRepository;
    private readonly ILogger<GetUserConsentsQueryHandler> _logger;

    public GetUserConsentsQueryHandler(
        IUserConsentRepository consentRepository,
        ILogger<GetUserConsentsQueryHandler> logger)
    {
        _consentRepository = consentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<UserConsentDto>> Handle(GetUserConsentsQuery request, CancellationToken cancellationToken)
    {
        var consents = await _consentRepository.GetUserConsentsAsync(
            request.UserId, request.TenantId, cancellationToken);

        _logger.LogDebug("Retrieved {Count} consents for user {UserId}", 
            consents.Count(), request.UserId);

        return consents.Select(MapToDto);
    }

    private static UserConsentDto MapToDto(Identity.Domain.Entities.UserConsent consent)
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
