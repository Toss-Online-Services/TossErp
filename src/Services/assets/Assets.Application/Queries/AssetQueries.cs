using FluentValidation;

namespace TossErp.Assets.Application.Queries;

/// <summary>
/// Query to get assets with filtering and pagination
/// </summary>
public record GetAssetsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    AssetStatus? Status = null,
    AssetCategory? Category = null,
    string? Type = null,
    Guid? LocationId = null,
    Guid? DepartmentId = null,
    Guid? AssignedToUserId = null,
    DateOnly? PurchaseDateFrom = null,
    DateOnly? PurchaseDateTo = null,
    decimal? PurchasePriceFrom = null,
    decimal? PurchasePriceTo = null,
    string? SortBy = "Name",
    bool SortDescending = false
) : IRequest<PagedResult<AssetSummaryDto>>;

/// <summary>
/// Handler for getting assets with filtering and pagination
/// </summary>
public class GetAssetsQueryHandler : IRequestHandler<GetAssetsQuery, PagedResult<AssetSummaryDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetAssetsQueryHandler> _logger;

    public GetAssetsQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetAssetsQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<PagedResult<AssetSummaryDto>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting assets with filters - Page: {PageNumber}, Size: {PageSize}", 
            request.PageNumber, request.PageSize);

        var (assets, totalCount) = await _assetRepository.GetFilteredAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            searchTerm: request.SearchTerm,
            status: request.Status,
            category: request.Category,
            type: request.Type,
            locationId: request.LocationId,
            departmentId: request.DepartmentId,
            assignedToUserId: request.AssignedToUserId,
            purchaseDateFrom: request.PurchaseDateFrom,
            purchaseDateTo: request.PurchaseDateTo,
            purchasePriceFrom: request.PurchasePriceFrom,
            purchasePriceTo: request.PurchasePriceTo,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            cancellationToken: cancellationToken);

        var assetDtos = assets.Select(MapToSummaryDto).ToList();

        return new PagedResult<AssetSummaryDto>
        {
            Items = assetDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    private static AssetSummaryDto MapToSummaryDto(Asset asset)
    {
        return new AssetSummaryDto
        {
            Id = asset.Id,
            AssetNumber = asset.AssetNumber.Value,
            Name = asset.Name,
            Category = asset.Category,
            Type = asset.Type,
            Status = asset.Status,
            SerialNumber = asset.SerialNumber,
            Manufacturer = asset.Manufacturer,
            Model = asset.Model,
            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice?.Amount,
            PurchaseCurrency = asset.PurchasePrice?.Currency.ToString(),
            CurrentValue = asset.CurrentValue?.Amount,
            LocationId = asset.LocationId,
            DepartmentId = asset.DepartmentId,
            AssignedToUserId = asset.AssignedToUserId,
            LastMaintenanceDate = asset.LastMaintenanceDate,
            NextMaintenanceDate = asset.NextMaintenanceDate,
            CreatedAt = asset.CreatedAt,
            LastModified = asset.LastModified
        };
    }
}

/// <summary>
/// Validator for GetAssetsQuery
/// </summary>
public class GetAssetsQueryValidator : AbstractValidator<GetAssetsQuery>
{
    public GetAssetsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.PurchaseDateFrom)
            .LessThanOrEqualTo(x => x.PurchaseDateTo)
            .When(x => x.PurchaseDateFrom.HasValue && x.PurchaseDateTo.HasValue)
            .WithMessage("Purchase date from must be before or equal to purchase date to");

        RuleFor(x => x.PurchasePriceFrom)
            .LessThanOrEqualTo(x => x.PurchasePriceTo)
            .When(x => x.PurchasePriceFrom.HasValue && x.PurchasePriceTo.HasValue)
            .WithMessage("Purchase price from must be less than or equal to purchase price to");
    }
}

/// <summary>
/// Query to get a single asset by ID
/// </summary>
public record GetAssetByIdQuery(Guid AssetId) : IRequest<AssetDto?>;

