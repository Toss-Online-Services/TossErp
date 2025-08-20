using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Integration.Tests;

public class OrchestratorAgentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrchestratorAgentIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task IntentToWorkflow_Flow()
    {
        var client = _factory.CreateClient();

        // Call intent
        var intentResp = await client.PostAsJsonAsync("/agents/intent", new { text = "auto reorder" });
        intentResp.EnsureSuccessStatusCode();
        var intentObj = await intentResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(intentObj.TryGetProperty("suggestedActions", out var actions));

        // Start workflow
        var execResp = await client.PostAsJsonAsync("/orchestrator/execute", new { source = "agent", action = actions[0] });
        execResp.EnsureSuccessStatusCode();
        var execObj = await execResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(execObj.TryGetProperty("workflowId", out _));
    }
}
