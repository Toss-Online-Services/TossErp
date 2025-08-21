using FluentValidation;

namespace TossErp.Assets.Application.Queries;

/// <summary>
/// Query to get asset depreciation report
/// </summary>
public record GetAssetDepreciationReportQuery(
    DateOnly? AsOfDate = null,
    AssetCategory? Category = null,
    Guid? LocationId = null,
    Guid? DepartmentId = null
) : IRequest<List<AssetDepreciationDto>>;

/// <summary>
/// Handler for getting asset depreciation report
/// </summary>
public class GetAssetDepreciationReportQueryHandler : IRequestHandler<GetAssetDepreciationReportQuery, List<AssetDepreciationDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetAssetDepreciationReportQueryHandler> _logger;

    public GetAssetDepreciationReportQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetAssetDepreciationReportQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<List<AssetDepreciationDto>> Handle(GetAssetDepreciationReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating asset depreciation report");

        var asOfDate = request.AsOfDate ?? DateOnly.FromDateTime(DateTime.Today);

        var assets = await _assetRepository.GetAssetsForDepreciationReportAsync(
            asOfDate: asOfDate,
            category: request.Category,
            locationId: request.LocationId,
            departmentId: request.DepartmentId,
            cancellationToken: cancellationToken);

        var depreciationData = assets.Select(asset => MapToDepreciationDto(asset, asOfDate)).ToList();

        return depreciationData;
    }

    private static AssetDepreciationDto MapToDepreciationDto(Asset asset, DateOnly asOfDate)
    {
        // Calculate depreciation based on asset's depreciation info
        var yearsOwned = CalculateYearsOwned(asset.PurchaseDate, asOfDate);
        var currentDepreciation = CalculateDepreciation(asset, yearsOwned);

        return new AssetDepreciationDto
        {
            AssetId = asset.Id,
            AssetNumber = asset.AssetNumber.Value,
            AssetName = asset.Name,
            Category = asset.Category,
            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice?.Amount ?? 0,
            Currency = asset.PurchasePrice?.Currency.ToString() ?? "USD",
            DepreciationMethod = asset.DepreciationInfo?.Method ?? "StraightLine",
            UsefulLifeYears = asset.DepreciationInfo?.UsefulLifeYears ?? 5,
            SalvageValue = asset.DepreciationInfo?.SalvageValue?.Amount ?? 0,
            YearsOwned = yearsOwned,
            AccumulatedDepreciation = currentDepreciation,
            BookValue = (asset.PurchasePrice?.Amount ?? 0) - currentDepreciation,
            DepreciationPerYear = CalculateAnnualDepreciation(asset),
            AsOfDate = asOfDate
        };
    }

    private static decimal CalculateYearsOwned(DateOnly? purchaseDate, DateOnly asOfDate)
    {
        if (!purchaseDate.HasValue) return 0;
        
        var daysDifference = asOfDate.DayNumber - purchaseDate.Value.DayNumber;
        return (decimal)daysDifference / 365.25m; // Account for leap years
    }

    private static decimal CalculateDepreciation(Asset asset, decimal yearsOwned)
    {
        if (asset.DepreciationInfo == null || asset.PurchasePrice == null)
            return 0;

        var depreciableAmount = asset.PurchasePrice.Amount - (asset.DepreciationInfo.SalvageValue?.Amount ?? 0);
        var usefulLife = asset.DepreciationInfo.UsefulLifeYears;

        return asset.DepreciationInfo.Method switch
        {
            "StraightLine" => Math.Min(depreciableAmount, (depreciableAmount / usefulLife) * yearsOwned),
            "DoubleDeclining" => CalculateDoubleDecliningDepreciation(depreciableAmount, usefulLife, yearsOwned),
            _ => Math.Min(depreciableAmount, (depreciableAmount / usefulLife) * yearsOwned)
        };
    }

    private static decimal CalculateDoubleDecliningDepreciation(decimal depreciableAmount, int usefulLife, decimal yearsOwned)
    {
        var rate = 2m / usefulLife;
        var bookValue = depreciableAmount;
        var totalDepreciation = 0m;

        for (int year = 1; year <= Math.Ceiling(yearsOwned) && totalDepreciation < depreciableAmount; year++)
        {
            var yearlyDepreciation = bookValue * rate;
            if (totalDepreciation + yearlyDepreciation > depreciableAmount)
            {
                yearlyDepreciation = depreciableAmount - totalDepreciation;
            }

            totalDepreciation += yearlyDepreciation;
            bookValue -= yearlyDepreciation;
        }

        return totalDepreciation;
    }

    private static decimal CalculateAnnualDepreciation(Asset asset)
    {
        if (asset.DepreciationInfo == null || asset.PurchasePrice == null)
            return 0;

        var depreciableAmount = asset.PurchasePrice.Amount - (asset.DepreciationInfo.SalvageValue?.Amount ?? 0);
        return depreciableAmount / asset.DepreciationInfo.UsefulLifeYears;
    }
}

