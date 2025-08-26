using Crm.Application.Commands;
using Crm.Application.DTOs;
using Crm.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TossErp.CRM.Domain.Enums;

namespace Crm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpportunitiesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OpportunitiesController> _logger;

    public OpportunitiesController(IMediator mediator, ILogger<OpportunitiesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OpportunitySummaryDto>>> GetOpportunities(
        [FromQuery] OpportunityStage? stage = null,
        [FromQuery] OpportunityType? type = null,
        [FromQuery] OpportunityPriority? priority = null,
        [FromQuery] string? assignedTo = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isOverdue = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetOpportunitiesQuery(stage, type, priority, assignedTo, searchTerm, isOverdue, page, pageSize);
            var opportunities = await _mediator.Send(query, cancellationToken);
            return Ok(opportunities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities");
            return StatusCode(500, "An error occurred while retrieving opportunities");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OpportunityDto>> GetOpportunity(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetOpportunityByIdQuery(id);
            var opportunity = await _mediator.Send(query, cancellationToken);
            
            if (opportunity == null)
                return NotFound($"Opportunity with ID {id} not found");
                
            return Ok(opportunity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunity {OpportunityId}", id);
            return StatusCode(500, "An error occurred while retrieving the opportunity");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOpportunity([FromBody] CreateOpportunityCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var opportunityId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetOpportunity), new { id = opportunityId }, opportunityId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create opportunity: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating opportunity");
            return StatusCode(500, "An error occurred while creating the opportunity");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateOpportunity(Guid id, [FromBody] UpdateOpportunityCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest("Opportunity ID in URL does not match the ID in the request body");
                
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update opportunity {OpportunityId}: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating opportunity {OpportunityId}", id);
            return StatusCode(500, "An error occurred while updating the opportunity");
        }
    }

    [HttpPatch("{id:guid}/stage")]
    public async Task<ActionResult> AdvanceStage(Guid id, [FromBody] AdvanceOpportunityStageCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest("Opportunity ID in URL does not match the ID in the request body");
                
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to advance opportunity stage {OpportunityId}: {Message}", id, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error advancing opportunity stage {OpportunityId}", id);
            return StatusCode(500, "An error occurred while advancing the opportunity stage");
        }
    }

    [HttpPost("{id:guid}/win")]
    public async Task<ActionResult> WinOpportunity(Guid id, [FromBody] WinOpportunityCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest("Opportunity ID in URL does not match the ID in the request body");
                
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to win opportunity {OpportunityId}: {Message}", id, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error winning opportunity {OpportunityId}", id);
            return StatusCode(500, "An error occurred while winning the opportunity");
        }
    }

    [HttpPost("{id:guid}/lose")]
    public async Task<ActionResult> LoseOpportunity(Guid id, [FromBody] LoseOpportunityCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest("Opportunity ID in URL does not match the ID in the request body");
                
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to lose opportunity {OpportunityId}: {Message}", id, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error losing opportunity {OpportunityId}", id);
            return StatusCode(500, "An error occurred while losing the opportunity");
        }
    }

    [HttpGet("by-customer/{customerId:guid}")]
    public async Task<ActionResult<PagedResult<OpportunitySummaryDto>>> GetOpportunitiesByCustomer(
        Guid customerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetOpportunitiesByCustomerQuery(customerId, page, pageSize);
            var opportunities = await _mediator.Send(query, cancellationToken);
            return Ok(opportunities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities for customer {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving opportunities for the customer");
        }
    }

    [HttpGet("closing-soon")]
    public async Task<ActionResult<PagedResult<OpportunitySummaryDto>>> GetOpportunitiesClosingSoon(
        [FromQuery] int daysThreshold = 7,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetOpportunitiesClosingSoonQuery(daysThreshold, page, pageSize);
            var opportunities = await _mediator.Send(query, cancellationToken);
            return Ok(opportunities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities closing soon");
            return StatusCode(500, "An error occurred while retrieving opportunities closing soon");
        }
    }

    [HttpGet("pipeline")]
    public async Task<ActionResult<OpportunityPipelineDto>> GetPipeline(CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetOpportunityPipelineQuery();
            var pipeline = await _mediator.Send(query, cancellationToken);
            return Ok(pipeline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunity pipeline");
            return StatusCode(500, "An error occurred while retrieving the opportunity pipeline");
        }
    }
}
