using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Assets.Queries.GetAssetMaintenanceLogs;

public record GetAssetMaintenanceLogsQuery : IRequest<PaginatedList<MaintenanceLogDto>>
{
    public int AssetId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetAssetMaintenanceLogsQueryHandler : IRequestHandler<GetAssetMaintenanceLogsQuery, PaginatedList<MaintenanceLogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetAssetMaintenanceLogsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<MaintenanceLogDto>> Handle(GetAssetMaintenanceLogsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Verify asset exists and belongs to business
        var assetExists = await _context.Assets
            .AnyAsync(a => a.Id == request.AssetId
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (!assetExists)
        {
            throw new NotFoundException("Asset", request.AssetId.ToString());
        }

        var query = _context.AssetMaintenanceLogs
            .Where(m => m.AssetId == request.AssetId
                && m.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .OrderByDescending(m => m.MaintenanceDate);

        var logQuery = query
            .Select(m => new MaintenanceLogDto
            {
                Id = m.Id,
                AssetId = m.AssetId,
                MaintenanceDate = m.MaintenanceDate,
                MaintenanceType = m.MaintenanceType,
                Description = m.Description,
                Cost = m.Cost,
                ServiceProvider = m.ServiceProvider,
                NextMaintenanceDate = m.NextMaintenanceDate,
                Notes = m.Notes,
                Created = m.Created
            });

        return await PaginatedList<MaintenanceLogDto>.CreateAsync(logQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record MaintenanceLogDto
{
    public int Id { get; init; }
    public int AssetId { get; init; }
    public DateTimeOffset MaintenanceDate { get; init; }
    public string MaintenanceType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal? Cost { get; init; }
    public string? ServiceProvider { get; init; }
    public DateTimeOffset? NextMaintenanceDate { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset Created { get; init; }
}

