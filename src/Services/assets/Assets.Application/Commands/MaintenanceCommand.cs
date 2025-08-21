using FluentValidation;

namespace TossErp.Assets.Application.Commands;

/// <summary>
/// Command to schedule maintenance for an asset
/// </summary>
public record ScheduleMaintenanceCommand(
    Guid AssetId,
    MaintenanceType Type,
    string Description,
    DateOnly ScheduledDate,
    DateOnly? DueDate,
    decimal? EstimatedCost,
    string? Currency,
    int? EstimatedDurationHours,
    string? AssignedTechnician,
    string? Instructions,
    int Priority = 1
) : IRequest<MaintenanceRecordDto>;

/// <summary>
/// Handler for scheduling asset maintenance
/// </summary>
public class ScheduleMaintenanceCommandHandler : IRequestHandler<ScheduleMaintenanceCommand, MaintenanceRecordDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ScheduleMaintenanceCommandHandler> _logger;

    public ScheduleMaintenanceCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<ScheduleMaintenanceCommandHandler> logger)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<MaintenanceRecordDto> Handle(ScheduleMaintenanceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduling maintenance for asset {AssetId}", request.AssetId);

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset == null)
        {
            throw new InvalidOperationException($"Asset with ID {request.AssetId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Create maintenance record
        var cost = request.EstimatedCost.HasValue && !string.IsNullOrEmpty(request.Currency)
            ? Money.Create(request.EstimatedCost.Value, request.Currency)
            : null;

        var maintenanceRecord = new MaintenanceRecord(
            assetId: asset.Id,
            type: request.Type,
            description: request.Description,
            scheduledDate: request.ScheduledDate,
            estimatedCost: cost,
            estimatedDurationHours: request.EstimatedDurationHours,
            assignedTechnician: request.AssignedTechnician,
            instructions: request.Instructions,
            priority: request.Priority,
            scheduledBy: currentUserId);

        // Set due date if provided, otherwise use scheduled date
        if (request.DueDate.HasValue)
        {
            maintenanceRecord.SetDueDate(request.DueDate.Value);
        }

        // Schedule maintenance on the asset
        asset.ScheduleMaintenance(maintenanceRecord, currentUserId);

        // Save changes
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to assigned technician
        if (!string.IsNullOrEmpty(request.AssignedTechnician))
        {
            await _notificationService.SendMaintenanceScheduledNotificationAsync(
                asset.Id, asset.Name, maintenanceRecord.Id, request.AssignedTechnician, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(asset.DomainEvents, cancellationToken);
        asset.ClearDomainEvents();

        _logger.LogInformation("Successfully scheduled maintenance for asset {AssetId}", request.AssetId);

        return MapToDto(maintenanceRecord, asset);
    }

    private static MaintenanceRecordDto MapToDto(MaintenanceRecord record, Asset asset)
    {
        return new MaintenanceRecordDto
        {
            Id = record.Id,
            AssetId = record.AssetId,
            AssetName = asset.Name,
            AssetNumber = asset.AssetNumber.Value,
            Type = record.Type,
            Description = record.Description,
            Status = record.Status,
            ScheduledDate = record.ScheduledDate,
            DueDate = record.DueDate,
            StartDate = record.StartDate,
            CompletionDate = record.CompletionDate,
            EstimatedCost = record.EstimatedCost?.Amount,
            ActualCost = record.ActualCost?.Amount,
            Currency = record.EstimatedCost?.Currency.ToString() ?? record.ActualCost?.Currency.ToString(),
            EstimatedDurationHours = record.EstimatedDurationHours,
            ActualDurationHours = record.ActualDurationHours,
            AssignedTechnician = record.AssignedTechnician,
            Instructions = record.Instructions,
            WorkPerformed = record.WorkPerformed,
            PartsUsed = record.PartsUsed,
            Priority = record.Priority,
            ScheduledBy = record.ScheduledBy,
            ScheduledAt = record.ScheduledAt,
            CompletedBy = record.CompletedBy
        };
    }
}

/// <summary>
/// Validator for ScheduleMaintenanceCommand
/// </summary>
public class ScheduleMaintenanceCommandValidator : AbstractValidator<ScheduleMaintenanceCommand>
{
    public ScheduleMaintenanceCommandValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Valid maintenance type is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Maintenance description is required")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty()
            .WithMessage("Scheduled date is required")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Scheduled date cannot be in the past");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.ScheduledDate)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be on or after the scheduled date");

        RuleFor(x => x.EstimatedCost)
            .GreaterThan(0)
            .When(x => x.EstimatedCost.HasValue)
            .WithMessage("Estimated cost must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .When(x => x.EstimatedCost.HasValue)
            .WithMessage("Currency is required when estimated cost is provided");

        RuleFor(x => x.EstimatedDurationHours)
            .GreaterThan(0)
            .When(x => x.EstimatedDurationHours.HasValue)
            .WithMessage("Estimated duration must be greater than 0 hours");

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 5)
            .WithMessage("Priority must be between 1 (highest) and 5 (lowest)");
    }
}

