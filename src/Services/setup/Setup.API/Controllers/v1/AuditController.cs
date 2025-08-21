using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Setup.Application.Common.Interfaces;
using Setup.Application.Features.Audit.Queries.GetAuditEntries;
using Setup.Application.Features.Audit.Queries.GetAuditEntry;
using MediatR;

namespace Setup.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditController> _logger;

    public AuditController(IMediator mediator, IAuditService auditService, ILogger<AuditController> logger)
    {
        _mediator = mediator;
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>
    /// Get audit entries with optional filtering
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="userId">Filter by user ID</param>
    /// <param name="entityType">Filter by entity type</param>
    /// <param name="entityId">Filter by entity ID</param>
    /// <param name="action">Filter by action type</param>
    /// <param name="fromDate">Start date for audit entries</param>
    /// <param name="toDate">End date for audit entries</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 200)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of audit entries</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetAuditEntriesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetAuditEntriesResponse>> GetAuditEntries(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? entityType = null,
        [FromQuery] string? entityId = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 200); // Limit page size

            var query = new GetAuditEntriesQuery
            {
                TenantId = tenantId,
                UserId = userId,
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
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
            _logger.LogError(ex, "Error retrieving audit entries");
            return BadRequest(new { Message = "Error retrieving audit entries", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific audit entry by ID
    /// </summary>
    /// <param name="id">Audit entry ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Audit entry details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetAuditEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetAuditEntryResponse>> GetAuditEntry(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetAuditEntryQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { Message = $"Audit entry with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit entry {AuditEntryId}", id);
            return BadRequest(new { Message = "Error retrieving audit entry", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get audit trail for a specific entity
    /// </summary>
    /// <param name="entityType">Entity type</param>
    /// <param name="entityId">Entity ID</param>
    /// <param name="fromDate">Start date for audit trail</param>
    /// <param name="toDate">End date for audit trail</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 200)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity audit trail</returns>
    [HttpGet("entity/{entityType}/{entityId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEntityAuditTrail(
        string entityType,
        string entityId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 200); // Limit page size

            var trail = await _auditService.GetEntityAuditTrailAsync(
                entityType, entityId, fromDate, toDate, pageNumber, pageSize, cancellationToken);
            
            return Ok(trail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entity audit trail for {EntityType} {EntityId}", entityType, entityId);
            return BadRequest(new { Message = "Error retrieving entity audit trail", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get user activity log
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fromDate">Start date for activity log</param>
    /// <param name="toDate">End date for activity log</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 200)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User activity log</returns>
    [HttpGet("user/{userId}/activity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserActivityLog(
        string userId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 200); // Limit page size

            var activity = await _auditService.GetUserActivityLogAsync(
                userId, fromDate, toDate, pageNumber, pageSize, cancellationToken);
            
            return Ok(activity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user activity log for {UserId}", userId);
            return BadRequest(new { Message = "Error retrieving user activity log", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get audit statistics
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <param name="groupBy">Group statistics by (day, week, month)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Audit statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAuditStatistics(
        [FromQuery] string? tenantId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string groupBy = "day",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var statistics = await _auditService.GetAuditStatisticsAsync(
                tenantId, fromDate, toDate, groupBy, cancellationToken);
            
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit statistics");
            return BadRequest(new { Message = "Error retrieving audit statistics", Details = ex.Message });
        }
    }

    /// <summary>
    /// Export audit log to file
    /// </summary>
    /// <param name="request">Export request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Audit log file</returns>
    [HttpPost("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportAuditLog(
        [FromBody] ExportAuditLogRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var export = await _auditService.ExportAuditLogAsync(
                request.TenantId,
                request.FromDate,
                request.ToDate,
                request.EntityType,
                request.UserId,
                request.Format,
                cancellationToken);

            var fileName = $"audit_log_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{request.Format.ToLower()}";
            var contentType = request.Format.ToLower() switch
            {
                "csv" => "text/csv",
                "json" => "application/json",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            return File(export, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting audit log");
            return BadRequest(new { Message = "Error exporting audit log", Details = ex.Message });
        }
    }

    /// <summary>
    /// Search audit entries
    /// </summary>
    /// <param name="request">Search request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    [HttpPost("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchAuditEntries(
        [FromBody] SearchAuditEntriesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            request.PageSize = Math.Min(request.PageSize, 200); // Limit page size

            var results = await _auditService.SearchAuditEntriesAsync(
                request.SearchTerm,
                request.TenantId,
                request.FromDate,
                request.ToDate,
                request.EntityTypes,
                request.Actions,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
            
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching audit entries");
            return BadRequest(new { Message = "Error searching audit entries", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get compliance report
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="reportType">Report type (GDPR, SOX, HIPAA, etc.)</param>
    /// <param name="fromDate">Start date for report</param>
    /// <param name="toDate">End date for report</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Compliance report</returns>
    [HttpGet("compliance/{reportType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetComplianceReport(
        string reportType,
        [FromQuery] string? tenantId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var report = await _auditService.GenerateComplianceReportAsync(
                reportType, tenantId, fromDate, toDate, cancellationToken);
            
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating compliance report {ReportType}", reportType);
            return BadRequest(new { Message = "Error generating compliance report", Details = ex.Message });
        }
    }

    /// <summary>
    /// Archive old audit entries
    /// </summary>
    /// <param name="request">Archive request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Archive result</returns>
    [HttpPost("archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ArchiveAuditEntries(
        [FromBody] ArchiveAuditEntriesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _auditService.ArchiveAuditEntriesAsync(
                request.BeforeDate,
                request.TenantId,
                request.ArchiveLocation,
                request.DeleteAfterArchive,
                cancellationToken);
            
            return Ok(new {
                Message = "Audit entries archived successfully",
                ArchivedCount = result.ArchivedCount,
                DeletedCount = result.DeletedCount,
                ArchiveLocation = result.ArchiveLocation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving audit entries");
            return BadRequest(new { Message = "Error archiving audit entries", Details = ex.Message });
        }
    }

    /// <summary>
    /// Purge audit entries (permanent deletion)
    /// </summary>
    /// <param name="request">Purge request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Purge result</returns>
    [HttpDelete("purge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PurgeAuditEntries(
        [FromBody] PurgeAuditEntriesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _auditService.PurgeAuditEntriesAsync(
                request.BeforeDate,
                request.TenantId,
                request.ConfirmPurge,
                cancellationToken);
            
            return Ok(new {
                Message = "Audit entries purged successfully",
                PurgedCount = result.PurgedCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error purging audit entries");
            return BadRequest(new { Message = "Error purging audit entries", Details = ex.Message });
        }
    }

    /// <summary>
    /// Validate audit integrity
    /// </summary>
    /// <param name="tenantId">Tenant ID (optional)</param>
    /// <param name="fromDate">Start date for validation</param>
    /// <param name="toDate">End date for validation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Integrity validation result</returns>
    [HttpPost("validate-integrity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ValidateAuditIntegrity(
        [FromQuery] string? tenantId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validation = await _auditService.ValidateAuditIntegrityAsync(
                tenantId, fromDate, toDate, cancellationToken);
            
            return Ok(new {
                IsValid = validation.IsValid,
                TotalEntries = validation.TotalEntries,
                CorruptedEntries = validation.CorruptedEntries,
                MissingEntries = validation.MissingEntries,
                ValidationErrors = validation.ValidationErrors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating audit integrity");
            return BadRequest(new { Message = "Error validating audit integrity", Details = ex.Message });
        }
    }
}

// Request models
public class ExportAuditLogRequest
{
    public string? TenantId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? EntityType { get; set; }
    public string? UserId { get; set; }
    public string Format { get; set; } = "csv"; // csv, json, xlsx
}

public class SearchAuditEntriesRequest
{
    public string SearchTerm { get; set; } = string.Empty;
    public string? TenantId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<string>? EntityTypes { get; set; }
    public List<string>? Actions { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class ArchiveAuditEntriesRequest
{
    public DateTime BeforeDate { get; set; }
    public string? TenantId { get; set; }
    public string ArchiveLocation { get; set; } = string.Empty;
    public bool DeleteAfterArchive { get; set; } = false;
}

public class PurgeAuditEntriesRequest
{
    public DateTime BeforeDate { get; set; }
    public string? TenantId { get; set; }
    public bool ConfirmPurge { get; set; } = false;
}
