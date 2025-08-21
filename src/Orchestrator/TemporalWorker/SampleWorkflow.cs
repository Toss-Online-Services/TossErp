using Temporalio.Workflows;
using Microsoft.Extensions.Logging;

namespace Orchestrator.TemporalWorker;

[Workflow]
public class SampleWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(string input)
    {
        var logger = Workflow.Logger;
        logger.LogInformation("SampleWorkflow started with input: {Input}", input);

        // Simple workflow that processes input and returns result
        var processedInput = await Workflow.ExecuteActivityAsync(
            (SampleActivities activities) => activities.ProcessInputAsync(input),
            new()
            {
                ScheduleToCloseTimeout = TimeSpan.FromMinutes(1),
                RetryPolicy = new()
                {
                    MaximumAttempts = 3,
                    InitialInterval = TimeSpan.FromSeconds(1),
                    MaximumInterval = TimeSpan.FromSeconds(10),
                    BackoffCoefficient = 2.0
                }
            });

        logger.LogInformation("SampleWorkflow completed with result: {Result}", processedInput);
        return processedInput;
    }
}
