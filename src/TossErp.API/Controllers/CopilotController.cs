using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TossErp.Copilot.Domain;
using TossErp.Shared.DTOs;

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
        public async Task<ActionResult<CommonResponseDto<string>>> ProcessQuery([FromBody] string prompt)
        {
            try
            {
                _logger.LogInformation("Processing Copilot query: {Prompt}", prompt);
                
                var response = await _copilotService.GetCopilotResponseAsync(prompt);
                return Ok(new CommonResponseDto<string>
                {
                    Success = true,
                    Data = response,
                    Message = "Query processed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Copilot query: {Prompt}", prompt);
                return BadRequest(new CommonResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to process query: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get business insights and recommendations
        /// </summary>
        [HttpGet("insights")]
        public async Task<ActionResult<CommonResponseDto<string>>> GetBusinessInsights()
        {
            try
            {
                _logger.LogInformation("Getting business insights");
                
                var prompt = "Provide business insights and recommendations for a township enterprise";
                var response = await _copilotService.GetCopilotResponseAsync(prompt);
                return Ok(new CommonResponseDto<string>
                {
                    Success = true,
                    Data = response,
                    Message = "Business insights retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business insights");
                return BadRequest(new CommonResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to get business insights: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get inventory management recommendations
        /// </summary>
        [HttpGet("inventory")]
        public async Task<ActionResult<CommonResponseDto<string>>> GetInventoryRecommendations()
        {
            try
            {
                _logger.LogInformation("Getting inventory recommendations");
                
                var prompt = "Provide inventory management recommendations for a small business";
                var response = await _copilotService.GetCopilotResponseAsync(prompt);
                return Ok(new CommonResponseDto<string>
                {
                    Success = true,
                    Data = response,
                    Message = "Inventory recommendations retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory recommendations");
                return BadRequest(new CommonResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to get inventory recommendations: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get sales insights and trends
        /// </summary>
        [HttpGet("sales")]
        public async Task<ActionResult<CommonResponseDto<string>>> GetSalesInsights()
        {
            try
            {
                _logger.LogInformation("Getting sales insights");
                
                var prompt = "Provide sales insights and trends analysis for a township business";
                var response = await _copilotService.GetCopilotResponseAsync(prompt);
                return Ok(new CommonResponseDto<string>
                {
                    Success = true,
                    Data = response,
                    Message = "Sales insights retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales insights");
                return BadRequest(new CommonResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to get sales insights: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get financial management recommendations
        /// </summary>
        [HttpGet("financial")]
        public async Task<ActionResult<CommonResponseDto<string>>> GetFinancialRecommendations()
        {
            try
            {
                _logger.LogInformation("Getting financial recommendations");
                
                var prompt = "Provide financial management recommendations for a small township enterprise";
                var response = await _copilotService.GetCopilotResponseAsync(prompt);
                return Ok(new CommonResponseDto<string>
                {
                    Success = true,
                    Data = response,
                    Message = "Financial recommendations retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting financial recommendations");
                return BadRequest(new CommonResponseDto<string>
                {
                    Success = false,
                    Message = $"Failed to get financial recommendations: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get a quick business health check
        /// </summary>
        [HttpGet("health-check")]
        public async Task<ActionResult<CommonResponseDto<object>>> GetBusinessHealthCheck()
        {
            try
            {
                _logger.LogInformation("Getting business health check");
                
                var prompt = "Provide a comprehensive business health check and priority actions for a township enterprise";
                var response = await _copilotService.GetCopilotResponseAsync(prompt);
                
                var healthCheck = new
                {
                    Timestamp = DateTime.UtcNow,
                    Analysis = response,
                    OverallHealth = "Good", // This could be calculated based on various metrics
                    PriorityActions = new List<string> { "Review inventory levels", "Analyze sales trends", "Check financial metrics" }
                };
                
                return Ok(new CommonResponseDto<object>
                {
                    Success = true,
                    Data = healthCheck,
                    Message = "Business health check completed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business health check");
                return BadRequest(new CommonResponseDto<object>
                {
                    Success = false,
                    Message = $"Failed to get business health check: {ex.Message}"
                });
            }
        }
    }
} 
