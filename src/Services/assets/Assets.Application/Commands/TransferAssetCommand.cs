using FluentValidation;

namespace TossErp.Assets.Application.Commands;

/// <summary>
/// Command to transfer an asset to a new location or user
/// </summary>
public record TransferAssetCommand(
    Guid AssetId,
    Guid? NewLocationId,
    Guid? NewDepartmentId,
    Guid? NewAssignedUserId,
    string Reason,
    DateOnly TransferDate,
    string? Notes
) : IRequest<AssetTransferDto>;

/// <summary>
/// Handler for transferring assets
/// </summary>
public class TransferAssetCommandHandler : IRequestHandler<TransferAssetCommand, AssetTransferDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<TransferAssetCommandHandler> _logger;

    public TransferAssetCommandHandler(
        IAssetRepository assetRepository,
        ILocationRepository locationRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<TransferAssetCommandHandler> logger)
    {
        _assetRepository = assetRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<AssetTransferDto> Handle(TransferAssetCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transferring asset {AssetId}", request.AssetId);

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset == null)
        {
            throw new InvalidOperationException($"Asset with ID {request.AssetId} not found");
        }

        // Validate location if provided
        if (request.NewLocationId.HasValue)
        {
            var location = await _locationRepository.GetByIdAsync(request.NewLocationId.Value, cancellationToken);
            if (location == null)
            {
                throw new InvalidOperationException($"Location with ID {request.NewLocationId} not found");
            }
        }

        var currentUserId = _currentUserService.UserId ?? "system";
        var oldLocationId = asset.LocationId;
        var oldDepartmentId = asset.DepartmentId;
        var oldAssignedUserId = asset.AssignedToUserId;

        // Create transfer record
        var transfer = new AssetTransfer(
            assetId: asset.Id,
            fromLocationId: oldLocationId,
            toLocationId: request.NewLocationId,
            fromDepartmentId: oldDepartmentId,
            toDepartmentId: request.NewDepartmentId,
            fromUserId: oldAssignedUserId,
            toUserId: request.NewAssignedUserId,
            transferDate: request.TransferDate,
            reason: request.Reason,
            notes: request.Notes,
            transferredBy: currentUserId);

        // Update asset location
        if (request.NewLocationId.HasValue && request.NewLocationId != asset.LocationId)
        {
            asset.UpdateLocation(request.NewLocationId.Value, currentUserId);
        }

        // Update asset department
        if (request.NewDepartmentId.HasValue && request.NewDepartmentId != asset.DepartmentId)
        {
            asset.UpdateDepartment(request.NewDepartmentId.Value, currentUserId);
        }

        // Update asset assignment
        if (request.NewAssignedUserId.HasValue && request.NewAssignedUserId != asset.AssignedToUserId)
        {
            asset.AssignToUser(request.NewAssignedUserId.Value, currentUserId);
        }
        else if (!request.NewAssignedUserId.HasValue && asset.AssignedToUserId.HasValue)
        {
            asset.UnassignFromUser(currentUserId);
        }

        // Record the transfer
        asset.RecordTransfer(transfer, currentUserId);

        // Save changes
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notifications
        if (oldAssignedUserId.HasValue && request.NewAssignedUserId != oldAssignedUserId)
        {
            await _notificationService.SendAssetTransferNotificationAsync(
                asset.Id, asset.Name, oldAssignedUserId.Value, request.NewAssignedUserId, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(asset.DomainEvents, cancellationToken);
        asset.ClearDomainEvents();

        _logger.LogInformation("Successfully transferred asset {AssetId}", request.AssetId);

        return MapToDto(transfer, asset);
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
/// Validator for TransferAssetCommand
/// </summary>
public class TransferAssetCommandValidator : AbstractValidator<TransferAssetCommand>
{
    public TransferAssetCommandValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Transfer reason is required")
            .MaximumLength(500)
            .WithMessage("Transfer reason cannot exceed 500 characters");

        RuleFor(x => x.TransferDate)
            .NotEmpty()
            .WithMessage("Transfer date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Transfer date cannot be in the future");

        RuleFor(x => x)
            .Must(x => x.NewLocationId.HasValue || x.NewDepartmentId.HasValue || x.NewAssignedUserId.HasValue)
            .WithMessage("At least one transfer target (location, department, or user) must be specified");
    }
}

/// <summary>
/// Command to transfer multiple assets in bulk
/// </summary>
public record BulkTransferAssetsCommand(
    List<Guid> AssetIds,
    Guid? NewLocationId,
    Guid? NewDepartmentId,
    Guid? NewAssignedUserId,
    string Reason,
    DateOnly TransferDate,
    string? Notes
) : IRequest<List<AssetTransferDto>>;

/// <summary>
/// Handler for bulk asset transfers
/// </summary>
public class BulkTransferAssetsCommandHandler : IRequestHandler<BulkTransferAssetsCommand, List<AssetTransferDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<BulkTransferAssetsCommandHandler> _logger;

    public BulkTransferAssetsCommandHandler(
        IAssetRepository assetRepository,
        ILocationRepository locationRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<BulkTransferAssetsCommandHandler> logger)
    {
        _assetRepository = assetRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<List<AssetTransferDto>> Handle(BulkTransferAssetsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bulk transferring {AssetCount} assets", request.AssetIds.Count);

        // Validate location if provided
        if (request.NewLocationId.HasValue)
        {
            var location = await _locationRepository.GetByIdAsync(request.NewLocationId.Value, cancellationToken);
            if (location == null)
            {
                throw new InvalidOperationException($"Location with ID {request.NewLocationId} not found");
            }
        }

        var assets = await _assetRepository.GetByIdsAsync(request.AssetIds, cancellationToken);
        if (assets.Count != request.AssetIds.Count)
        {
            var missingIds = request.AssetIds.Except(assets.Select(a => a.Id)).ToList();
            throw new InvalidOperationException($"Assets not found: {string.Join(", ", missingIds)}");
        }

        var currentUserId = _currentUserService.UserId ?? "system";
        var transfers = new List<AssetTransferDto>();
        var affectedUserIds = new HashSet<Guid>();

        foreach (var asset in assets)
        {
            var oldLocationId = asset.LocationId;
            var oldDepartmentId = asset.DepartmentId;
            var oldAssignedUserId = asset.AssignedToUserId;

            // Create transfer record
            var transfer = new AssetTransfer(
                assetId: asset.Id,
                fromLocationId: oldLocationId,
                toLocationId: request.NewLocationId,
                fromDepartmentId: oldDepartmentId,
                toDepartmentId: request.NewDepartmentId,
                fromUserId: oldAssignedUserId,
                toUserId: request.NewAssignedUserId,
                transferDate: request.TransferDate,
                reason: request.Reason,
                notes: request.Notes,
                transferredBy: currentUserId);

            // Update asset
            if (request.NewLocationId.HasValue && request.NewLocationId != asset.LocationId)
            {
                asset.UpdateLocation(request.NewLocationId.Value, currentUserId);
            }

            if (request.NewDepartmentId.HasValue && request.NewDepartmentId != asset.DepartmentId)
            {
                asset.UpdateDepartment(request.NewDepartmentId.Value, currentUserId);
            }

            if (request.NewAssignedUserId.HasValue && request.NewAssignedUserId != asset.AssignedToUserId)
            {
                asset.AssignToUser(request.NewAssignedUserId.Value, currentUserId);
                affectedUserIds.Add(request.NewAssignedUserId.Value);
            }
            else if (!request.NewAssignedUserId.HasValue && asset.AssignedToUserId.HasValue)
            {
                asset.UnassignFromUser(currentUserId);
            }

            if (oldAssignedUserId.HasValue)
            {
                affectedUserIds.Add(oldAssignedUserId.Value);
            }

            // Record the transfer
            asset.RecordTransfer(transfer, currentUserId);

            transfers.Add(MapToDto(transfer, asset));
        }

        // Save all changes
        await _assetRepository.UpdateRangeAsync(assets, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send bulk transfer notification
        await _notificationService.SendBulkAssetTransferNotificationAsync(
            assets.Select(a => new { a.Id, a.Name }).ToList(),
            affectedUserIds.ToList(),
            cancellationToken);

        // Publish domain events for all assets
        var allDomainEvents = assets.SelectMany(a => a.DomainEvents).ToList();
        await _domainEventService.PublishAsync(allDomainEvents, cancellationToken);
        
        foreach (var asset in assets)
        {
            asset.ClearDomainEvents();
        }

        _logger.LogInformation("Successfully bulk transferred {AssetCount} assets", request.AssetIds.Count);

        return transfers;
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
/// Validator for BulkTransferAssetsCommand
/// </summary>
public class BulkTransferAssetsCommandValidator : AbstractValidator<BulkTransferAssetsCommand>
{
    public BulkTransferAssetsCommandValidator()
    {
        RuleFor(x => x.AssetIds)
            .NotEmpty()
            .WithMessage("At least one asset ID is required")
            .Must(x => x.Count <= 100)
            .WithMessage("Cannot transfer more than 100 assets at once");

        RuleForEach(x => x.AssetIds)
            .NotEmpty()
            .WithMessage("Asset ID cannot be empty");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Transfer reason is required")
            .MaximumLength(500)
            .WithMessage("Transfer reason cannot exceed 500 characters");

        RuleFor(x => x.TransferDate)
            .NotEmpty()
            .WithMessage("Transfer date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Transfer date cannot be in the future");

        RuleFor(x => x)
            .Must(x => x.NewLocationId.HasValue || x.NewDepartmentId.HasValue || x.NewAssignedUserId.HasValue)
            .WithMessage("At least one transfer target (location, department, or user) must be specified");
    }
}
