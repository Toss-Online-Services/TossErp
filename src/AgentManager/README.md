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
