using Crm.Application.Commands;
using Crm.Application.DTOs;
using Crm.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TossErp.CRM.Domain.Enums;

namespace Crm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LeadsController> _logger;

    public LeadsController(IMediator mediator, ILogger<LeadsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<LeadSummaryDto>>> GetLeads(
        [FromQuery] LeadStatus? status = null,
        [FromQuery] LeadSource? source = null,
        [FromQuery] string? assignedTo = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetLeadsQuery(status, source, assignedTo, searchTerm, page, pageSize);
            var leads = await _mediator.Send(query, cancellationToken);
            return Ok(leads);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads");
            return StatusCode(500, "An error occurred while retrieving leads");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeadDto>> GetLead(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetLeadByIdQuery(id);
            var lead = await _mediator.Send(query, cancellationToken);
            
            if (lead == null)
                return NotFound($"Lead with ID {id} not found");
                
            return Ok(lead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lead {LeadId}", id);
            return StatusCode(500, "An error occurred while retrieving the lead");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateLead([FromBody] CreateLeadCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var leadId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetLead), new { id = leadId }, leadId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create lead: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lead");
            return StatusCode(500, "An error occurred while creating the lead");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateLead(Guid id, [FromBody] UpdateLeadCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest("Lead ID in URL does not match the ID in the request body");
                
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update lead {LeadId}: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lead {LeadId}", id);
            return StatusCode(500, "An error occurred while updating the lead");
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult> ChangeStatus(Guid id, [FromBody] ChangeLeadStatusCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest("Lead ID in URL does not match the ID in the request body");
                
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to change lead status {LeadId}: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing lead status {LeadId}", id);
            return StatusCode(500, "An error occurred while changing the lead status");
        }
    }

    [HttpPost("{id:guid}/convert")]
    public async Task<ActionResult<ConvertLeadResult>> ConvertLead(Guid id, [FromBody] ConvertLeadCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.LeadId)
                return BadRequest("Lead ID in URL does not match the ID in the request body");
                
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to convert lead {LeadId}: {Message}", id, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting lead {LeadId}", id);
            return StatusCode(500, "An error occurred while converting the lead");
        }
    }

    [HttpGet("hot")]
    public async Task<ActionResult<PagedResult<LeadSummaryDto>>> GetHotLeads(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetHotLeadsQuery(page, pageSize);
            var leads = await _mediator.Send(query, cancellationToken);
            return Ok(leads);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hot leads");
            return StatusCode(500, "An error occurred while retrieving hot leads");
        }
    }

    [HttpGet("stale")]
    public async Task<ActionResult<PagedResult<LeadSummaryDto>>> GetStaleLeads(
        [FromQuery] int daysThreshold = 30,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetStaleLeadsQuery(daysThreshold, page, pageSize);
            var leads = await _mediator.Send(query, cancellationToken);
            return Ok(leads);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stale leads");
            return StatusCode(500, "An error occurred while retrieving stale leads");
        }
    }

    [HttpGet("by-score")]
    public async Task<ActionResult<PagedResult<LeadSummaryDto>>> GetLeadsByScore(
        [FromQuery] int minScore = 0,
        [FromQuery] int maxScore = 100,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetLeadsByScoreQuery(minScore, maxScore, page, pageSize);
            var leads = await _mediator.Send(query, cancellationToken);
            return Ok(leads);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by score");
            return StatusCode(500, "An error occurred while retrieving leads by score");
        }
    }

    [HttpGet("by-campaign/{campaignName}")]
    public async Task<ActionResult<PagedResult<LeadSummaryDto>>> GetLeadsByCampaign(
        string campaignName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(campaignName))
                return BadRequest("Campaign name is required");
                
            var query = new GetLeadsByCampaignQuery(campaignName, page, pageSize);
            var leads = await _mediator.Send(query, cancellationToken);
            return Ok(leads);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by campaign: {CampaignName}", campaignName);
            return StatusCode(500, "An error occurred while retrieving leads by campaign");
        }
    }
}
