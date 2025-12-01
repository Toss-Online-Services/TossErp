using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Assets;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Assets.Commands.AddMaintenanceLog;

public record AddMaintenanceLogCommand : IRequest<int>
{
    public int AssetId { get; init; }
    public DateTimeOffset MaintenanceDate { get; init; }
    public string MaintenanceType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal? Cost { get; init; }
    public string? ServiceProvider { get; init; }
    public DateTimeOffset? NextMaintenanceDate { get; init; }
    public string? Notes { get; init; }
}

public class AddMaintenanceLogCommandHandler : IRequestHandler<AddMaintenanceLogCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public AddMaintenanceLogCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(AddMaintenanceLogCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.MaintenanceType))
        {
            throw new ValidationException("Maintenance type is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ValidationException("Maintenance description is required.");
        }

        // Validate asset exists and belongs to business
        var asset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == request.AssetId
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (asset == null)
        {
            throw new NotFoundException("Asset", request.AssetId.ToString());
        }

        var maintenanceLog = new AssetMaintenanceLog
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            AssetId = request.AssetId,
            MaintenanceDate = request.MaintenanceDate,
            MaintenanceType = request.MaintenanceType,
            Description = request.Description,
            Cost = request.Cost,
            ServiceProvider = request.ServiceProvider,
            NextMaintenanceDate = request.NextMaintenanceDate,
            Notes = request.Notes
        };

        _context.AssetMaintenanceLogs.Add(maintenanceLog);
        await _context.SaveChangesAsync(cancellationToken);

        return maintenanceLog.Id;
    }
}

