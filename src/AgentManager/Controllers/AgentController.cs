using Microsoft.AspNetCore.Mvc;

namespace AgentManager.Controllers;

[ApiController]
[Route("agents")]
public class AgentController : ControllerBase
{
    private static readonly Dictionary<string, object> _pending = new();

    [HttpPost("intent")]
    public IActionResult Intent([FromBody] object intent)
    {
        // For MVP return a suggested action stub
        var action = new
        {
            suggestedActions = new[] {
                new {
                    actionId = "a-" + Guid.NewGuid().ToString("N"),
                    type = "create_purchase_order",
                    payload = new { supplierId = "supplier-99", items = new[] { new { id = "item-456", qty = 20 } } },
                    confidence = 0.92,
                    explanation = "Autosuggest based on sales trend and current stock"
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
