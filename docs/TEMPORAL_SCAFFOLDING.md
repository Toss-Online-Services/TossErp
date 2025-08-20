# Temporal scaffolding (notes)

This file documents how to add Temporal support locally and a minimal worker scaffold.

- Add Temporal .NET SDK package: `Temporalio` (check latest package name/version).
- Create a Worker project that registers workflows and activities.
- Provide docker-compose entries for Temporal server (Frontend, History, Matching, Cassandra/postgres) or use Temporal Cloud.
- Keep sample worker minimal: one workflow that waits and returns a value; wire configuration via appsettings.

Local dev quick-start (manual):
1. Start Temporal server via docker-compose (not included here).
2. Run Worker project.
3. From Orchestrator, call the Temporal client to start a workflow by name.

This doc is a scaffold; I can add a sample Worker project if you want.
