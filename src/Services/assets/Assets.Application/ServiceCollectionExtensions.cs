using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;

namespace TossErp.Assets.Application;

/// <summary>
/// Extension methods for configuring Assets Application services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Assets Application services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAssetsApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Add AutoMapper
        services.AddAutoMapper(assembly);

        // Add FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Add Application Services
        services.AddScoped<IAssetApplicationService, AssetApplicationService>();

        return services;
    }
}

/// <summary>
/// Application service interface for Asset operations
/// </summary>
public interface IAssetApplicationService
{
    Task<AssetDto> CreateAssetAsync(CreateAssetCommand command, CancellationToken cancellationToken = default);
    Task<AssetDto> UpdateAssetAsync(UpdateAssetCommand command, CancellationToken cancellationToken = default);
    Task<AssetDto> ChangeAssetStatusAsync(ChangeAssetStatusCommand command, CancellationToken cancellationToken = default);
    Task<AssetTransferDto> TransferAssetAsync(TransferAssetCommand command, CancellationToken cancellationToken = default);
    Task<List<AssetTransferDto>> BulkTransferAssetsAsync(BulkTransferAssetsCommand command, CancellationToken cancellationToken = default);
    Task<MaintenanceRecordDto> ScheduleMaintenanceAsync(ScheduleMaintenanceCommand command, CancellationToken cancellationToken = default);
    Task<MaintenanceRecordDto> CompleteMaintenanceAsync(CompleteMaintenanceCommand command, CancellationToken cancellationToken = default);
    Task<AssetDisposalDto> DisposeAssetAsync(DisposeAssetCommand command, CancellationToken cancellationToken = default);
    Task<AssetValuationDto> RecordAssetValuationAsync(RecordAssetValuationCommand command, CancellationToken cancellationToken = default);
    
    Task<PagedResult<AssetSummaryDto>> GetAssetsAsync(GetAssetsQuery query, CancellationToken cancellationToken = default);
    Task<AssetDto?> GetAssetByIdAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task<PagedResult<AssetSummaryDto>> GetAssetsByStatusAsync(GetAssetsByStatusQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<MaintenanceRecordDto>> GetMaintenanceRecordsAsync(GetMaintenanceRecordsQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<AssetSummaryDto>> GetMaintenanceDueAsync(GetMaintenanceDueQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<MaintenanceRecordDto>> GetOverdueMaintenanceAsync(GetOverdueMaintenanceQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<MaintenanceRecordDto>> GetAssetMaintenanceHistoryAsync(GetAssetMaintenanceHistoryQuery query, CancellationToken cancellationToken = default);
    Task<List<AssetDepreciationDto>> GetAssetDepreciationReportAsync(GetAssetDepreciationReportQuery query, CancellationToken cancellationToken = default);
    Task<AssetStatisticsDto> GetAssetStatisticsAsync(GetAssetStatisticsQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<AssetTransferDto>> GetAssetTransferHistoryAsync(GetAssetTransferHistoryQuery query, CancellationToken cancellationToken = default);
    Task<List<LocationDto>> GetLocationsAsync(GetLocationsQuery query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Application service implementation for Asset operations
/// </summary>
public class AssetApplicationService : IAssetApplicationService
{
    private readonly IMediator _mediator;
    private readonly ILogger<AssetApplicationService> _logger;

    public AssetApplicationService(
        IMediator mediator,
        ILogger<AssetApplicationService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<AssetDto> CreateAssetAsync(CreateAssetCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating asset: {AssetName}", command.Name);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<AssetDto> UpdateAssetAsync(UpdateAssetCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating asset: {AssetId}", command.AssetId);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<AssetDto> ChangeAssetStatusAsync(ChangeAssetStatusCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Changing asset status: {AssetId} to {NewStatus}", command.AssetId, command.NewStatus);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<AssetTransferDto> TransferAssetAsync(TransferAssetCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Transferring asset: {AssetId}", command.AssetId);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<List<AssetTransferDto>> BulkTransferAssetsAsync(BulkTransferAssetsCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Bulk transferring {AssetCount} assets", command.AssetIds.Count);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<MaintenanceRecordDto> ScheduleMaintenanceAsync(ScheduleMaintenanceCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Scheduling maintenance for asset: {AssetId}", command.AssetId);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<MaintenanceRecordDto> CompleteMaintenanceAsync(CompleteMaintenanceCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completing maintenance: {MaintenanceRecordId}", command.MaintenanceRecordId);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<AssetDisposalDto> DisposeAssetAsync(DisposeAssetCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Disposing asset: {AssetId}", command.AssetId);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<AssetValuationDto> RecordAssetValuationAsync(RecordAssetValuationCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Recording valuation for asset: {AssetId}", command.AssetId);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<PagedResult<AssetSummaryDto>> GetAssetsAsync(GetAssetsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting assets with filters");
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<AssetDto?> GetAssetByIdAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting asset by ID: {AssetId}", assetId);
        return await _mediator.Send(new GetAssetByIdQuery(assetId), cancellationToken);
    }

    public async Task<PagedResult<AssetSummaryDto>> GetAssetsByStatusAsync(GetAssetsByStatusQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting assets by status: {Status}", query.Status);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<PagedResult<MaintenanceRecordDto>> GetMaintenanceRecordsAsync(GetMaintenanceRecordsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting maintenance records");
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<PagedResult<AssetSummaryDto>> GetMaintenanceDueAsync(GetMaintenanceDueQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting assets due for maintenance");
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<PagedResult<MaintenanceRecordDto>> GetOverdueMaintenanceAsync(GetOverdueMaintenanceQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting overdue maintenance");
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<PagedResult<MaintenanceRecordDto>> GetAssetMaintenanceHistoryAsync(GetAssetMaintenanceHistoryQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting maintenance history for asset: {AssetId}", query.AssetId);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<List<AssetDepreciationDto>> GetAssetDepreciationReportAsync(GetAssetDepreciationReportQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting asset depreciation report");
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<AssetStatisticsDto> GetAssetStatisticsAsync(GetAssetStatisticsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting asset statistics");
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<PagedResult<AssetTransferDto>> GetAssetTransferHistoryAsync(GetAssetTransferHistoryQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting asset transfer history");
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<List<LocationDto>> GetLocationsAsync(GetLocationsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting locations");
        return await _mediator.Send(query, cancellationToken);
    }
}
