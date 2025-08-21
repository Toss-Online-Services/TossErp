using Microsoft.AspNetCore.Mvc;
using MediatR;
using Collaboration.Application.Commands;
using Collaboration.Application.Queries;
using Collaboration.Application.DTOs;

namespace Collaboration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierQuotationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SupplierQuotationController> _logger;

    public SupplierQuotationController(IMediator mediator, ILogger<SupplierQuotationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new supplier quotation for a campaign
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SupplierQuotationDto>> CreateQuotation([FromBody] CreateSupplierQuotationDto request)
    {
        try
        {
            _logger.LogInformation("Creating supplier quotation for campaign: {CampaignId}", request.CampaignId);
            
            var command = new CreateSupplierQuotationCommand(
                request.CampaignId,
                request.SupplierId,
                request.UnitPrice,
                request.MinQuantity,
                request.MaxQuantity,
                request.ExpiryDate,
                request.Notes
            );

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier quotation");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Accept a supplier quotation
    /// </summary>
    [HttpPost("{id}/accept")]
    public async Task<ActionResult> AcceptQuotation(Guid id)
    {
        try
        {
            _logger.LogInformation("Accepting supplier quotation: {QuotationId}", id);
            
            var command = new AcceptSupplierQuotationCommand(id);
            await _mediator.Send(command);
            
            return Ok(new { message = "Quotation accepted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting supplier quotation: {QuotationId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get best price quotations for a campaign
    /// </summary>
    [HttpGet("campaign/{campaignId}/best-price")]
    public async Task<ActionResult<IEnumerable<QuotationComparisonDto>>> GetBestPriceQuotations(
        Guid campaignId, 
        [FromQuery] int quantity)
    {
        try
        {
            _logger.LogInformation("Getting best price quotations for campaign: {CampaignId} with quantity: {Quantity}", campaignId, quantity);
            
            var query = new GetBestPriceQuotationsQuery(campaignId, quantity);
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting best price quotations for campaign: {CampaignId}", campaignId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all quotations for a campaign
    /// </summary>
    [HttpGet("campaign/{campaignId}")]
    public async Task<ActionResult<IEnumerable<SupplierQuotationDto>>> GetCampaignQuotations(Guid campaignId)
    {
        try
        {
            _logger.LogInformation("Getting quotations for campaign: {CampaignId}", campaignId);
            
            // This would typically use a query handler, but for now we'll return a placeholder
            return Ok(new List<SupplierQuotationDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quotations for campaign: {CampaignId}", campaignId);
            return BadRequest(new { error = ex.Message });
        }
    }
}
