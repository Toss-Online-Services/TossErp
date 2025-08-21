using FluentValidation;

namespace TossErp.Assets.Application.Commands;

/// <summary>
/// Command to create a new asset
/// </summary>
public record CreateAssetCommand(
    string TenantId,
    string AssetNumber,
    string Name,
    string? Description,
    AssetCategory Category,
    AssetType Type,
    string? SerialNumber,
    string? Model,
    string? Manufacturer,
    DateTime? PurchaseDate,
    decimal? PurchasePrice,
    string? PurchaseCurrency,
    DateTime? WarrantyStartDate,
    DateTime? WarrantyEndDate,
    int? WarrantyPeriodMonths,
    string? WarrantyProvider,
    Guid? LocationId,
    Guid? DepartmentId,
    Guid? AssignedToUserId,
    int MaintenanceIntervalDays,
    DepreciationMethod? DepreciationMethod,
    int? UsefulLifeYears,
    decimal? SalvageValue
) : IRequest<AssetDto>;

/// <summary>
/// Handler for creating assets
/// </summary>
public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, AssetDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateAssetCommandHandler> _logger;

    public CreateAssetCommandHandler(
        IAssetRepository assetRepository,
        ILocationRepository locationRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<CreateAssetCommandHandler> logger)
    {
        _assetRepository = assetRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<AssetDto> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating asset {AssetNumber} for tenant {TenantId}", request.AssetNumber, request.TenantId);

        // Check if asset number already exists
        var existingAsset = await _assetRepository.GetByAssetNumberAsync(request.AssetNumber, cancellationToken);
        if (existingAsset != null)
        {
            throw new InvalidOperationException($"Asset with number {request.AssetNumber} already exists");
        }

        // Validate location if provided
        if (request.LocationId.HasValue)
        {
            var location = await _locationRepository.GetByIdAsync(request.LocationId.Value, cancellationToken);
            if (location == null)
            {
                throw new InvalidOperationException($"Location with ID {request.LocationId} not found");
            }
        }

        // Create asset value objects
        var assetNumber = new AssetNumber(request.AssetNumber);
        
        Money? purchasePrice = null;
        if (request.PurchasePrice.HasValue && !string.IsNullOrEmpty(request.PurchaseCurrency))
        {
            if (Enum.TryParse<CurrencyCode>(request.PurchaseCurrency, out var currency))
            {
                purchasePrice = new Money(request.PurchasePrice.Value, currency);
            }
        }

        DateRange? warrantyPeriod = null;
        if (request.WarrantyStartDate.HasValue && request.WarrantyEndDate.HasValue)
        {
            warrantyPeriod = new DateRange(request.WarrantyStartDate.Value, request.WarrantyEndDate.Value);
        }

        var maintenanceSchedule = new MaintenanceSchedule(request.MaintenanceIntervalDays);

        DepreciationInfo? depreciation = null;
        if (request.DepreciationMethod.HasValue && request.UsefulLifeYears.HasValue)
        {
            var salvageValue = request.SalvageValue.HasValue && !string.IsNullOrEmpty(request.PurchaseCurrency)
                ? new Money(request.SalvageValue.Value, Enum.Parse<CurrencyCode>(request.PurchaseCurrency))
                : null;
            
            depreciation = new DepreciationInfo(
                request.DepreciationMethod.Value,
                request.UsefulLifeYears.Value,
                salvageValue);
        }

        // Create asset aggregate
        var asset = new Asset(
            Guid.NewGuid(),
            request.TenantId,
            assetNumber,
            request.Name,
            request.Category,
            request.Type,
            _currentUserService.UserId ?? "system",
            request.Description,
            request.SerialNumber,
            request.Model,
            request.Manufacturer,
            request.PurchaseDate,
            purchasePrice,
            warrantyPeriod,
            request.WarrantyProvider,
            request.LocationId,
            request.DepartmentId,
            maintenanceSchedule,
            depreciation);

        // Assign to user if provided
        if (request.AssignedToUserId.HasValue)
        {
            asset.AssignToUser(request.AssignedToUserId.Value, _currentUserService.UserId ?? "system");
        }

        // Save asset
        await _assetRepository.AddAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(asset.DomainEvents, cancellationToken);
        asset.ClearDomainEvents();

        _logger.LogInformation("Successfully created asset {AssetId} with number {AssetNumber}", asset.Id, request.AssetNumber);

        return MapToDto(asset);
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
/// Validator for CreateAssetCommand
/// </summary>
public class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required");

        RuleFor(x => x.AssetNumber)
            .NotEmpty()
            .WithMessage("Asset number is required")
            .MaximumLength(50)
            .WithMessage("Asset number cannot exceed 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Asset name is required")
            .MaximumLength(200)
            .WithMessage("Asset name cannot exceed 200 characters");

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Valid asset category is required");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Valid asset type is required");

        RuleFor(x => x.PurchasePrice)
            .GreaterThan(0)
            .When(x => x.PurchasePrice.HasValue)
            .WithMessage("Purchase price must be greater than 0");

        RuleFor(x => x.PurchaseCurrency)
            .NotEmpty()
            .When(x => x.PurchasePrice.HasValue)
            .WithMessage("Purchase currency is required when purchase price is provided");

        RuleFor(x => x.MaintenanceIntervalDays)
            .GreaterThan(0)
            .WithMessage("Maintenance interval must be greater than 0 days");

        RuleFor(x => x.UsefulLifeYears)
            .GreaterThan(0)
            .When(x => x.DepreciationMethod.HasValue)
            .WithMessage("Useful life years must be greater than 0 when depreciation method is specified");

        RuleFor(x => x.WarrantyEndDate)
            .GreaterThan(x => x.WarrantyStartDate)
            .When(x => x.WarrantyStartDate.HasValue && x.WarrantyEndDate.HasValue)
            .WithMessage("Warranty end date must be after start date");
    }
}
