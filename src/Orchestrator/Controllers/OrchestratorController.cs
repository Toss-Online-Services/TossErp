using Microsoft.AspNetCore.Mvc;
using Temporalio.Client;

namespace Orchestrator.Controllers;

[ApiController]
[Route("orchestrator")]
public class OrchestratorController : ControllerBase
{
    private readonly Task<TemporalClient> _clientTask;

    public OrchestratorController(Task<TemporalClient> clientTask)
    {
        _clientTask = clientTask;
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute([FromBody] ExecuteRequest request)
    {
        var client = await _clientTask; // lazy connect
        var workflowId = request.WorkflowId ?? ($"sample-" + Guid.NewGuid().ToString("N"));
        var handle = await client.StartWorkflowAsync((TemporalWorker.SampleWorkflow wf) => wf.RunAsync(request.Input ?? ""),
            new(id: workflowId, taskQueue: "orchestrator-tq"));
        return Ok(new { workflowId = handle.Id, runId = handle.RunId, status = "started" });
    }

    [HttpGet("workflow/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var client = await _clientTask;
        try
        {
            var handle = client.GetWorkflowHandle(id);
            var desc = await handle.DescribeAsync();
            return Ok(new { workflowId = id, status = desc.Status.ToString(), runId = desc.RunId });
        }
        catch (WorkflowNotFoundException)
        {
            return NotFound();
        }
    }
}

public record ExecuteRequest(string? WorkflowId, string? Input);
