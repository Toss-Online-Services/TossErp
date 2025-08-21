using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assets.Application.Common.Interfaces;
using Assets.Application.Features.Assets.Commands.CreateAsset;
using Assets.Application.Features.Assets.Commands.UpdateAsset;
using Assets.Application.Features.Assets.Commands.DeleteAsset;
using Assets.Application.Features.Assets.Queries.GetAsset;
using Assets.Application.Features.Assets.Queries.GetAssets;
using MediatR;

namespace Assets.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAssetService _assetService;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(IMediator mediator, IAssetService assetService, ILogger<AssetsController> logger)
    {
        _mediator = mediator;
        _assetService = assetService;
        _logger = logger;
    }

    /// <summary>
    /// Get all assets with optional filtering
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="categoryId">Filter by category ID</param>
    /// <param name="status">Filter by asset status</param>
    /// <param name="locationId">Filter by location ID</param>
    /// <param name="searchTerm">Search in name and description</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of assets</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetAssetsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetAssetsResponse>> GetAssets(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? categoryId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? locationId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var query = new GetAssetsQuery
            {
                TenantId = tenantId,
                CategoryId = categoryId,
                Status = status,
                LocationId = locationId,
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets");
            return BadRequest(new { Message = "Error retrieving assets", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific asset by ID
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asset details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetAssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetAssetResponse>> GetAsset(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetAssetQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { Message = $"Asset with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset {AssetId}", id);
            return BadRequest(new { Message = "Error retrieving asset", Details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new asset
    /// </summary>
    /// <param name="command">Asset creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created asset details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateAssetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateAssetResponse>> CreateAsset(
        CreateAssetCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAsset), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            return BadRequest(new { Message = "Error creating asset", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing asset
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="command">Asset update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated asset details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateAssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UpdateAssetResponse>> UpdateAsset(
        string id, 
        UpdateAssetCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest(new { Message = "Asset ID in URL does not match command" });

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset {AssetId}", id);
            return BadRequest(new { Message = "Error updating asset", Details = ex.Message });
        }
    }

    /// <summary>
    /// Delete an asset
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAsset(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new DeleteAssetCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Asset deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset {AssetId}", id);
            return BadRequest(new { Message = "Error deleting asset", Details = ex.Message });
        }
    }

    /// <summary>
    /// Check out an asset
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="request">Check out request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/checkout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckOutAsset(
        string id,
        [FromBody] CheckOutAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _assetService.CheckOutAssetAsync(
                id, request.AssignedToUserId, request.Notes, request.ExpectedReturnDate, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Asset with ID {id} not found or cannot be checked out" });

            return Ok(new { Message = "Asset checked out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking out asset {AssetId}", id);
            return BadRequest(new { Message = "Error checking out asset", Details = ex.Message });
        }
    }

    /// <summary>
    /// Check in an asset
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="request">Check in request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/checkin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckInAsset(
        string id,
        [FromBody] CheckInAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _assetService.CheckInAssetAsync(
                id, request.Condition, request.Notes, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Asset with ID {id} not found or cannot be checked in" });

            return Ok(new { Message = "Asset checked in successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking in asset {AssetId}", id);
            return BadRequest(new { Message = "Error checking in asset", Details = ex.Message });
        }
    }

    /// <summary>
    /// Transfer an asset to a new location
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="request">Transfer request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> TransferAsset(
        string id,
        [FromBody] TransferAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _assetService.TransferAssetAsync(
                id, request.NewLocationId, request.TransferReason, request.Notes, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Asset with ID {id} not found or cannot be transferred" });

            return Ok(new { Message = "Asset transferred successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring asset {AssetId}", id);
            return BadRequest(new { Message = "Error transferring asset", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get asset audit trail
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="fromDate">Start date for audit trail</param>
    /// <param name="toDate">End date for audit trail</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asset audit trail</returns>
    [HttpGet("{id}/audit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAssetAuditTrail(
        string id,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var auditTrail = await _assetService.GetAssetAuditTrailAsync(
                id, fromDate, toDate, pageNumber, pageSize, cancellationToken);
            
            return Ok(auditTrail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset audit trail {AssetId}", id);
            return BadRequest(new { Message = "Error retrieving asset audit trail", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get asset depreciation schedule
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asset depreciation schedule</returns>
    [HttpGet("{id}/depreciation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAssetDepreciation(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var depreciation = await _assetService.GetDepreciationScheduleAsync(id, cancellationToken);
            return Ok(depreciation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset depreciation {AssetId}", id);
            return BadRequest(new { Message = "Error retrieving asset depreciation", Details = ex.Message });
        }
    }

    /// <summary>
    /// Calculate current asset value
    /// </summary>
    /// <param name="id">Asset ID</param>
    /// <param name="asOfDate">Calculate value as of this date (optional, defaults to today)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current asset value</returns>
    [HttpGet("{id}/value")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CalculateAssetValue(
        string id,
        [FromQuery] DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _assetService.CalculateCurrentValueAsync(id, asOfDate, cancellationToken);
            return Ok(new { AssetId = id, CurrentValue = value, AsOfDate = asOfDate ?? DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating asset value {AssetId}", id);
            return BadRequest(new { Message = "Error calculating asset value", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get asset statistics
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="categoryId">Filter by category ID</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asset statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAssetStatistics(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? categoryId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var statistics = await _assetService.GetAssetStatisticsAsync(
                tenantId, categoryId, fromDate, toDate, cancellationToken);
            
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset statistics");
            return BadRequest(new { Message = "Error retrieving asset statistics", Details = ex.Message });
        }
    }
}

// Request models
public class CheckOutAssetRequest
{
    public string AssignedToUserId { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
}

public class CheckInAssetRequest
{
    public string Condition { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class TransferAssetRequest
{
    public string NewLocationId { get; set; } = string.Empty;
    public string TransferReason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
