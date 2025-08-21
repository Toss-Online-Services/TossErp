using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assets.Application.Common.Interfaces;
using Assets.Application.Features.AssetCategories.Commands.CreateAssetCategory;
using Assets.Application.Features.AssetCategories.Commands.UpdateAssetCategory;
using Assets.Application.Features.AssetCategories.Commands.DeleteAssetCategory;
using Assets.Application.Features.AssetCategories.Queries.GetAssetCategory;
using Assets.Application.Features.AssetCategories.Queries.GetAssetCategories;
using MediatR;

namespace Assets.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class AssetCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAssetCategoryService _assetCategoryService;
    private readonly ILogger<AssetCategoriesController> _logger;

    public AssetCategoriesController(IMediator mediator, IAssetCategoryService assetCategoryService, ILogger<AssetCategoriesController> logger)
    {
        _mediator = mediator;
        _assetCategoryService = assetCategoryService;
        _logger = logger;
    }

    /// <summary>
    /// Get all asset categories with optional filtering
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="parentId">Filter by parent category ID</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="searchTerm">Search in name and description</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of asset categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetAssetCategoriesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetAssetCategoriesResponse>> GetAssetCategories(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? parentId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var query = new GetAssetCategoriesQuery
            {
                TenantId = tenantId,
                ParentId = parentId,
                IsActive = isActive,
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset categories");
            return BadRequest(new { Message = "Error retrieving asset categories", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific asset category by ID
    /// </summary>
    /// <param name="id">Asset category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asset category details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetAssetCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetAssetCategoryResponse>> GetAssetCategory(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetAssetCategoryQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { Message = $"Asset category with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset category {CategoryId}", id);
            return BadRequest(new { Message = "Error retrieving asset category", Details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new asset category
    /// </summary>
    /// <param name="command">Asset category creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created asset category details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateAssetCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateAssetCategoryResponse>> CreateAssetCategory(
        CreateAssetCategoryCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAssetCategory), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset category");
            return BadRequest(new { Message = "Error creating asset category", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing asset category
    /// </summary>
    /// <param name="id">Asset category ID</param>
    /// <param name="command">Asset category update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated asset category details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateAssetCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UpdateAssetCategoryResponse>> UpdateAssetCategory(
        string id, 
        UpdateAssetCategoryCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest(new { Message = "Asset category ID in URL does not match command" });

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset category {CategoryId}", id);
            return BadRequest(new { Message = "Error updating asset category", Details = ex.Message });
        }
    }

    /// <summary>
    /// Delete an asset category
    /// </summary>
    /// <param name="id">Asset category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAssetCategory(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new DeleteAssetCategoryCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Asset category deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset category {CategoryId}", id);
            return BadRequest(new { Message = "Error deleting asset category", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get category hierarchy tree
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="rootId">Root category ID (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category hierarchy tree</returns>
    [HttpGet("hierarchy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategoryHierarchy(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? rootId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var hierarchy = await _assetCategoryService.GetCategoryHierarchyAsync(tenantId, rootId, cancellationToken);
            return Ok(hierarchy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category hierarchy");
            return BadRequest(new { Message = "Error retrieving category hierarchy", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get assets count by category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="includeSubcategories">Include subcategories in count</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Assets count</returns>
    [HttpGet("{id}/assets-count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAssetsCount(
        string id,
        [FromQuery] bool includeSubcategories = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _assetCategoryService.GetAssetsCountAsync(id, includeSubcategories, cancellationToken);
            return Ok(new { CategoryId = id, AssetsCount = count, IncludeSubcategories = includeSubcategories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets count for category {CategoryId}", id);
            return BadRequest(new { Message = "Error retrieving assets count", Details = ex.Message });
        }
    }

    /// <summary>
    /// Move category to new parent
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Move request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MoveCategory(
        string id,
        [FromBody] MoveCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _assetCategoryService.MoveCategoryAsync(id, request.NewParentId, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"Category with ID {id} not found or cannot be moved" });

            return Ok(new { Message = "Category moved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving category {CategoryId}", id);
            return BadRequest(new { Message = "Error moving category", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get category statistics
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategoryStatistics(
        [FromQuery] string? tenantId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var statistics = await _assetCategoryService.GetCategoryStatisticsAsync(
                tenantId, fromDate, toDate, cancellationToken);
            
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category statistics");
            return BadRequest(new { Message = "Error retrieving category statistics", Details = ex.Message });
        }
    }
}

// Request models
public class MoveCategoryRequest
{
    public string? NewParentId { get; set; }
}
