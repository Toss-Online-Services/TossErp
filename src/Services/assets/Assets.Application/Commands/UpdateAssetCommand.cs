using FluentValidation;

namespace TossErp.Assets.Application.Commands;

/// <summary>
/// Command to update an existing asset
/// </summary>
public record UpdateAssetCommand(
    Guid AssetId,
    string Name,
    string? Description,
    string? SerialNumber,
    string? Model,
    string? Manufacturer,
    Guid? LocationId,
    Guid? DepartmentId,
    Guid? AssignedToUserId,
    int MaintenanceIntervalDays
) : IRequest<AssetDto>;

/// <summary>
/// Handler for updating assets
/// </summary>
public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, AssetDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateAssetCommandHandler> _logger;

    public UpdateAssetCommandHandler(
        IAssetRepository assetRepository,
        ILocationRepository locationRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<UpdateAssetCommandHandler> logger)
    {
        _assetRepository = assetRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<AssetDto> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating asset {AssetId}", request.AssetId);

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset == null)
        {
            throw new InvalidOperationException($"Asset with ID {request.AssetId} not found");
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

        var currentUserId = _currentUserService.UserId ?? "system";

        // Update basic information
        asset.UpdateBasicInfo(
            request.Name,
            request.Description,
            request.SerialNumber,
            request.Model,
            request.Manufacturer,
            currentUserId);

        // Update location if changed
        if (asset.LocationId != request.LocationId)
        {
            if (request.LocationId.HasValue)
            {
                asset.UpdateLocation(request.LocationId.Value, currentUserId);
            }
        }

        // Update department if changed
        if (asset.DepartmentId != request.DepartmentId)
        {
            if (request.DepartmentId.HasValue)
            {
                asset.UpdateDepartment(request.DepartmentId.Value, currentUserId);
            }
        }

        // Update assignment if changed
        if (asset.AssignedToUserId != request.AssignedToUserId)
        {
            if (request.AssignedToUserId.HasValue)
            {
                asset.AssignToUser(request.AssignedToUserId.Value, currentUserId);
            }
            else
            {
                asset.UnassignFromUser(currentUserId);
            }
        }

        // Update maintenance schedule if changed
        if (asset.MaintenanceSchedule.IntervalDays != request.MaintenanceIntervalDays)
        {
            var newSchedule = new MaintenanceSchedule(request.MaintenanceIntervalDays);
            asset.UpdateMaintenanceSchedule(newSchedule, currentUserId);
        }

        // Save changes
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(asset.DomainEvents, cancellationToken);
        asset.ClearDomainEvents();

        _logger.LogInformation("Successfully updated asset {AssetId}", request.AssetId);

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
/// Validator for UpdateAssetCommand
/// </summary>
public class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Asset name is required")
            .MaximumLength(200)
            .WithMessage("Asset name cannot exceed 200 characters");

        RuleFor(x => x.MaintenanceIntervalDays)
            .GreaterThan(0)
            .WithMessage("Maintenance interval must be greater than 0 days");
    }
}

/// <summary>
/// Command to change asset status
/// </summary>
public record ChangeAssetStatusCommand(
    Guid AssetId,
    AssetStatus NewStatus,
    string? Notes
) : IRequest<AssetDto>;

/// <summary>
/// Handler for changing asset status
/// </summary>
public class ChangeAssetStatusCommandHandler : IRequestHandler<ChangeAssetStatusCommand, AssetDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ChangeAssetStatusCommandHandler> _logger;

    public ChangeAssetStatusCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<ChangeAssetStatusCommandHandler> logger)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<AssetDto> Handle(ChangeAssetStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing status of asset {AssetId} to {NewStatus}", request.AssetId, request.NewStatus);

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset == null)
        {
            throw new InvalidOperationException($"Asset with ID {request.AssetId} not found");
        }

        var oldStatus = asset.Status;
        var currentUserId = _currentUserService.UserId ?? "system";

        // Change status based on the target status
        switch (request.NewStatus)
        {
            case AssetStatus.Active:
                asset.Activate(currentUserId);
                break;
            case AssetStatus.Inactive:
                asset.Deactivate(currentUserId, request.Notes ?? "Status changed to inactive");
                break;
            case AssetStatus.InMaintenance:
                asset.StartMaintenance(currentUserId, request.Notes ?? "Asset placed in maintenance");
                break;
            case AssetStatus.OutOfService:
                asset.MarkOutOfService(currentUserId, request.Notes ?? "Asset marked out of service");
                break;
            case AssetStatus.Disposed:
                asset.MarkDisposed(currentUserId, request.Notes ?? "Asset disposed");
                break;
            default:
                throw new InvalidOperationException($"Cannot change status to {request.NewStatus}");
        }

        // Save changes
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification about status change
        await _notificationService.SendAssetStatusChangeNotificationAsync(
            asset.Id, asset.Name, oldStatus, request.NewStatus, cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(asset.DomainEvents, cancellationToken);
        asset.ClearDomainEvents();

        _logger.LogInformation("Successfully changed asset {AssetId} status from {OldStatus} to {NewStatus}", 
            request.AssetId, oldStatus, request.NewStatus);

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
/// Validator for ChangeAssetStatusCommand
/// </summary>
public class ChangeAssetStatusCommandValidator : AbstractValidator<ChangeAssetStatusCommand>
{
    public ChangeAssetStatusCommandValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Valid asset status is required");
    }
}
