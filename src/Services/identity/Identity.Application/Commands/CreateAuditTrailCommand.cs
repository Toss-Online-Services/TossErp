using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public record CreateAuditTrailCommand(CreateAuditTrailDto Dto) : IRequest<AuditTrailDto>;

public class CreateAuditTrailCommandHandler : IRequestHandler<CreateAuditTrailCommand, AuditTrailDto>
{
    private readonly IAuditTrailRepository _auditTrailRepository;
    private readonly ILogger<CreateAuditTrailCommandHandler> _logger;

    public CreateAuditTrailCommandHandler(
        IAuditTrailRepository auditTrailRepository,
        ILogger<CreateAuditTrailCommandHandler> logger)
    {
        _auditTrailRepository = auditTrailRepository;
        _logger = logger;
    }

    public async Task<AuditTrailDto> Handle(CreateAuditTrailCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var auditTrail = new AuditTrail(
            dto.UserId,
            dto.Action,
            dto.EntityType,
            dto.EntityId,
            dto.IpAddress,
            dto.UserAgent,
            dto.TenantId,
            dto.OldValues,
            dto.NewValues,
            dto.SessionId,
            dto.CorrelationId);

        await _auditTrailRepository.AddAsync(auditTrail, cancellationToken);

        _logger.LogDebug("Created audit trail for user {UserId}, action {Action}, entity {EntityType}:{EntityId}", 
            dto.UserId, dto.Action, dto.EntityType, dto.EntityId);

        return MapToDto(auditTrail);
    }

    private static AuditTrailDto MapToDto(AuditTrail auditTrail)
    {
        return new AuditTrailDto(
            auditTrail.Id,
            auditTrail.UserId,
            auditTrail.Action,
            auditTrail.EntityType,
            auditTrail.EntityId,
            auditTrail.OldValues,
            auditTrail.NewValues,
            auditTrail.IpAddress,
            auditTrail.UserAgent,
            auditTrail.TenantId,
            auditTrail.Timestamp,
            auditTrail.SessionId,
            auditTrail.CorrelationId);
    }
}
