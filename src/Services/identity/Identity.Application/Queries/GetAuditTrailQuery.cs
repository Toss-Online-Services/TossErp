using Identity.Application.DTOs;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

public record GetAuditTrailQuery(AuditTrailFilterDto Filter) : IRequest<IEnumerable<AuditTrailDto>>;

public class GetAuditTrailQueryHandler : IRequestHandler<GetAuditTrailQuery, IEnumerable<AuditTrailDto>>
{
    private readonly IAuditTrailRepository _auditTrailRepository;
    private readonly ILogger<GetAuditTrailQueryHandler> _logger;

    public GetAuditTrailQueryHandler(
        IAuditTrailRepository auditTrailRepository,
        ILogger<GetAuditTrailQueryHandler> logger)
    {
        _auditTrailRepository = auditTrailRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AuditTrailDto>> Handle(GetAuditTrailQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        IEnumerable<Identity.Domain.Entities.AuditTrail> auditTrails;

        if (filter.UserId.HasValue)
        {
            auditTrails = await _auditTrailRepository.GetUserAuditTrailAsync(
                filter.UserId.Value, filter.TenantId, filter.FromDate, filter.ToDate, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(filter.EntityType) && !string.IsNullOrEmpty(filter.EntityId))
        {
            auditTrails = await _auditTrailRepository.GetEntityAuditTrailAsync(
                filter.EntityType, filter.EntityId, filter.TenantId, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(filter.Action))
        {
            auditTrails = await _auditTrailRepository.GetAuditTrailByActionAsync(
                filter.Action, filter.TenantId, filter.FromDate, filter.ToDate, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(filter.SessionId))
        {
            auditTrails = await _auditTrailRepository.GetAuditTrailBySessionAsync(
                filter.SessionId, filter.TenantId, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(filter.CorrelationId))
        {
            auditTrails = await _auditTrailRepository.GetAuditTrailByCorrelationAsync(
                filter.CorrelationId, filter.TenantId, cancellationToken);
        }
        else
        {
            // Default to user audit trail if no specific filter is provided
            auditTrails = await _auditTrailRepository.GetUserAuditTrailAsync(
                Guid.Empty, filter.TenantId, filter.FromDate, filter.ToDate, cancellationToken);
        }

        // Apply pagination
        var pagedAuditTrails = auditTrails
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        _logger.LogDebug("Retrieved {Count} audit trail entries for tenant {TenantId}", 
            pagedAuditTrails.Count(), filter.TenantId);

        return pagedAuditTrails.Select(MapToDto);
    }

    private static AuditTrailDto MapToDto(Identity.Domain.Entities.AuditTrail auditTrail)
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
