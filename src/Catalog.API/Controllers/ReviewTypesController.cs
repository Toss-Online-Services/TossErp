using Catalog.API.Services;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewTypesController : ControllerBase
{
    private readonly IReviewTypeService _reviewTypeService;

    public ReviewTypesController(IReviewTypeService reviewTypeService)
    {
        _reviewTypeService = reviewTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewType>>> GetReviewTypes(
        [FromQuery] bool showHidden = false,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = int.MaxValue)
    {
        var reviewTypes = await _reviewTypeService.GetAllReviewTypesAsync(showHidden, pageIndex, pageSize);
        return Ok(reviewTypes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewType>> GetReviewType(int id)
    {
        var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(id);
        if (reviewType == null)
        {
            return NotFound();
        }
        return Ok(reviewType);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ReviewType>>> SearchReviewTypes(
        [FromQuery] string name,
        [FromQuery] bool showHidden = false,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = int.MaxValue)
    {
        var reviewTypes = await _reviewTypeService.GetReviewTypesByNameAsync(name, showHidden, pageIndex, pageSize);
        return Ok(reviewTypes);
    }

    [HttpGet("{id}/reviews")]
    public async Task<ActionResult<IEnumerable<ProductReview>>> GetProductReviewsByReviewType(
        int id,
        [FromQuery] bool showHidden = false,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = int.MaxValue)
    {
        var reviews = await _reviewTypeService.GetProductReviewsByReviewTypeIdAsync(id, showHidden, pageIndex, pageSize);
        return Ok(reviews);
    }

    [HttpGet("{id}/reviews/count")]
    public async Task<ActionResult<int>> GetProductReviewCountByReviewType(
        int id,
        [FromQuery] bool showHidden = false)
    {
        var count = await _reviewTypeService.GetProductReviewCountByReviewTypeIdAsync(id, showHidden);
        return Ok(count);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewType>> CreateReviewType(ReviewType reviewType)
    {
        var createdReviewType = await _reviewTypeService.InsertReviewTypeAsync(reviewType);
        return CreatedAtAction(nameof(GetReviewType), new { id = createdReviewType.Id }, createdReviewType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReviewType(int id, ReviewType reviewType)
    {
        if (id != reviewType.Id)
        {
            return BadRequest();
        }

        var existingReviewType = await _reviewTypeService.GetReviewTypeByIdAsync(id);
        if (existingReviewType == null)
        {
            return NotFound();
        }

        await _reviewTypeService.UpdateReviewTypeAsync(reviewType);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReviewType(int id)
    {
        var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(id);
        if (reviewType == null)
        {
            return NotFound();
        }

        await _reviewTypeService.DeleteReviewTypeAsync(reviewType);
        return NoContent();
    }

    [HttpDelete("batch")]
    public async Task<IActionResult> DeleteReviewTypes([FromBody] IList<ReviewType> reviewTypes)
    {
        await _reviewTypeService.DeleteReviewTypesAsync(reviewTypes);
        return NoContent();
    }
} 
