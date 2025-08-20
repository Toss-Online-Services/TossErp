# Run Orchestrator and AgentManager locally (PowerShell)

Push-Location $PSScriptRoot\..\src\Orchestrator; dotnet run --urls "http://localhost:5100"; Pop-Location
Start-Sleep -Milliseconds 200
Push-Location $PSScriptRoot\..\src\AgentManager; dotnet run --urls "http://localhost:5200"; Pop-Location
