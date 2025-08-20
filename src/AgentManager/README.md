AgentManager (MVP)

This is a minimal AgentManager WebAPI used for local development and demo purposes.

Quick start

```powershell
cd src/AgentManager
dotnet run --urls "http://localhost:5200"
```

End points
- POST http://localhost:5200/agents/intent
- POST http://localhost:5200/agents/authorize-action

Notes
- This project registers a stub LLM adapter (`Shared.LLMAdapter.StubLLMAdapter`) by default. Replace with a real adapter and configuration when integrating a provider.
- After running both services, you can export OpenAPI files with `scripts\export-swagger.ps1`.