/// <summary>
/// Handler for getting a single asset by ID
/// </summary>
public class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, AssetDto?>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetAssetByIdQueryHandler> _logger;

    public GetAssetByIdQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetAssetByIdQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<AssetDto?> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting asset by ID: {AssetId}", request.AssetId);

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        
        return asset == null ? null : MapToDto(asset);
    }

    private static AssetDto MapToDto(Asset asset)
    {
        return new AssetDto
        {
            Id = asset.Id,
            TenantId = asset.TenantId,
            AssetNumber = asset.AssetNumber.Value,
            Name = asset.Name,
            Description = asset.Description,
            Category = asset.Category,
            Type = asset.Type,
            Status = asset.Status,
            SerialNumber = asset.SerialNumber,
            Model = asset.Model,
            Manufacturer = asset.Manufacturer,
            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice?.Amount,
            PurchaseCurrency = asset.PurchasePrice?.Currency.ToString(),
            CurrentValue = asset.CurrentValue?.Amount,
            WarrantyStartDate = asset.WarrantyPeriod?.StartDate,
            WarrantyEndDate = asset.WarrantyPeriod?.EndDate,
            WarrantyPeriodMonths = asset.WarrantyPeriod?.DurationInMonths,
            WarrantyProvider = asset.WarrantyProvider,
            LocationId = asset.LocationId,
            DepartmentId = asset.DepartmentId,
            AssignedToUserId = asset.AssignedToUserId,
            LastMaintenanceDate = asset.LastMaintenanceDate,
            NextMaintenanceDate = asset.NextMaintenanceDate,
            MaintenanceIntervalDays = asset.MaintenanceSchedule.IntervalDays,
            DepreciationMethod = asset.DepreciationInfo?.Method,
            UsefulLifeYears = asset.DepreciationInfo?.UsefulLifeYears,
            SalvageValue = asset.DepreciationInfo?.SalvageValue?.Amount,
            AccumulatedDepreciation = asset.AccumulatedDepreciation?.Amount,
            CreatedAt = asset.CreatedAt,
            CreatedBy = asset.CreatedBy,
            LastModified = asset.LastModified,
            LastModifiedBy = asset.LastModifiedBy
        };
    }
}

/// <summary>
/// Validator for GetAssetByIdQuery
/// </summary>
public class GetAssetByIdQueryValidator : AbstractValidator<GetAssetByIdQuery>
{
    public GetAssetByIdQueryValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required");
    }
}

/// <summary>
/// Query to get assets by status
/// </summary>
public record GetAssetsByStatusQuery(
    AssetStatus Status,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = "Name",
    bool SortDescending = false
) : IRequest<PagedResult<AssetSummaryDto>>;

/// <summary>
/// Handler for getting assets by status
/// </summary>
public class GetAssetsByStatusQueryHandler : IRequestHandler<GetAssetsByStatusQuery, PagedResult<AssetSummaryDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetAssetsByStatusQueryHandler> _logger;

    public GetAssetsByStatusQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetAssetsByStatusQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<PagedResult<AssetSummaryDto>> Handle(GetAssetsByStatusQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting assets by status: {Status}", request.Status);

        var (assets, totalCount) = await _assetRepository.GetByStatusAsync(
            status: request.Status,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            cancellationToken: cancellationToken);

        var assetDtos = assets.Select(MapToSummaryDto).ToList();

        return new PagedResult<AssetSummaryDto>
        {
            Items = assetDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    private static AssetSummaryDto MapToSummaryDto(Asset asset)
    {
        return new AssetSummaryDto
        {
            Id = asset.Id,
            AssetNumber = asset.AssetNumber.Value,
            Name = asset.Name,
            Category = asset.Category,
            Type = asset.Type,
            Status = asset.Status,
            SerialNumber = asset.SerialNumber,
            Manufacturer = asset.Manufacturer,
            Model = asset.Model,
            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice?.Amount,
            PurchaseCurrency = asset.PurchasePrice?.Currency.ToString(),
            CurrentValue = asset.CurrentValue?.Amount,
            LocationId = asset.LocationId,
            DepartmentId = asset.DepartmentId,
            AssignedToUserId = asset.AssignedToUserId,
            LastMaintenanceDate = asset.LastMaintenanceDate,
            NextMaintenanceDate = asset.NextMaintenanceDate,
            CreatedAt = asset.CreatedAt,
            LastModified = asset.LastModified
        };
    }
}

/// <summary>
/// Validator for GetAssetsByStatusQuery
/// </summary>
public class GetAssetsByStatusQueryValidator : AbstractValidator<GetAssetsByStatusQuery>
{
    public GetAssetsByStatusQueryValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Valid asset status is required");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}
