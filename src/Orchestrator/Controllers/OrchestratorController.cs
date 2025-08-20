using Microsoft.AspNetCore.Mvc;

namespace Orchestrator.Controllers;

[ApiController]
[Route("orchestrator")]
public class OrchestratorController : ControllerBase
{
    private static readonly Dictionary<string, object> _store = new();

    [HttpPost("execute")]
    public IActionResult Execute([FromBody] object request)
    {
        var id = "wf_" + Guid.NewGuid().ToString("N");
        var entry = new { WorkflowId = id, Status = "started", Request = request, StartedAt = DateTime.UtcNow };
        _store[id] = entry;
        return Ok(new { workflowId = id, status = "started", startedAt = DateTime.UtcNow });
    }

    [HttpGet("workflow/{id}")]
    public IActionResult Get(string id)
    {
        if (_store.TryGetValue(id, out var entry))
            return Ok(entry);
        return NotFound();
    }
}
