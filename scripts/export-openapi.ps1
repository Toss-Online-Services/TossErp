param(
    [string]$Configuration = "Debug"
)
# Generates OpenAPI json for AgentManager and Orchestrator using Swashbuckle CLI
# Requires: dotnet tool install --global Swashbuckle.AspNetCore.Cli

$ErrorActionPreference = 'Stop'

$root = Split-Path $PSScriptRoot -Parent
$docsDir = Join-Path $root 'docs/openapi'
if (!(Test-Path $docsDir)) { New-Item -ItemType Directory -Path $docsDir | Out-Null }

function Invoke-SwaggerExport {
    param(
        [string]$ProjectPath,
        [string]$OutputName
    )
    Write-Host "Building $ProjectPath"
    dotnet build $ProjectPath -c $Configuration --no-incremental | Out-Null
    $dll = Get-ChildItem (Join-Path $ProjectPath "bin/$Configuration") -Recurse -Filter *.dll | Where-Object { $_.Name -notmatch 'ref|test' } | Select-Object -First 1
    if (!$dll) { throw "Unable to find built dll for $ProjectPath" }
    $outFile = Join-Path $docsDir $OutputName
    Write-Host "Exporting swagger to $outFile"
    dotnet swagger tofile --output $outFile $dll.FullName v1
}

Invoke-SwaggerExport -ProjectPath "$root/src/AgentManager" -OutputName 'agentmanager-v1.json'
Invoke-SwaggerExport -ProjectPath "$root/src/Orchestrator" -OutputName 'orchestrator-v1.json'
Write-Host "OpenAPI export complete -> $docsDir"
