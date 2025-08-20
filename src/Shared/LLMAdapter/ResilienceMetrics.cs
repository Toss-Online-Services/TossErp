using System.Diagnostics.Metrics;

namespace Shared.LLMAdapter;

/// <summary>
/// Metrics for LLM outbound resilience (retry, circuit breaker, outcomes)
/// </summary>
public static class ResilienceMetrics
{
    private static readonly Meter Meter = new("TossErp.LLM", "1.0.0");

    public static readonly Counter<long> OpenAiRequestsTotal = Meter.CreateCounter<long>("llm.openai.requests.total", description: "Total OpenAI requests attempted");
    public static readonly Counter<long> OpenAiRequestsSuccess = Meter.CreateCounter<long>("llm.openai.requests.success", description: "Successful OpenAI responses");
    public static readonly Counter<long> OpenAiRequestsFailed = Meter.CreateCounter<long>("llm.openai.requests.failed", description: "Failed OpenAI responses (non-success status)");
    public static readonly Counter<long> OpenAiRequestsTimeout = Meter.CreateCounter<long>("llm.openai.requests.timeout", description: "Timed out OpenAI requests");
    public static readonly Counter<long> OpenAiRetryAttempts = Meter.CreateCounter<long>("llm.openai.retries.attempts", description: "Total retry attempts for OpenAI requests");
    public static readonly Counter<long> OpenAiCircuitOpened = Meter.CreateCounter<long>("llm.openai.circuit.opened", description: "Number of times the OpenAI circuit breaker transitioned to Open");
}
