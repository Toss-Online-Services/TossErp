using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TossErp.Infrastructure.Services.AI;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CopilotController : ControllerBase
{
    private readonly ICopilotService _copilotService;
    private readonly ILogger<CopilotController> _logger;

    public CopilotController(ICopilotService copilotService, ILogger<CopilotController> logger)
    {
        _copilotService = copilotService;
        _logger = logger;
    }

    /// <summary>
    /// Process natural language query
    /// </summary>
    /// <param name="request">Query request with natural language question</param>
    /// <returns>AI-generated response with suggested actions</returns>
    [HttpPost("query")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CopilotResponse>> ProcessQuery([FromBody] CopilotQueryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest(new { error = "Query cannot be empty" });

        try
        {
            var response = await _copilotService.ProcessQuery(
                request.Query,
                request.Context,
                request.Language
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing copilot query: {Query}", request.Query);
            return StatusCode(500, new { error = "Failed to process query" });
        }
    }

    /// <summary>
    /// Get intelligent recommendations
    /// </summary>
    /// <param name="type">Type of recommendation requested</param>
    /// <returns>List of actionable recommendations</returns>
    [HttpGet("recommendations/{type}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Recommendation>>> GetRecommendations(
        RecommendationType type,
        [FromQuery] Dictionary<string, object>? parameters = null)
    {
        try
        {
            var recommendations = await _copilotService.GetRecommendations(type, parameters);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations of type {Type}", type);
            return StatusCode(500, new { error = "Failed to get recommendations" });
        }
    }

    /// <summary>
    /// Get predictive analytics
    /// </summary>
    /// <param name="type">Type of prediction requested</param>
    /// <returns>Prediction result with confidence and data</returns>
    [HttpGet("predictions/{type}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PredictionResult>> GetPrediction(
        PredictionType type,
        [FromQuery] Dictionary<string, object>? parameters = null)
    {
        try
        {
            var prediction = await _copilotService.GetPrediction(type, parameters);
            return Ok(prediction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prediction of type {Type}", type);
            return StatusCode(500, new { error = "Failed to get prediction" });
        }
    }

    /// <summary>
    /// Get available AI capabilities
    /// </summary>
    [HttpGet("capabilities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetCapabilities()
    {
        var capabilities = new
        {
            nlp = new
            {
                enabled = true,
                languages = new[] { "en", "zu", "af", "xh" },
                features = new[] { "query_processing", "contextual_understanding", "multi_turn_conversation" }
            },
            recommendations = new
            {
                enabled = true,
                types = Enum.GetNames(typeof(RecommendationType))
            },
            predictions = new
            {
                enabled = true,
                types = Enum.GetNames(typeof(PredictionType))
            },
            version = "1.0.0"
        };

        return Ok(capabilities);
    }
}

// Request DTOs
public record CopilotQueryRequest(
    string Query,
    string? Context = null,
    string? Language = "en"
);

