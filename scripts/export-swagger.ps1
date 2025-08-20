param(
    $OrchestratorUrl = 'http://localhost:5100',
    $AgentManagerUrl = 'http://localhost:5200'
)

Invoke-RestMethod -Uri "$OrchestratorUrl/swagger/v1/swagger.json" -OutFile "docs\openapi\orchestrator-swagger.json" -ErrorAction SilentlyContinue
Invoke-RestMethod -Uri "$AgentManagerUrl/swagger/v1/swagger.json" -OutFile "docs\openapi\agentmanager-swagger.json" -ErrorAction SilentlyContinue
Write-Output "Exported swagger files to docs/openapi (if services were running)." 
