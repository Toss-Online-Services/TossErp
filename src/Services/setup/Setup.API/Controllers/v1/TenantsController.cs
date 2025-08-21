using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Setup.Application.Common.Interfaces;
using Setup.Application.Features.Tenants.Commands.CreateTenant;
using Setup.Application.Features.Tenants.Commands.UpdateTenant;
using Setup.Application.Features.Tenants.Queries.GetTenant;
using Setup.Application.Features.Tenants.Queries.GetTenants;
using MediatR;

namespace Setup.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITenantService _tenantService;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(IMediator mediator, ITenantService tenantService, ILogger<TenantsController> logger)
    {
        _mediator = mediator;
        _tenantService = tenantService;
        _logger = logger;
    }

    /// <summary>
    /// Get all tenants with optional filtering
    /// </summary>
    /// <param name="status">Filter by tenant status</param>
    /// <param name="subscriptionPlan">Filter by subscription plan</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of tenants</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetTenantsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetTenantsResponse>> GetTenants(
        [FromQuery] string? status = null,
        [FromQuery] string? subscriptionPlan = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var query = new GetTenantsQuery
            {
                Status = status,
                SubscriptionPlan = subscriptionPlan,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenants");
            return BadRequest(new { Message = "Error retrieving tenants", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific tenant by ID
    /// </summary>
    /// <param name="id">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tenant details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetTenantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetTenantResponse>> GetTenant(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetTenantQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { Message = $"Tenant with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenant {TenantId}", id);
            return BadRequest(new { Message = "Error retrieving tenant", Details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new tenant
    /// </summary>
    /// <param name="command">Tenant creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created tenant details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTenantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateTenantResponse>> CreateTenant(
        CreateTenantCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetTenant), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant");
            return BadRequest(new { Message = "Error creating tenant", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing tenant
    /// </summary>
    /// <param name="id">Tenant ID</param>
    /// <param name="command">Tenant update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated tenant details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateTenantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UpdateTenantResponse>> UpdateTenant(
        string id, 
        UpdateTenantCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest(new { Message = "Tenant ID in URL does not match command" });

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant {TenantId}", id);
            return BadRequest(new { Message = "Error updating tenant", Details = ex.Message });
        }
    }

    /// <summary>
    /// Suspend a tenant
    /// </summary>
    /// <param name="id">Tenant ID</param>
    /// <param name="reason">Suspension reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/suspend")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SuspendTenant(
        string id, 
        [FromBody] string reason,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _tenantService.SuspendTenantAsync(id, reason, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Tenant with ID {id} not found" });

            return Ok(new { Message = "Tenant suspended successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suspending tenant {TenantId}", id);
            return BadRequest(new { Message = "Error suspending tenant", Details = ex.Message });
        }
    }

    /// <summary>
    /// Activate a tenant
    /// </summary>
    /// <param name="id">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ActivateTenant(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _tenantService.ActivateTenantAsync(id, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Tenant with ID {id} not found" });

            return Ok(new { Message = "Tenant activated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating tenant {TenantId}", id);
            return BadRequest(new { Message = "Error activating tenant", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get tenant quota status
    /// </summary>
    /// <param name="id">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Quota status details</returns>
    [HttpGet("{id}/quota")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTenantQuota(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var quotaStatus = await _tenantService.GetTenantQuotaStatusAsync(id, cancellationToken);
            return Ok(quotaStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenant quota {TenantId}", id);
            return BadRequest(new { Message = "Error retrieving tenant quota", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get tenant metrics
    /// </summary>
    /// <param name="id">Tenant ID</param>
    /// <param name="fromDate">Start date for metrics</param>
    /// <param name="toDate">End date for metrics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tenant metrics</returns>
    [HttpGet("{id}/metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTenantMetrics(
        string id, 
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var metrics = await _tenantService.GetTenantMetricsAsync(id, fromDate, toDate, cancellationToken);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenant metrics {TenantId}", id);
            return BadRequest(new { Message = "Error retrieving tenant metrics", Details = ex.Message });
        }
    }
}
