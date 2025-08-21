using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Setup.Application.Common.Interfaces;
using Setup.Application.Features.SystemConfig.Commands.UpdateSystemConfig;
using Setup.Application.Features.SystemConfig.Queries.GetSystemConfig;
using MediatR;

namespace Setup.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class SystemConfigController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISystemConfigService _systemConfigService;
    private readonly ILogger<SystemConfigController> _logger;

    public SystemConfigController(IMediator mediator, ISystemConfigService systemConfigService, ILogger<SystemConfigController> logger)
    {
        _mediator = mediator;
        _systemConfigService = systemConfigService;
        _logger = logger;
    }

    /// <summary>
    /// Get system configuration settings
    /// </summary>
    /// <param name="category">Filter by configuration category</param>
    /// <param name="environment">Filter by environment (development, staging, production)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>System configuration settings</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetSystemConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetSystemConfigResponse>> GetSystemConfig(
        [FromQuery] string? category = null,
        [FromQuery] string? environment = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetSystemConfigQuery
            {
                Category = category,
                Environment = environment
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system configuration");
            return BadRequest(new { Message = "Error retrieving system configuration", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific configuration setting by key
    /// </summary>
    /// <param name="key">Configuration key</param>
    /// <param name="environment">Environment filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Configuration setting</returns>
    [HttpGet("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetConfigValue(
        string key, 
        [FromQuery] string? environment = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _systemConfigService.GetConfigValueAsync(key, environment, cancellationToken);
            
            if (value == null)
                return NotFound(new { Message = $"Configuration key '{key}' not found" });

            return Ok(new { Key = key, Value = value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving configuration value for key {Key}", key);
            return BadRequest(new { Message = "Error retrieving configuration value", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update system configuration settings
    /// </summary>
    /// <param name="command">Configuration update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update result</returns>
    [HttpPut]
    [ProducesResponseType(typeof(UpdateSystemConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UpdateSystemConfigResponse>> UpdateSystemConfig(
        UpdateSystemConfigCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating system configuration");
            return BadRequest(new { Message = "Error updating system configuration", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update a specific configuration value
    /// </summary>
    /// <param name="key">Configuration key</param>
    /// <param name="request">Value update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateConfigValue(
        string key, 
        [FromBody] UpdateConfigValueRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _systemConfigService.SetConfigValueAsync(
                key, 
                request.Value, 
                request.Environment, 
                request.Category,
                request.Description,
                cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Configuration key '{key}' not found" });

            return Ok(new { Message = "Configuration value updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configuration value for key {Key}", key);
            return BadRequest(new { Message = "Error updating configuration value", Details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new configuration setting
    /// </summary>
    /// <param name="request">Configuration creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateConfigSetting(
        [FromBody] CreateConfigSettingRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _systemConfigService.SetConfigValueAsync(
                request.Key, 
                request.Value, 
                request.Environment, 
                request.Category,
                request.Description,
                cancellationToken);

            if (success)
                return CreatedAtAction(nameof(GetConfigValue), new { key = request.Key }, 
                    new { Key = request.Key, Value = request.Value });

            return BadRequest(new { Message = "Failed to create configuration setting" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating configuration setting for key {Key}", request.Key);
            return BadRequest(new { Message = "Error creating configuration setting", Details = ex.Message });
        }
    }

    /// <summary>
    /// Delete a configuration setting
    /// </summary>
    /// <param name="key">Configuration key</param>
    /// <param name="environment">Environment filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteConfigSetting(
        string key, 
        [FromQuery] string? environment = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _systemConfigService.DeleteConfigValueAsync(key, environment, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Configuration key '{key}' not found" });

            return Ok(new { Message = "Configuration setting deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting configuration setting for key {Key}", key);
            return BadRequest(new { Message = "Error deleting configuration setting", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get configuration audit trail
    /// </summary>
    /// <param name="key">Configuration key (optional)</param>
    /// <param name="fromDate">Start date for audit trail</param>
    /// <param name="toDate">End date for audit trail</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Configuration audit trail</returns>
    [HttpGet("audit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetConfigAudit(
        [FromQuery] string? key = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var audit = await _systemConfigService.GetConfigAuditAsync(
                key, fromDate, toDate, pageNumber, pageSize, cancellationToken);
            
            return Ok(audit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving configuration audit trail");
            return BadRequest(new { Message = "Error retrieving configuration audit trail", Details = ex.Message });
        }
    }

    /// <summary>
    /// Backup configuration settings
    /// </summary>
    /// <param name="environment">Environment to backup</param>
    /// <param name="category">Category to backup (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Backup file</returns>
    [HttpPost("backup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> BackupConfiguration(
        [FromQuery] string environment,
        [FromQuery] string? category = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var backup = await _systemConfigService.ExportConfigurationAsync(environment, category, cancellationToken);
            
            var fileName = $"config_backup_{environment}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            return File(backup, "application/json", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating configuration backup for environment {Environment}", environment);
            return BadRequest(new { Message = "Error creating configuration backup", Details = ex.Message });
        }
    }

    /// <summary>
    /// Import configuration settings from backup
    /// </summary>
    /// <param name="file">Backup file</param>
    /// <param name="overwrite">Whether to overwrite existing settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Import result</returns>
    [HttpPost("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ImportConfiguration(
        IFormFile file,
        [FromQuery] bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "No file provided" });

            using var stream = file.OpenReadStream();
            var result = await _systemConfigService.ImportConfigurationAsync(stream, overwrite, cancellationToken);
            
            return Ok(new { 
                Message = "Configuration imported successfully", 
                ImportedCount = result.ImportedCount,
                SkippedCount = result.SkippedCount,
                Errors = result.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing configuration");
            return BadRequest(new { Message = "Error importing configuration", Details = ex.Message });
        }
    }

    /// <summary>
    /// Validate configuration settings
    /// </summary>
    /// <param name="environment">Environment to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation results</returns>
    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ValidateConfiguration(
        [FromQuery] string environment,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validation = await _systemConfigService.ValidateConfigurationAsync(environment, cancellationToken);
            
            return Ok(new {
                IsValid = validation.IsValid,
                Errors = validation.Errors,
                Warnings = validation.Warnings,
                MissingRequired = validation.MissingRequired
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating configuration for environment {Environment}", environment);
            return BadRequest(new { Message = "Error validating configuration", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get system health status
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>System health status</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetSystemHealth(CancellationToken cancellationToken = default)
    {
        try
        {
            var health = await _systemConfigService.GetSystemHealthAsync(cancellationToken);
            
            var statusCode = health.IsHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
            return StatusCode(statusCode, health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system health");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                new { Message = "Error retrieving system health", Details = ex.Message });
        }
    }
}

// Request models
public class UpdateConfigValueRequest
{
    public string Value { get; set; } = string.Empty;
    public string? Environment { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}

public class CreateConfigSettingRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Environment { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}
