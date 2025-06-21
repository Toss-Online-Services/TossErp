using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TossErp.Application.DTOs;
using TossErp.Application.Services;

namespace TossErp.API.Controllers
{
    /// <summary>
    /// AI Copilot controller for business intelligence and decision support
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CopilotController : ControllerBase
    {
        private readonly ICopilotService _copilotService;
        private readonly ILogger<CopilotController> _logger;

        public CopilotController(ICopilotService copilotService, ILogger<CopilotController> logger)
        {
            _copilotService = copilotService ?? throw new ArgumentNullException(nameof(copilotService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Process a natural language query and get AI-powered response
        /// </summary>
        [HttpPost("query")]
        public async Task<ActionResult<CopilotResponseDto>> ProcessQuery([FromBody] CopilotQueryDto query)
        {
            try
            {
                _logger.LogInformation("Processing Copilot query for business {BusinessId}", query.BusinessId);
                
                var response = await _copilotService.ProcessQueryAsync(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Copilot query for business {BusinessId}", query.BusinessId);
                return StatusCode(500, new { message = "Failed to process query", error = ex.Message });
            }
        }

        /// <summary>
        /// Get business insights and recommendations
        /// </summary>
        [HttpGet("insights/{businessId}")]
        public async Task<ActionResult<CopilotResponseDto>> GetBusinessInsights(Guid businessId)
        {
            try
            {
                _logger.LogInformation("Getting business insights for business {BusinessId}", businessId);
                
                var response = await _copilotService.GetBusinessInsightsAsync(businessId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business insights for business {BusinessId}", businessId);
                return StatusCode(500, new { message = "Failed to get business insights", error = ex.Message });
            }
        }

        /// <summary>
        /// Get inventory management recommendations
        /// </summary>
        [HttpGet("inventory/{businessId}")]
        public async Task<ActionResult<CopilotResponseDto>> GetInventoryRecommendations(Guid businessId)
        {
            try
            {
                _logger.LogInformation("Getting inventory recommendations for business {BusinessId}", businessId);
                
                var response = await _copilotService.GetInventoryRecommendationsAsync(businessId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory recommendations for business {BusinessId}", businessId);
                return StatusCode(500, new { message = "Failed to get inventory recommendations", error = ex.Message });
            }
        }

        /// <summary>
        /// Get sales insights and trends for a date range
        /// </summary>
        [HttpGet("sales/{businessId}")]
        public async Task<ActionResult<CopilotResponseDto>> GetSalesInsights(
            Guid businessId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Getting sales insights for business {BusinessId} from {FromDate} to {ToDate}", 
                    businessId, fromDate, toDate);
                
                var response = await _copilotService.GetSalesInsightsAsync(businessId, fromDate, toDate);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales insights for business {BusinessId}", businessId);
                return StatusCode(500, new { message = "Failed to get sales insights", error = ex.Message });
            }
        }

        /// <summary>
        /// Get financial management recommendations
        /// </summary>
        [HttpGet("financial/{businessId}")]
        public async Task<ActionResult<CopilotResponseDto>> GetFinancialRecommendations(Guid businessId)
        {
            try
            {
                _logger.LogInformation("Getting financial recommendations for business {BusinessId}", businessId);
                
                var response = await _copilotService.GetFinancialRecommendationsAsync(businessId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting financial recommendations for business {BusinessId}", businessId);
                return StatusCode(500, new { message = "Failed to get financial recommendations", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a quick business health check
        /// </summary>
        [HttpGet("health-check/{businessId}")]
        public async Task<ActionResult<object>> GetBusinessHealthCheck(Guid businessId)
        {
            try
            {
                _logger.LogInformation("Getting business health check for business {BusinessId}", businessId);
                
                var insights = await _copilotService.GetBusinessInsightsAsync(businessId);
                var inventory = await _copilotService.GetInventoryRecommendationsAsync(businessId);
                var financial = await _copilotService.GetFinancialRecommendationsAsync(businessId);
                
                var healthCheck = new
                {
                    BusinessId = businessId,
                    Timestamp = DateTime.UtcNow,
                    OverallHealth = "Good", // This could be calculated based on various metrics
                    Insights = insights,
                    InventoryRecommendations = inventory,
                    FinancialRecommendations = financial,
                    PriorityActions = insights.Actions.Take(3).ToList()
                };
                
                return Ok(healthCheck);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business health check for business {BusinessId}", businessId);
                return StatusCode(500, new { message = "Failed to get business health check", error = ex.Message });
            }
        }
    }
} 
