using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assets.Application.Common.Interfaces;
using Assets.Application.Features.AssetMaintenance.Commands.CreateMaintenanceRecord;
using Assets.Application.Features.AssetMaintenance.Commands.UpdateMaintenanceRecord;
using Assets.Application.Features.AssetMaintenance.Commands.DeleteMaintenanceRecord;
using Assets.Application.Features.AssetMaintenance.Queries.GetMaintenanceRecord;
using Assets.Application.Features.AssetMaintenance.Queries.GetMaintenanceRecords;
using MediatR;

namespace Assets.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class AssetMaintenanceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAssetMaintenanceService _assetMaintenanceService;
    private readonly ILogger<AssetMaintenanceController> _logger;

    public AssetMaintenanceController(IMediator mediator, IAssetMaintenanceService assetMaintenanceService, ILogger<AssetMaintenanceController> logger)
    {
        _mediator = mediator;
        _assetMaintenanceService = assetMaintenanceService;
        _logger = logger;
    }

    /// <summary>
    /// Get all maintenance records with optional filtering
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="assetId">Filter by asset ID</param>
    /// <param name="maintenanceType">Filter by maintenance type</param>
    /// <param name="status">Filter by maintenance status</param>
    /// <param name="performedBy">Filter by who performed the maintenance</param>
    /// <param name="fromDate">Start date for maintenance records</param>
    /// <param name="toDate">End date for maintenance records</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of maintenance records</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetMaintenanceRecordsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetMaintenanceRecordsResponse>> GetMaintenanceRecords(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? assetId = null,
        [FromQuery] string? maintenanceType = null,
        [FromQuery] string? status = null,
        [FromQuery] string? performedBy = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var query = new GetMaintenanceRecordsQuery
            {
                TenantId = tenantId,
                AssetId = assetId,
                MaintenanceType = maintenanceType,
                Status = status,
                PerformedBy = performedBy,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving maintenance records");
            return BadRequest(new { Message = "Error retrieving maintenance records", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific maintenance record by ID
    /// </summary>
    /// <param name="id">Maintenance record ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Maintenance record details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetMaintenanceRecordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetMaintenanceRecordResponse>> GetMaintenanceRecord(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetMaintenanceRecordQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { Message = $"Maintenance record with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving maintenance record {MaintenanceRecordId}", id);
            return BadRequest(new { Message = "Error retrieving maintenance record", Details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new maintenance record
    /// </summary>
    /// <param name="command">Maintenance record creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created maintenance record details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateMaintenanceRecordResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateMaintenanceRecordResponse>> CreateMaintenanceRecord(
        CreateMaintenanceRecordCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetMaintenanceRecord), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating maintenance record");
            return BadRequest(new { Message = "Error creating maintenance record", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing maintenance record
    /// </summary>
    /// <param name="id">Maintenance record ID</param>
    /// <param name="command">Maintenance record update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated maintenance record details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateMaintenanceRecordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UpdateMaintenanceRecordResponse>> UpdateMaintenanceRecord(
        string id, 
        UpdateMaintenanceRecordCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest(new { Message = "Maintenance record ID in URL does not match command" });

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating maintenance record {MaintenanceRecordId}", id);
            return BadRequest(new { Message = "Error updating maintenance record", Details = ex.Message });
        }
    }

    /// <summary>
    /// Delete a maintenance record
    /// </summary>
    /// <param name="id">Maintenance record ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteMaintenanceRecord(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new DeleteMaintenanceRecordCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Maintenance record deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting maintenance record {MaintenanceRecordId}", id);
            return BadRequest(new { Message = "Error deleting maintenance record", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get maintenance schedule for an asset
    /// </summary>
    /// <param name="assetId">Asset ID</param>
    /// <param name="fromDate">Start date for schedule</param>
    /// <param name="toDate">End date for schedule</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Maintenance schedule</returns>
    [HttpGet("asset/{assetId}/schedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMaintenanceSchedule(
        string assetId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _assetMaintenanceService.GetMaintenanceScheduleAsync(
                assetId, fromDate, toDate, cancellationToken);
            
            return Ok(schedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving maintenance schedule for asset {AssetId}", assetId);
            return BadRequest(new { Message = "Error retrieving maintenance schedule", Details = ex.Message });
        }
    }

    /// <summary>
    /// Schedule preventive maintenance for an asset
    /// </summary>
    /// <param name="assetId">Asset ID</param>
    /// <param name="request">Schedule request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("asset/{assetId}/schedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SchedulePreventiveMaintenance(
        string assetId,
        [FromBody] ScheduleMaintenanceRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _assetMaintenanceService.SchedulePreventiveMaintenanceAsync(
                assetId, request.MaintenanceType, request.ScheduledDate, 
                request.RecurrencePattern, request.Description, request.EstimatedCost, 
                request.AssignedTechnician, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Asset with ID {assetId} not found" });

            return Ok(new { Message = "Preventive maintenance scheduled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling preventive maintenance for asset {AssetId}", assetId);
            return BadRequest(new { Message = "Error scheduling preventive maintenance", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get overdue maintenance records
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="daysPastDue">Minimum days past due (default: 0)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Overdue maintenance records</returns>
    [HttpGet("overdue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOverdueMaintenance(
        [FromQuery] string? tenantId = null,
        [FromQuery] int daysPastDue = 0,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var overdue = await _assetMaintenanceService.GetOverdueMaintenanceAsync(
                tenantId, daysPastDue, pageNumber, pageSize, cancellationToken);
            
            return Ok(overdue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue maintenance");
            return BadRequest(new { Message = "Error retrieving overdue maintenance", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get upcoming maintenance records
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="daysAhead">Days ahead to look for upcoming maintenance (default: 30)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upcoming maintenance records</returns>
    [HttpGet("upcoming")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUpcomingMaintenance(
        [FromQuery] string? tenantId = null,
        [FromQuery] int daysAhead = 30,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var upcoming = await _assetMaintenanceService.GetUpcomingMaintenanceAsync(
                tenantId, daysAhead, pageNumber, pageSize, cancellationToken);
            
            return Ok(upcoming);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming maintenance");
            return BadRequest(new { Message = "Error retrieving upcoming maintenance", Details = ex.Message });
        }
    }

    /// <summary>
    /// Complete a maintenance record
    /// </summary>
    /// <param name="id">Maintenance record ID</param>
    /// <param name="request">Completion request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CompleteMaintenance(
        string id,
        [FromBody] CompleteMaintenanceRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _assetMaintenanceService.CompleteMaintenanceAsync(
                id, request.CompletedBy, request.CompletionDate, request.ActualCost, 
                request.WorkPerformed, request.PartsUsed, request.NextMaintenanceDate, 
                cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Maintenance record with ID {id} not found" });

            return Ok(new { Message = "Maintenance completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing maintenance {MaintenanceRecordId}", id);
            return BadRequest(new { Message = "Error completing maintenance", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get maintenance statistics
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="assetId">Filter by asset ID</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Maintenance statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMaintenanceStatistics(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? assetId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var statistics = await _assetMaintenanceService.GetMaintenanceStatisticsAsync(
                tenantId, assetId, fromDate, toDate, cancellationToken);
            
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving maintenance statistics");
            return BadRequest(new { Message = "Error retrieving maintenance statistics", Details = ex.Message });
        }
    }

    /// <summary>
    /// Export maintenance records
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="assetId">Filter by asset ID</param>
    /// <param name="fromDate">Start date for export</param>
    /// <param name="toDate">End date for export</param>
    /// <param name="format">Export format (csv, excel)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exported maintenance data</returns>
    [HttpGet("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportMaintenanceRecords(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? assetId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string format = "csv",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var export = await _assetMaintenanceService.ExportMaintenanceRecordsAsync(
                tenantId, assetId, fromDate, toDate, format, cancellationToken);

            var fileName = $"maintenance_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{format.ToLower()}";
            var contentType = format.ToLower() switch
            {
                "csv" => "text/csv",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            return File(export, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting maintenance records");
            return BadRequest(new { Message = "Error exporting maintenance records", Details = ex.Message });
        }
    }
}

// Request models
public class ScheduleMaintenanceRequest
{
    public string MaintenanceType { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string? RecurrencePattern { get; set; }
    public string? Description { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string? AssignedTechnician { get; set; }
}

public class CompleteMaintenanceRequest
{
    public string CompletedBy { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public decimal? ActualCost { get; set; }
    public string? WorkPerformed { get; set; }
    public List<string>? PartsUsed { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
}
