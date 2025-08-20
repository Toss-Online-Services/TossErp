using Temporalio.Worker;
using Temporalio.Client;
using TemporalWorker;

// Basic Temporal worker host. Expects a Temporal server at localhost:7233 (default docker-compose.temporal.yml assumption).
// Environment overrides:
//   TEMPORAL_TARGET (host:port)
//   TEMPORAL_NAMESPACE (default) - usually "default"

var target = Environment.GetEnvironmentVariable("TEMPORAL_TARGET") ?? "localhost:7233";
var ns = Environment.GetEnvironmentVariable("TEMPORAL_NAMESPACE") ?? "default";

Console.WriteLine($"Starting Temporal worker (target={target}, namespace={ns})...");

var client = await TemporalClient.ConnectAsync(new TemporalClientConnectOptions(target) { Namespace = ns });

// Register workflows & activities
var worker = new TemporalWorker(client, new TemporalWorkerOptions(taskQueue: "orchestrator-tq")
	.AddWorkflow<SampleWorkflow>()
	.AddAllActivities(typeof(Activities))
);

await worker.ExecuteAsync();
