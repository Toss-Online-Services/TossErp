Orchestrator (MVP)

This is a minimal Orchestrator WebAPI used for local development and demo purposes.

Quick start

```powershell
cd src/Orchestrator
dotnet run --urls "http://localhost:5100"
```

End points
- POST http://localhost:5100/orchestrator/execute
- GET  http://localhost:5100/orchestrator/workflow/{id}
