using Temporalio.Activities;
using Microsoft.Extensions.Logging;

namespace Orchestrator.TemporalWorker;

public class SampleActivities
{
    private readonly ILogger<SampleActivities> _logger;

    public SampleActivities(ILogger<SampleActivities> logger)
    {
        _logger = logger;
    }

    [Activity]
    public async Task<string> ProcessInputAsync(string input)
    {
        _logger.LogInformation("Processing input: {Input}", input);
        
        // Simulate some processing work
        await Task.Delay(TimeSpan.FromSeconds(1));
        
        var result = $"Processed: {input} at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
        
        _logger.LogInformation("Processing completed with result: {Result}", result);
        return result;
    }
}
