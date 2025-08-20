Param(
    [string]$OrchestratorSwagger = "http://localhost:5002/swagger/v1/swagger.json",
    [string]$AgentManagerSwagger = "http://localhost:5001/swagger/v1/swagger.json"
)

Write-Host "Generating API clients (requires services running with Swagger enabled)..." -ForegroundColor Cyan

if (-not (Get-Command nswag -ErrorAction SilentlyContinue)) {
    Write-Host "NSwag CLI not found. Installing global tool..." -ForegroundColor Yellow
    dotnet tool install --global NSwag.ConsoleCore | Out-Null
}

$root = Split-Path $MyInvocation.MyCommand.Path -Parent
$clientRoot = Join-Path $root "..\src\clients"
Push-Location $clientRoot

# Ensure output folder exists
$generated = Join-Path $clientRoot "Generated"
if (-not (Test-Path $generated)) { New-Item -ItemType Directory -Path $generated | Out-Null }

Write-Host "Updating Orchestrator config URL -> $OrchestratorSwagger" -ForegroundColor Gray
(Get-Content nswag.orchestrator.json) | ForEach-Object { $_ -replace 'http://localhost:5002/swagger/v1/swagger.json', $OrchestratorSwagger } | Set-Content nswag.orchestrator.tmp.json
nswag run nswag.orchestrator.tmp.json | Write-Host

Write-Host "Updating AgentManager config URL -> $AgentManagerSwagger" -ForegroundColor Gray
(Get-Content nswag.agentmanager.json) | ForEach-Object { $_ -replace 'http://localhost:5001/swagger/v1/swagger.json', $AgentManagerSwagger } | Set-Content nswag.agentmanager.tmp.json
nswag run nswag.agentmanager.tmp.json | Write-Host

Remove-Item nswag.orchestrator.tmp.json, nswag.agentmanager.tmp.json -ErrorAction SilentlyContinue

Write-Host "Client generation complete. Output in src/clients/Generated" -ForegroundColor Green
Pop-Location
