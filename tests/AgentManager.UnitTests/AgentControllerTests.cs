using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AgentManager.UnitTests;

public class AgentControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AgentControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Intent_Returns_SuggestedActions()
    {
        var client = _factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/agents/intent", new { text = "create PO" });
        resp.EnsureSuccessStatusCode();
        var obj = await resp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(obj.TryGetProperty("suggestedActions", out _));
    }

    [Fact]
    public async Task LlmHealth_Returns_Stub_When_No_ApiKey()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/agents/health/llm");
        resp.EnsureSuccessStatusCode();
        var obj = await resp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(obj.TryGetProperty("adapter", out var adapter));
        Assert.Contains("Stub", adapter.GetString());
    }

    [Fact]
    public async Task Idempotency_Replays_Response_On_Second_Call()
    {
        var client = _factory.CreateClient();
        var key = Guid.NewGuid().ToString();
        var request = new { text = "create PO" };
        var req1 = new HttpRequestMessage(HttpMethod.Post, "/agents/intent") { Content = JsonContent.Create(request) };
        req1.Headers.Add("Idempotency-Key", key);
        var resp1 = await client.SendAsync(req1);
        resp1.EnsureSuccessStatusCode();
        var body1 = await resp1.Content.ReadAsStringAsync();

        var req2 = new HttpRequestMessage(HttpMethod.Post, "/agents/intent") { Content = JsonContent.Create(request) };
        req2.Headers.Add("Idempotency-Key", key);
        var resp2 = await client.SendAsync(req2);
        resp2.EnsureSuccessStatusCode();
        var body2 = await resp2.Content.ReadAsStringAsync();

        Assert.Equal(body1, body2); // Replay should be identical
    }

    [Fact]
    public async Task Missing_Tenant_Header_Returns_400_When_Enforced()
    {
        // Rebuild factory with TENANT_REQUIRED=true
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("TENANT_REQUIRED", "true");
            builder.ConfigureAppConfiguration((ctx, cfg) => { });
        });
        var client = factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/agents/intent", new { text = "x" });
        // Expect 400 due to missing X-Tenant-Id
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, resp.StatusCode);
    }
}
