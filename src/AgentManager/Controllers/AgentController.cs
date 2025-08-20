using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AgentManager.Controllers;

[ApiController]
[Route("agents")]
public class AgentController : ControllerBase
{
    private static readonly Dictionary<string, object> _pending = new();
    private readonly Shared.LLMAdapter.ILLMAdapter _llm;

    public AgentController(Shared.LLMAdapter.ILLMAdapter llm)
    {
        _llm = llm;
    }

    [HttpGet("health/llm")]
    public IActionResult LlmHealth()
    {
        var impl = _llm.GetType().Name;
        return Ok(new { adapter = impl });
    }

    [HttpPost("intent")]
    public async Task<IActionResult> Intent([FromBody] object intent)
    {
        // Use LLM adapter (stub or real) to produce explanation & confidence
        var prompt = "Suggest action for intent: " + intent?.ToString();
        var llmResp = await _llm.CompleteAsync(prompt);

        var action = new
        {
            suggestedActions = new[]
            {
                new
                {
                    actionId = "a-" + Guid.NewGuid().ToString("N"),
                    type = "create_purchase_order",
                    payload = new { supplierId = "supplier-99", items = new[] { new { id = "item-456", qty = 20 } } },
                    confidence = llmResp.Confidence,
                    explanation = llmResp.Text
                }
            },
            requiresApproval = true
        };

        var id = "p_" + Guid.NewGuid().ToString("N");
        _pending[id] = action;
        return Ok(action);
    }

    [HttpPost("authorize-action")]
    public IActionResult Authorize([FromBody] object req)
    {
        // Accept and return simple ack
        return Ok(new { status = "ok" });
    }
}
