using Microsoft.AspNetCore.Mvc;
using TossErp.Application.DTOs;
using TossErp.Application.Services;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CopilotController : ControllerBase
{
    private readonly ICopilotService _copilotService;
    private readonly ILogger<CopilotController> _logger;

    public CopilotController(ICopilotService copilotService, ILogger<CopilotController> logger)
    {
        _copilotService = copilotService;
        _logger = logger;
    }

    [HttpPost("query")]
    public async Task<ActionResult<CopilotResponseDto>> ProcessQuery([FromBody] CopilotQueryDto queryDto)
    {
        try
        {
            var response = await _copilotService.ProcessQueryAsync(
                queryDto.Query, 
                queryDto.BusinessId, 
                queryDto.UserId);

            var copilotResponse = new CopilotResponseDto
            {
                Response = response,
                Type = "text",
                Actions = new List<string>(),
                Suggestions = new List<string>(),
                RequiresAction = false
            };

            return Ok(copilotResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing copilot query for business {BusinessId}", queryDto.BusinessId);
            return StatusCode(500, "An error occurred while processing your query");
        }
    }

    [HttpGet("sales-insights")]
    public async Task<ActionResult<string>> GetSalesInsights(
        [FromQuery] Guid businessId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        try
        {
            var insights = await _copilotService.GetSalesInsightsAsync(businessId, fromDate, toDate);
            return Ok(insights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales insights for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while retrieving sales insights");
        }
    }

    [HttpGet("inventory-alerts")]
    public async Task<ActionResult<string>> GetInventoryAlerts([FromQuery] Guid businessId)
    {
        try
        {
            var alerts = await _copilotService.GetInventoryAlertsAsync(businessId);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory alerts for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while retrieving inventory alerts");
        }
    }

    [HttpGet("group-purchase-suggestions")]
    public async Task<ActionResult<string>> GetGroupPurchaseSuggestions([FromQuery] Guid businessId)
    {
        try
        {
            var suggestions = await _copilotService.GetGroupPurchaseSuggestionsAsync(businessId);
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting group purchase suggestions for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while retrieving group purchase suggestions");
        }
    }

    [HttpGet("promotion-suggestions")]
    public async Task<ActionResult<string>> GetPromotionSuggestions([FromQuery] Guid businessId)
    {
        try
        {
            var suggestions = await _copilotService.GetPromotionSuggestionsAsync(businessId);
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting promotion suggestions for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while retrieving promotion suggestions");
        }
    }

    [HttpGet("business-recommendations")]
    public async Task<ActionResult<string>> GetBusinessRecommendations([FromQuery] Guid businessId)
    {
        try
        {
            var recommendations = await _copilotService.GetBusinessRecommendationsAsync(businessId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting business recommendations for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while retrieving business recommendations");
        }
    }
} 
