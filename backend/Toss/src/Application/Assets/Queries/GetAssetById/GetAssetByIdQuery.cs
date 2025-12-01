using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Assets;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Assets.Queries.GetAssetById;

public record GetAssetByIdQuery(int Id) : IRequest<AssetDetailDto>;

public class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, AssetDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetAssetByIdQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<AssetDetailDto> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var asset = await _context.Assets
            .Include(a => a.Shop)
            .Include(a => a.MaintenanceLogs.OrderByDescending(m => m.MaintenanceDate))
            .FirstOrDefaultAsync(a => a.Id == request.Id
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (asset == null)
        {
            throw new NotFoundException("Asset", request.Id.ToString());
        }

        return new AssetDetailDto
        {
            Id = asset.Id,
            Name = asset.Name,
            Code = asset.Code,
            Value = asset.Value,
            PurchaseCost = asset.PurchaseCost,
            PurchaseDate = asset.PurchaseDate,
            Location = asset.Location,
            ShopId = asset.ShopId,
            ShopName = asset.Shop != null ? asset.Shop.Name : null,
            Condition = asset.Condition,
            Category = asset.Category,
            Brand = asset.Brand,
            Model = asset.Model,
            SerialNumber = asset.SerialNumber,
            Notes = asset.Notes,
            IsActive = asset.IsActive,
            Created = asset.Created,
            MaintenanceLogs = asset.MaintenanceLogs.Select(m => new MaintenanceLogDto
            {
                Id = m.Id,
                MaintenanceDate = m.MaintenanceDate,
                MaintenanceType = m.MaintenanceType,
                Description = m.Description,
                Cost = m.Cost,
                ServiceProvider = m.ServiceProvider,
                NextMaintenanceDate = m.NextMaintenanceDate,
                Notes = m.Notes,
                Created = m.Created
            }).ToList()
        };
    }
}

public record AssetDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public decimal Value { get; init; }
    public decimal? PurchaseCost { get; init; }
    public DateTimeOffset? PurchaseDate { get; init; }
    public string Location { get; init; } = string.Empty;
    public int? ShopId { get; init; }
    public string? ShopName { get; init; }
    public Domain.Enums.AssetCondition Condition { get; init; }
    public string? Category { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset Created { get; init; }
    public List<MaintenanceLogDto> MaintenanceLogs { get; init; } = new();
}

public record MaintenanceLogDto
{
    public int Id { get; init; }
    public DateTimeOffset MaintenanceDate { get; init; }
    public string MaintenanceType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal? Cost { get; init; }
    public string? ServiceProvider { get; init; }
    public DateTimeOffset? NextMaintenanceDate { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset Created { get; init; }
}