/// <summary>
/// DTO for asset depreciation data
/// </summary>
public class AssetDepreciationDto
{
    public Guid AssetId { get; set; }
    public string AssetNumber { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public AssetCategory Category { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string DepreciationMethod { get; set; } = string.Empty;
    public int UsefulLifeYears { get; set; }
    public decimal SalvageValue { get; set; }
    public decimal YearsOwned { get; set; }
    public decimal AccumulatedDepreciation { get; set; }
    public decimal BookValue { get; set; }
    public decimal DepreciationPerYear { get; set; }
    public DateOnly AsOfDate { get; set; }
}

/// <summary>
/// Query to get asset statistics
/// </summary>
public record GetAssetStatisticsQuery(
    Guid? LocationId = null,
    Guid? DepartmentId = null
) : IRequest<AssetStatisticsDto>;

/// <summary>
/// Handler for getting asset statistics
/// </summary>
public class GetAssetStatisticsQueryHandler : IRequestHandler<GetAssetStatisticsQuery, AssetStatisticsDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetAssetStatisticsQueryHandler> _logger;

    public GetAssetStatisticsQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetAssetStatisticsQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<AssetStatisticsDto> Handle(GetAssetStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting asset statistics");

        var statistics = await _assetRepository.GetAssetStatisticsAsync(
            locationId: request.LocationId,
            departmentId: request.DepartmentId,
            cancellationToken: cancellationToken);

        return statistics;
    }
}

/// <summary>
/// DTO for asset statistics
/// </summary>
public class AssetStatisticsDto
{
    public int TotalAssets { get; set; }
    public int ActiveAssets { get; set; }
    public int InactiveAssets { get; set; }
    public int InMaintenanceAssets { get; set; }
    public int DisposedAssets { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageValue { get; set; }
    public string Currency { get; set; } = "USD";
    public Dictionary<AssetCategory, int> AssetsByCategory { get; set; } = new();
    public Dictionary<string, int> AssetsByType { get; set; } = new();
    public Dictionary<AssetStatus, int> AssetsByStatus { get; set; } = new();
    public int AssetsRequiringMaintenance { get; set; }
    public int OverdueMaintenanceCount { get; set; }
    public decimal MaintenanceCostThisMonth { get; set; }
    public decimal MaintenanceCostThisYear { get; set; }
}

/// <summary>
/// Query to get asset transfer history
/// </summary>
public record GetAssetTransferHistoryQuery(
    Guid? AssetId = null,
    Guid? UserId = null,
    DateOnly? TransferDateFrom = null,
    DateOnly? TransferDateTo = null,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedResult<AssetTransferDto>>;

/// <summary>
/// Handler for getting asset transfer history
/// </summary>
public class GetAssetTransferHistoryQueryHandler : IRequestHandler<GetAssetTransferHistoryQuery, PagedResult<AssetTransferDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetAssetTransferHistoryQueryHandler> _logger;

    public GetAssetTransferHistoryQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetAssetTransferHistoryQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<PagedResult<AssetTransferDto>> Handle(GetAssetTransferHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting asset transfer history");

        var (transfers, totalCount) = await _assetRepository.GetAssetTransferHistoryAsync(
            assetId: request.AssetId,
            userId: request.UserId,
            transferDateFrom: request.TransferDateFrom,
            transferDateTo: request.TransferDateTo,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var transferDtos = transfers.Select(t => MapToDto(t.Transfer, t.Asset)).ToList();

        return new PagedResult<AssetTransferDto>
        {
            Items = transferDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    private static AssetTransferDto MapToDto(AssetTransfer transfer, Asset asset)
    {
        return new AssetTransferDto
        {
            Id = transfer.Id,
            AssetId = transfer.AssetId,
            AssetName = asset.Name,
            AssetNumber = asset.AssetNumber.Value,
            FromLocationId = transfer.FromLocationId,
            ToLocationId = transfer.ToLocationId,
            FromDepartmentId = transfer.FromDepartmentId,
            ToDepartmentId = transfer.ToDepartmentId,
            FromUserId = transfer.FromUserId,
            ToUserId = transfer.ToUserId,
            TransferDate = transfer.TransferDate,
            Reason = transfer.Reason,
            Notes = transfer.Notes,
            TransferredBy = transfer.TransferredBy,
            TransferredAt = transfer.TransferredAt
        };
    }
}

/// <summary>
/// Validator for GetAssetTransferHistoryQuery
/// </summary>
public class GetAssetTransferHistoryQueryValidator : AbstractValidator<GetAssetTransferHistoryQuery>
{
    public GetAssetTransferHistoryQueryValidator()
    {
        RuleFor(x => x.TransferDateFrom)
            .LessThanOrEqualTo(x => x.TransferDateTo)
            .When(x => x.TransferDateFrom.HasValue && x.TransferDateTo.HasValue)
            .WithMessage("Transfer date from must be before or equal to transfer date to");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

/// <summary>
/// Query to get locations
/// </summary>
public record GetLocationsQuery(
    string? SearchTerm = null,
    bool IncludeInactive = false
) : IRequest<List<LocationDto>>;

/// <summary>
/// Handler for getting locations
/// </summary>
public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, List<LocationDto>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<GetLocationsQueryHandler> _logger;

    public GetLocationsQueryHandler(
        ILocationRepository locationRepository,
        ILogger<GetLocationsQueryHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<List<LocationDto>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting locations");

        var locations = await _locationRepository.GetFilteredAsync(
            searchTerm: request.SearchTerm,
            includeInactive: request.IncludeInactive,
            cancellationToken: cancellationToken);

        return locations.Select(MapToDto).ToList();
    }

    private static LocationDto MapToDto(Location location)
    {
        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Description = location.Description,
            Address = location.Address,
            City = location.City,
            State = location.State,
            PostalCode = location.PostalCode,
            Country = location.Country,
            IsActive = location.IsActive,
            CreatedAt = location.CreatedAt,
            LastModified = location.LastModified
        };
    }
}
