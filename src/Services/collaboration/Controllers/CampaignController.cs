using Microsoft.AspNetCore.Mvc;
using MediatR;
using Collaboration.Application.Commands;
using Collaboration.Application.Queries;
using Collaboration.Application.DTOs;

namespace Collaboration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CampaignController> _logger;

    public CampaignController(IMediator mediator, ILogger<CampaignController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all campaigns
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CampaignDto>>> GetCampaigns(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Getting campaigns with page: {Page}, pageSize: {PageSize}", page, pageSize);
            
            var campaigns = await _mediator.Send(new GetCampaignsQuery(
                Status: null,
                Type: null,
                Page: page,
                PageSize: pageSize
            ));
            
            return Ok(campaigns);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaigns");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific campaign by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CampaignDetailsDto>> GetCampaign(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting campaign: {CampaignId}", id);
            
            var query = new GetCampaignByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound($"Campaign with ID {id} not found");
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign: {CampaignId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new campaign
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CampaignDto>> CreateCampaign([FromBody] CreateCampaignDto request)
    {
        try
        {
            _logger.LogInformation("Creating campaign: {CampaignName}", request.Name);
            
            var command = new CreateCampaignCommand(
                request.Name,
                request.Description,
                request.Type,
                request.StartDate,
                request.EndDate,
                request.MinParticipants,
                request.MaxParticipants,
                request.TargetAmount,
                request.DiscountPercentage,
                request.CreatedBy,
                request.TenantId
            );

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCampaign), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating campaign");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update campaign details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCampaign(Guid id, [FromBody] UpdateCampaignDto request)
    {
        try
        {
            _logger.LogInformation("Updating campaign: {CampaignId}", id);
            
            var command = new UpdateCampaignCommand(
                id,
                request.Name,
                request.Description,
                request.StartDate,
                request.EndDate
            );

            await _mediator.Send(command);
            return Ok(new { message = "Campaign updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating campaign: {CampaignId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Launch a campaign
    /// </summary>
    [HttpPost("{id}/launch")]
    public async Task<ActionResult> LaunchCampaign(Guid id)
    {
        try
        {
            _logger.LogInformation("Launching campaign: {CampaignId}", id);
            
            var command = new LaunchCampaignCommand(id);
            await _mediator.Send(command);
            
            return Ok(new { message = "Campaign launched successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error launching campaign: {CampaignId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Join a campaign as a participant
    /// </summary>
    [HttpPost("{id}/join")]
    public async Task<ActionResult> JoinCampaign(Guid id, [FromBody] JoinCampaignDto request)
    {
        try
        {
            _logger.LogInformation("User {UserId} joining campaign: {CampaignId}", request.UserId, id);
            
            var command = new JoinCampaignCommand(
                id,
                request.UserId,
                request.CommittedAmount,
                request.DesiredQuantity
            );

            await _mediator.Send(command);
            return Ok(new { message = "Successfully joined campaign" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining campaign: {CampaignId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Leave a campaign
    /// </summary>
    [HttpPost("{id}/leave")]
    public async Task<ActionResult> LeaveCampaign(Guid id, [FromBody] LeaveCampaignDto request)
    {
        try
        {
            _logger.LogInformation("User {UserId} leaving campaign: {CampaignId}", request.UserId, id);
            
            var command = new LeaveCampaignCommand(id, request.UserId);
            await _mediator.Send(command);
            
            return Ok(new { message = "Successfully left campaign" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving campaign: {CampaignId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Complete a campaign
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult> CompleteCampaign(Guid id)
    {
        try
        {
            _logger.LogInformation("Completing campaign: {CampaignId}", id);
            
            var command = new CompleteCampaignCommand(id);
            await _mediator.Send(command);
            
            return Ok(new { message = "Campaign completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing campaign: {CampaignId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a campaign
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelCampaign(Guid id, [FromBody] CancelCampaignDto request)
    {
        try
        {
            _logger.LogInformation("Cancelling campaign: {CampaignId}", id);
            
            var command = new CancelCampaignCommand(id, request.Reason, request.CancelledBy);
            await _mediator.Send(command);
            
            return Ok(new { message = "Campaign cancelled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling campaign: {CampaignId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get campaign participants
    /// </summary>
    [HttpGet("{id}/participants")]
    public async Task<ActionResult<IEnumerable<CampaignParticipantDto>>> GetCampaignParticipants(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting participants for campaign: {CampaignId}", id);
            
            var query = new GetCampaignParticipantsQuery(id);
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign participants: {CampaignId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get campaign analytics
    /// </summary>
    [HttpGet("{id}/analytics")]
    public async Task<ActionResult<CampaignAnalyticsDto>> GetCampaignAnalytics(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting analytics for campaign: {CampaignId}", id);
            
            var query = new GetCampaignAnalyticsQuery(id);
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign analytics: {CampaignId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
