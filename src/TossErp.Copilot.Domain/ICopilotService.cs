namespace TossErp.Copilot.Domain;

public interface ICopilotService
{
    Task<string> GetCopilotResponseAsync(string prompt, CancellationToken cancellationToken = default);
    // Add other domain-level contracts as needed
} 
