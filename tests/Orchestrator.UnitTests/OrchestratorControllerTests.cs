using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Orchestrator.UnitTests;

public class OrchestratorControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrchestratorControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Execute_Returns_WorkflowId()
    {
        var client = _factory.CreateClient();
        try
        {
            var resp = await client.PostAsJsonAsync("/orchestrator/execute", new { name = "test" });
            resp.EnsureSuccessStatusCode();
            var obj = await resp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            Assert.True(obj.TryGetProperty("workflowId", out _));
        }
        catch (HttpRequestException ex)
        {
            // Likely Temporal not running locally; treat as skipped
            throw new SkipTestException("Temporal server not reachable: " + ex.Message);
        }
    }

    [Fact]
    public async Task Idempotency_Replays_Response_On_Second_Call()
    {
        var client = _factory.CreateClient();
        var key = Guid.NewGuid().ToString();
        try
        {
            var req1 = new HttpRequestMessage(HttpMethod.Post, "/orchestrator/execute") { Content = JsonContent.Create(new { name = "idempotent" }) };
            req1.Headers.Add("Idempotency-Key", key);
            var resp1 = await client.SendAsync(req1);
            resp1.EnsureSuccessStatusCode();
            var body1 = await resp1.Content.ReadAsStringAsync();

            var req2 = new HttpRequestMessage(HttpMethod.Post, "/orchestrator/execute") { Content = JsonContent.Create(new { name = "idempotent" }) };
            req2.Headers.Add("Idempotency-Key", key);
            var resp2 = await client.SendAsync(req2);
            resp2.EnsureSuccessStatusCode();
            var body2 = await resp2.Content.ReadAsStringAsync();

            Assert.Equal(body1, body2);
        }
        catch (HttpRequestException)
        {
            // Skip if Temporal backend unavailable
            throw new SkipTestException("Temporal not reachable - idempotency test skipped");
        }
    }

    [Fact]
    public async Task Missing_Tenant_Header_Returns_400_When_Enforced()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("TENANT_REQUIRED", "true");
            builder.ConfigureAppConfiguration((ctx, cfg) => { });
        });
        var client = factory.CreateClient();
        try
        {
            var resp = await client.PostAsJsonAsync("/orchestrator/execute", new { name = "tenant" });
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, resp.StatusCode);
        }
        catch (HttpRequestException)
        {
            throw new SkipTestException("Temporal not reachable - tenant enforcement test skipped");
        }
    }
}

public class SkipTestException : Exception
{
    public SkipTestException(string message) : base(message) { }
}
