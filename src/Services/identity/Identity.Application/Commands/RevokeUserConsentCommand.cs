using Identity.Application.DTOs;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public record RevokeUserConsentCommand(RevokeUserConsentDto Dto) : IRequest<bool>;

public class RevokeUserConsentCommandHandler : IRequestHandler<RevokeUserConsentCommand, bool>
{
    private readonly IUserConsentRepository _consentRepository;
    private readonly ILogger<RevokeUserConsentCommandHandler> _logger;

    public RevokeUserConsentCommandHandler(
        IUserConsentRepository consentRepository,
        ILogger<RevokeUserConsentCommandHandler> logger)
    {
        _consentRepository = consentRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(RevokeUserConsentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var consent = await _consentRepository.GetByIdAsync(dto.ConsentId, cancellationToken);
        if (consent == null)
        {
            _logger.LogWarning("Consent {ConsentId} not found", dto.ConsentId);
            return false;
        }

        if (!consent.IsGranted)
        {
            _logger.LogWarning("Consent {ConsentId} is already revoked", dto.ConsentId);
            return false;
        }

        consent.Revoke(dto.RevokedBy, dto.Reason);
        await _consentRepository.UpdateAsync(consent, cancellationToken);

        _logger.LogInformation("Revoked consent {ConsentId} by {RevokedBy}", 
            dto.ConsentId, dto.RevokedBy);

        return true;
    }
}
