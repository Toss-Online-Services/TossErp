using Microsoft.AspNetCore.Mvc;
using MediatR;
using TossErp.Sales.Application.Commands.CreateSale;
using TossErp.Sales.Application.Commands.CancelSale;
using TossErp.Sales.Application.Queries.GetDailySales;
using TossErp.Sales.Application.Common.DTOs;

namespace TossErp.Sales.API.Controllers;

/// <summary>
/// Sales API controller for managing sales transactions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SalesController> _logger;

    public SalesController(IMediator mediator, ILogger<SalesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new sale
    /// </summary>
    /// <param name="request">Sale creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created sale details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SaleDto>> CreateSale(
        [FromBody] CreateSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateSaleCommand
            {
                TillId = request.TillId,
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                Items = request.Items.Select(item => new CreateSaleItemRequest
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    ItemSku = item.ItemSku,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TaxRate = item.TaxRate
                }).ToList(),
                DiscountAmount = request.DiscountAmount,
                DiscountReason = request.DiscountReason,
                Notes = request.Notes
            };

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetSale), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating sale");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while creating the sale" });
        }
    }

    /// <summary>
    /// Get a sale by ID
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sale details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SaleDto>> GetSale(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement GetSaleQuery
            return NotFound(new { error = "Sale not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sale {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while retrieving the sale" });
        }
    }

    /// <summary>
    /// Cancel a sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <param name="request">Cancellation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cancelled sale details</returns>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SaleDto>> CancelSale(
        Guid id,
        [FromBody] CancelSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CancelSaleCommand
            {
                SaleId = id,
                Reason = request.Reason
            };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while cancelling sale {SaleId}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling sale {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while cancelling the sale" });
        }
    }

    /// <summary>
    /// Get daily sales summary
    /// </summary>
    /// <param name="date">Date for summary (default: today)</param>
    /// <param name="tillId">Optional till ID filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Daily sales summary</returns>
    [HttpGet("daily-summary")]
    [ProducesResponseType(typeof(DailySalesSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DailySalesSummaryDto>> GetDailySales(
        [FromQuery] DateTime? date = null,
        [FromQuery] Guid? tillId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetDailySalesQuery
            {
                Date = date ?? DateTime.Today,
                TillId = tillId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving daily sales summary for date {Date}", date);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while retrieving the daily sales summary" });
        }
    }

    /// <summary>
    /// Get sales by till
    /// </summary>
    /// <param name="tillId">Till ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sales for the till</returns>
    [HttpGet("till/{tillId:guid}")]
    [ProducesResponseType(typeof(List<SaleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<SaleDto>>> GetSalesByTill(
        Guid tillId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement GetSalesByTillQuery
            return Ok(new List<SaleDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales for till {TillId}", tillId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while retrieving sales for the till" });
        }
    }
}

/// <summary>
/// Request DTO for cancelling a sale
/// </summary>
public class CancelSaleRequest
{
    /// <summary>
    /// Reason for cancellation
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