/// <summary>
/// Command to complete maintenance work
/// </summary>
public record CompleteMaintenanceCommand(
    Guid MaintenanceRecordId,
    DateOnly CompletionDate,
    string WorkPerformed,
    decimal? ActualCost,
    string? Currency,
    int? ActualDurationHours,
    string? PartsUsed,
    string? Notes
) : IRequest<MaintenanceRecordDto>;

/// <summary>
/// Handler for completing maintenance work
/// </summary>
public class CompleteMaintenanceCommandHandler : IRequestHandler<CompleteMaintenanceCommand, MaintenanceRecordDto>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CompleteMaintenanceCommandHandler> _logger;

    public CompleteMaintenanceCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<CompleteMaintenanceCommandHandler> logger)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<MaintenanceRecordDto> Handle(CompleteMaintenanceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing maintenance record {MaintenanceRecordId}", request.MaintenanceRecordId);

        var asset = await _assetRepository.GetByMaintenanceRecordIdAsync(request.MaintenanceRecordId, cancellationToken);
        if (asset == null)
        {
            throw new InvalidOperationException($"Asset with maintenance record ID {request.MaintenanceRecordId} not found");
        }

        var maintenanceRecord = asset.MaintenanceHistory.FirstOrDefault(m => m.Id == request.MaintenanceRecordId);
        if (maintenanceRecord == null)
        {
            throw new InvalidOperationException($"Maintenance record {request.MaintenanceRecordId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Create actual cost if provided
        var actualCost = request.ActualCost.HasValue && !string.IsNullOrEmpty(request.Currency)
            ? Money.Create(request.ActualCost.Value, request.Currency)
            : null;

        // Complete the maintenance
        maintenanceRecord.Complete(
            completionDate: request.CompletionDate,
            workPerformed: request.WorkPerformed,
            actualCost: actualCost,
            actualDurationHours: request.ActualDurationHours,
            partsUsed: request.PartsUsed,
            completedBy: currentUserId);

        // Update asset maintenance status
        asset.CompleteMaintenance(maintenanceRecord.Id, currentUserId);

        // Save changes
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send completion notification
        await _notificationService.SendMaintenanceCompletedNotificationAsync(
            asset.Id, asset.Name, maintenanceRecord.Id, cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(asset.DomainEvents, cancellationToken);
        asset.ClearDomainEvents();

        _logger.LogInformation("Successfully completed maintenance record {MaintenanceRecordId}", request.MaintenanceRecordId);

        return MapToDto(maintenanceRecord, asset);
    }

    private static MaintenanceRecordDto MapToDto(MaintenanceRecord record, Asset asset)
    {
        return new MaintenanceRecordDto
        {
            Id = record.Id,
            AssetId = record.AssetId,
            AssetName = asset.Name,
            AssetNumber = asset.AssetNumber.Value,
            Type = record.Type,
            Description = record.Description,
            Status = record.Status,
            ScheduledDate = record.ScheduledDate,
            DueDate = record.DueDate,
            StartDate = record.StartDate,
            CompletionDate = record.CompletionDate,
            EstimatedCost = record.EstimatedCost?.Amount,
            ActualCost = record.ActualCost?.Amount,
            Currency = record.EstimatedCost?.Currency.ToString() ?? record.ActualCost?.Currency.ToString(),
            EstimatedDurationHours = record.EstimatedDurationHours,
            ActualDurationHours = record.ActualDurationHours,
            AssignedTechnician = record.AssignedTechnician,
            Instructions = record.Instructions,
            WorkPerformed = record.WorkPerformed,
            PartsUsed = record.PartsUsed,
            Priority = record.Priority,
            ScheduledBy = record.ScheduledBy,
            ScheduledAt = record.ScheduledAt,
            CompletedBy = record.CompletedBy
        };
    }
}

/// <summary>
/// Validator for CompleteMaintenanceCommand
/// </summary>
public class CompleteMaintenanceCommandValidator : AbstractValidator<CompleteMaintenanceCommand>
{
    public CompleteMaintenanceCommandValidator()
    {
        RuleFor(x => x.MaintenanceRecordId)
            .NotEmpty()
            .WithMessage("Maintenance record ID is required");

        RuleFor(x => x.CompletionDate)
            .NotEmpty()
            .WithMessage("Completion date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Completion date cannot be in the future");

        RuleFor(x => x.WorkPerformed)
            .NotEmpty()
            .WithMessage("Work performed description is required")
            .MaximumLength(2000)
            .WithMessage("Work performed description cannot exceed 2000 characters");

        RuleFor(x => x.ActualCost)
            .GreaterThan(0)
            .When(x => x.ActualCost.HasValue)
            .WithMessage("Actual cost must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .When(x => x.ActualCost.HasValue)
            .WithMessage("Currency is required when actual cost is provided");

        RuleFor(x => x.ActualDurationHours)
            .GreaterThan(0)
            .When(x => x.ActualDurationHours.HasValue)
            .WithMessage("Actual duration must be greater than 0 hours");
    }
}
