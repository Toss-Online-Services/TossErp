using System.Threading;

namespace Shared.LLMAdapter;

public class StubLLMAdapter : ILLMAdapter
{
    public Task<LLMResponse> CompleteAsync(string prompt, CancellationToken ct = default)
    {
        var text = "[stub] simulated LLM response for prompt: " + prompt;
        return Task.FromResult(new LLMResponse(text, 0.5));
    }
}
