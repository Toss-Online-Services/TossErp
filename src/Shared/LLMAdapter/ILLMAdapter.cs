namespace Shared.LLMAdapter;

public record LLMResponse(string Text, double Confidence);

public interface ILLMAdapter
{
    Task<LLMResponse> CompleteAsync(string prompt, CancellationToken ct = default);
}
