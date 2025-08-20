using Temporalio.Activities;
using Temporalio.Workflows;

namespace TemporalWorker;

[Workflow]
public class SampleWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(string input)
    {
        // Simulate calling an activity (Echo)
        return await Workflow.ExecuteActivityAsync(
            () => Activities.Echo(input),
            new ActivityOptions { ScheduleToCloseTimeout = TimeSpan.FromSeconds(10) });
    }
}
