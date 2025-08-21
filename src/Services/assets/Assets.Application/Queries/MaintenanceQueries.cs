using FluentValidation;

namespace TossErp.Assets.Application.Queries;

/// <summary>
/// Query to get maintenance records with filtering and pagination
/// </summary>
public record GetMaintenanceRecordsQuery(
    Guid? AssetId = null,
    MaintenanceStatus? Status = null,
    MaintenanceType? Type = null,
    DateOnly? ScheduledDateFrom = null,
    DateOnly? ScheduledDateTo = null,
    DateOnly? DueDateFrom = null,
    DateOnly? DueDateTo = null,
    string? AssignedTechnician = null,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = "ScheduledDate",
    bool SortDescending = false
) : IRequest<PagedResult<MaintenanceRecordDto>>;

/// <summary>
/// Handler for getting maintenance records with filtering and pagination
/// </summary>
public class GetMaintenanceRecordsQueryHandler : IRequestHandler<GetMaintenanceRecordsQuery, PagedResult<MaintenanceRecordDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetMaintenanceRecordsQueryHandler> _logger;

    public GetMaintenanceRecordsQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetMaintenanceRecordsQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<PagedResult<MaintenanceRecordDto>> Handle(GetMaintenanceRecordsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting maintenance records with filters - Page: {PageNumber}, Size: {PageSize}", 
            request.PageNumber, request.PageSize);

        var (maintenanceRecords, totalCount) = await _assetRepository.GetMaintenanceRecordsAsync(
            assetId: request.AssetId,
            status: request.Status,
            type: request.Type,
            scheduledDateFrom: request.ScheduledDateFrom,
            scheduledDateTo: request.ScheduledDateTo,
            dueDateFrom: request.DueDateFrom,
            dueDateTo: request.DueDateTo,
            assignedTechnician: request.AssignedTechnician,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            cancellationToken: cancellationToken);

        var maintenanceDto = maintenanceRecords.Select(mr => MapToDto(mr.MaintenanceRecord, mr.Asset)).ToList();

        return new PagedResult<MaintenanceRecordDto>
        {
            Items = maintenanceDto,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
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
/// Validator for GetMaintenanceRecordsQuery
/// </summary>
public class GetMaintenanceRecordsQueryValidator : AbstractValidator<GetMaintenanceRecordsQuery>
{
    public GetMaintenanceRecordsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.ScheduledDateFrom)
            .LessThanOrEqualTo(x => x.ScheduledDateTo)
            .When(x => x.ScheduledDateFrom.HasValue && x.ScheduledDateTo.HasValue)
            .WithMessage("Scheduled date from must be before or equal to scheduled date to");

        RuleFor(x => x.DueDateFrom)
            .LessThanOrEqualTo(x => x.DueDateTo)
            .When(x => x.DueDateFrom.HasValue && x.DueDateTo.HasValue)
            .WithMessage("Due date from must be before or equal to due date to");
    }
}

/// <summary>
/// Query to get assets due for maintenance
/// </summary>
public record GetMaintenanceDueQuery(
    DateOnly? DueDate = null,
    int DaysAhead = 30,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedResult<AssetSummaryDto>>;

/// <summary>
/// Handler for getting assets due for maintenance
/// </summary>
public class GetMaintenanceDueQueryHandler : IRequestHandler<GetMaintenanceDueQuery, PagedResult<AssetSummaryDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetMaintenanceDueQueryHandler> _logger;

    public GetMaintenanceDueQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetMaintenanceDueQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<PagedResult<AssetSummaryDto>> Handle(GetMaintenanceDueQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting assets due for maintenance within {DaysAhead} days", request.DaysAhead);

        var dueDate = request.DueDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(request.DaysAhead));

        var (assets, totalCount) = await _assetRepository.GetAssetsMaintenanceDueAsync(
            dueDate: dueDate,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
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
/// Validator for GetMaintenanceDueQuery
/// </summary>
public class GetMaintenanceDueQueryValidator : AbstractValidator<GetMaintenanceDueQuery>
{
    public GetMaintenanceDueQueryValidator()
    {
        RuleFor(x => x.DaysAhead)
            .GreaterThan(0)
            .WithMessage("Days ahead must be greater than 0");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

/// <summary>
/// Query to get overdue maintenance
/// </summary>
public record GetOverdueMaintenanceQuery(
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedResult<MaintenanceRecordDto>>;

/// <summary>
/// Handler for getting overdue maintenance
/// </summary>
public class GetOverdueMaintenanceQueryHandler : IRequestHandler<GetOverdueMaintenanceQuery, PagedResult<MaintenanceRecordDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetOverdueMaintenanceQueryHandler> _logger;

    public GetOverdueMaintenanceQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetOverdueMaintenanceQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<PagedResult<MaintenanceRecordDto>> Handle(GetOverdueMaintenanceQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting overdue maintenance records");

        var today = DateOnly.FromDateTime(DateTime.Today);

        var (maintenanceRecords, totalCount) = await _assetRepository.GetOverdueMaintenanceAsync(
            currentDate: today,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var maintenanceDto = maintenanceRecords.Select(mr => MapToDto(mr.MaintenanceRecord, mr.Asset)).ToList();

        return new PagedResult<MaintenanceRecordDto>
        {
            Items = maintenanceDto,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
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
/// Validator for GetOverdueMaintenanceQuery
/// </summary>
public class GetOverdueMaintenanceQueryValidator : AbstractValidator<GetOverdueMaintenanceQuery>
{
    public GetOverdueMaintenanceQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

/// <summary>
/// Query to get asset maintenance history
/// </summary>
public record GetAssetMaintenanceHistoryQuery(
    Guid AssetId,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedResult<MaintenanceRecordDto>>;

/// <summary>
/// Handler for getting asset maintenance history
/// </summary>
public class GetAssetMaintenanceHistoryQueryHandler : IRequestHandler<GetAssetMaintenanceHistoryQuery, PagedResult<MaintenanceRecordDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly ILogger<GetAssetMaintenanceHistoryQueryHandler> _logger;

    public GetAssetMaintenanceHistoryQueryHandler(
        IAssetRepository assetRepository,
        ILogger<GetAssetMaintenanceHistoryQueryHandler> logger)
    {
        _assetRepository = assetRepository;
        _logger = logger;
    }

    public async Task<PagedResult<MaintenanceRecordDto>> Handle(GetAssetMaintenanceHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting maintenance history for asset {AssetId}", request.AssetId);

        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset == null)
        {
            throw new InvalidOperationException($"Asset with ID {request.AssetId} not found");
        }

        var maintenanceRecords = asset.MaintenanceHistory
            .OrderByDescending(m => m.ScheduledDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var totalCount = asset.MaintenanceHistory.Count;

        var maintenanceDto = maintenanceRecords.Select(record => MapToDto(record, asset)).ToList();

        return new PagedResult<MaintenanceRecordDto>
        {
            Items = maintenanceDto,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
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
/// Validator for GetAssetMaintenanceHistoryQuery
/// </summary>
public class GetAssetMaintenanceHistoryQueryValidator : AbstractValidator<GetAssetMaintenanceHistoryQuery>
{
    public GetAssetMaintenanceHistoryQueryValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}
