using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using System.Net;
using System.Net.Http;
using Xunit;

namespace AgentManager.Resilience.UnitTests;

public class RetryPipelineTests
{
    [Fact]
    public async Task Retries_On_Transient_Then_Succeeds()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClient("openai", c =>
        {
            c.BaseAddress = new Uri("https://api.openai.com/");
            c.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddResilienceHandler("openai-pipeline", (builderCtx, context) =>
        {
            builderCtx.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(10),
                ShouldHandle = args =>
                {
                    if (args.Outcome.Result is HttpResponseMessage r && (int)r.StatusCode >= 500) return ValueTask.FromResult(true);
                    return ValueTask.FromResult(false);
                }
            });
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SequenceHandler(
            new HttpResponseMessage(HttpStatusCode.InternalServerError),
            new HttpResponseMessage(HttpStatusCode.InternalServerError),
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        ));

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("openai");
        var response = await client.PostAsync("/v1/chat/completions", new StringContent(""));

        Assert.True(response.IsSuccessStatusCode);
        // We expect 3 calls total (initial + 2 retries). SequenceHandler tracks calls.
        var handler = (SequenceHandler)SequenceHandler.LastInstance!;
        Assert.Equal(3, handler.CallCount);
    }

    [Fact]
    public async Task Does_Not_Retry_On_Client_Error()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClient("openai", c => c.BaseAddress = new Uri("https://api.openai.com/"))
            .AddResilienceHandler("openai-pipeline", (builderCtx, context) =>
            {
                builderCtx.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(10),
                    ShouldHandle = args =>
                    {
                        if (args.Outcome.Result is HttpResponseMessage r && (int)r.StatusCode >= 500) return ValueTask.FromResult(true);
                        return ValueTask.FromResult(false);
                    }
                });
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SequenceHandler(new HttpResponseMessage(HttpStatusCode.BadRequest)));

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("openai");
        var response = await client.PostAsync("/v1/chat/completions", new StringContent(""));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var handler = (SequenceHandler)SequenceHandler.LastInstance!;
        Assert.Equal(1, handler.CallCount);
    }

    private class SequenceHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses;
        public int CallCount { get; private set; }
        public static SequenceHandler? LastInstance { get; private set; }

        public SequenceHandler(params HttpResponseMessage[] responses)
        {
            _responses = new Queue<HttpResponseMessage>(responses);
            LastInstance = this;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            if (_responses.Count == 0)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
            return Task.FromResult(_responses.Dequeue());
        }
    }
}
