using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assets.Application.Common.Interfaces;
using Assets.Application.Features.AssetLocations.Commands.CreateAssetLocation;
using Assets.Application.Features.AssetLocations.Commands.UpdateAssetLocation;
using Assets.Application.Features.AssetLocations.Commands.DeleteAssetLocation;
using Assets.Application.Features.AssetLocations.Queries.GetAssetLocation;
using Assets.Application.Features.AssetLocations.Queries.GetAssetLocations;
using MediatR;

namespace Assets.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class AssetLocationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAssetLocationService _assetLocationService;
    private readonly ILogger<AssetLocationsController> _logger;

    public AssetLocationsController(IMediator mediator, IAssetLocationService assetLocationService, ILogger<AssetLocationsController> logger)
    {
        _mediator = mediator;
        _assetLocationService = assetLocationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all asset locations with optional filtering
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="parentId">Filter by parent location ID</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="locationType">Filter by location type</param>
    /// <param name="searchTerm">Search in name and description</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of asset locations</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetAssetLocationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetAssetLocationsResponse>> GetAssetLocations(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? parentId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? locationType = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var query = new GetAssetLocationsQuery
            {
                TenantId = tenantId,
                ParentId = parentId,
                IsActive = isActive,
                LocationType = locationType,
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset locations");
            return BadRequest(new { Message = "Error retrieving asset locations", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific asset location by ID
    /// </summary>
    /// <param name="id">Asset location ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asset location details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetAssetLocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetAssetLocationResponse>> GetAssetLocation(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetAssetLocationQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { Message = $"Asset location with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset location {LocationId}", id);
            return BadRequest(new { Message = "Error retrieving asset location", Details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new asset location
    /// </summary>
    /// <param name="command">Asset location creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created asset location details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateAssetLocationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateAssetLocationResponse>> CreateAssetLocation(
        CreateAssetLocationCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAssetLocation), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset location");
            return BadRequest(new { Message = "Error creating asset location", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing asset location
    /// </summary>
    /// <param name="id">Asset location ID</param>
    /// <param name="command">Asset location update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated asset location details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateAssetLocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UpdateAssetLocationResponse>> UpdateAssetLocation(
        string id, 
        UpdateAssetLocationCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest(new { Message = "Asset location ID in URL does not match command" });

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset location {LocationId}", id);
            return BadRequest(new { Message = "Error updating asset location", Details = ex.Message });
        }
    }

    /// <summary>
    /// Delete an asset location
    /// </summary>
    /// <param name="id">Asset location ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAssetLocation(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new DeleteAssetLocationCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Asset location deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset location {LocationId}", id);
            return BadRequest(new { Message = "Error deleting asset location", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get location hierarchy tree
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="rootId">Root location ID (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Location hierarchy tree</returns>
    [HttpGet("hierarchy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLocationHierarchy(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? rootId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var hierarchy = await _assetLocationService.GetLocationHierarchyAsync(tenantId, rootId, cancellationToken);
            return Ok(hierarchy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving location hierarchy");
            return BadRequest(new { Message = "Error retrieving location hierarchy", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get assets count by location
    /// </summary>
    /// <param name="id">Location ID</param>
    /// <param name="includeSublocations">Include sublocations in count</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Assets count</returns>
    [HttpGet("{id}/assets-count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAssetsCount(
        string id,
        [FromQuery] bool includeSublocations = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _assetLocationService.GetAssetsCountAsync(id, includeSublocations, cancellationToken);
            return Ok(new { LocationId = id, AssetsCount = count, IncludeSublocations = includeSublocations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets count for location {LocationId}", id);
            return BadRequest(new { Message = "Error retrieving assets count", Details = ex.Message });
        }
    }

    /// <summary>
    /// Move location to new parent
    /// </summary>
    /// <param name="id">Location ID</param>
    /// <param name="request">Move request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MoveLocation(
        string id,
        [FromBody] MoveLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _assetLocationService.MoveLocationAsync(id, request.NewParentId, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Location with ID {id} not found or cannot be moved" });

            return Ok(new { Message = "Location moved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving location {LocationId}", id);
            return BadRequest(new { Message = "Error moving location", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get location capacity and utilization
    /// </summary>
    /// <param name="id">Location ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Location capacity and utilization data</returns>
    [HttpGet("{id}/capacity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLocationCapacity(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var capacity = await _assetLocationService.GetLocationCapacityAsync(id, cancellationToken);
            return Ok(capacity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving location capacity {LocationId}", id);
            return BadRequest(new { Message = "Error retrieving location capacity", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update location capacity
    /// </summary>
    /// <param name="id">Location ID</param>
    /// <param name="request">Capacity update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}/capacity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateLocationCapacity(
        string id,
        [FromBody] UpdateLocationCapacityRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _assetLocationService.UpdateLocationCapacityAsync(
                id, request.MaxAssets, request.MaxValue, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Location with ID {id} not found" });

            return Ok(new { Message = "Location capacity updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location capacity {LocationId}", id);
            return BadRequest(new { Message = "Error updating location capacity", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get location statistics
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="locationType">Filter by location type</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Location statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLocationStatistics(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? locationType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var statistics = await _assetLocationService.GetLocationStatisticsAsync(
                tenantId, locationType, fromDate, toDate, cancellationToken);
            
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving location statistics");
            return BadRequest(new { Message = "Error retrieving location statistics", Details = ex.Message });
        }
    }

    /// <summary>
    /// Export location data
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="format">Export format (csv, excel)</param>
    /// <param name="includeAssets">Include assets in export</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exported location data</returns>
    [HttpGet("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportLocations(
        [FromQuery] string? tenantId = null,
        [FromQuery] string format = "csv",
        [FromQuery] bool includeAssets = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var export = await _assetLocationService.ExportLocationsAsync(
                tenantId, format, includeAssets, cancellationToken);

            var fileName = $"locations_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{format.ToLower()}";
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
            _logger.LogError(ex, "Error exporting locations");
            return BadRequest(new { Message = "Error exporting locations", Details = ex.Message });
        }
    }
}

// Request models
public class MoveLocationRequest
{
    public string? NewParentId { get; set; }
}

public class UpdateLocationCapacityRequest
{
    public int? MaxAssets { get; set; }
    public decimal? MaxValue { get; set; }
}
