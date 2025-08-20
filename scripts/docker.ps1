# TOSS ERP - Docker Management Script
# This script manages Docker operations for the TOSS ERP system

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("up", "down", "build", "logs", "clean", "dev", "prod", "infra", "services", "frontend")]
    [string]$Action,
    
    [switch]$Detached,
    [switch]$Force
)

Write-Host "🐳 TOSS ERP Docker Management Script" -ForegroundColor Green
Write-Host "Action: $Action" -ForegroundColor Yellow

# Set error action preference
$ErrorActionPreference = "Stop"

# Function to execute docker-compose command
function Invoke-DockerCompose {
    param([string]$Command, [string]$Description)
    
    Write-Host "`n📋 $Description" -ForegroundColor Cyan
    Write-Host "Executing: $Command" -ForegroundColor Gray
    
    Invoke-Expression $Command
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed: $Description" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "✅ Completed: $Description" -ForegroundColor Green
}

try {
    $detachedFlag = if ($Detached) { "-d" } else { "" }
    $forceFlag = if ($Force) { "--force" } else { "" }
    
    switch ($Action) {
        "up" {
            Invoke-DockerCompose -Command "docker-compose -f docker/docker-compose.yml up $detachedFlag" -Description "Starting all services"
        }
        
        "down" {
            Invoke-DockerCompose -Command "docker-compose -f docker/docker-compose.yml down $forceFlag" -Description "Stopping all services"
        }
        
        "build" {
            Invoke-DockerCompose -Command "docker-compose -f docker/docker-compose.yml build" -Description "Building all services"
        }
        
        "dev" {
            Invoke-DockerCompose -Command "docker-compose -f docker/docker-compose.yml -f docker/docker-compose.dev.yml up $detachedFlag" -Description "Starting development environment"
        }
        
        "prod" {
            Invoke-DockerCompose -Command "docker-compose -f docker/docker-compose.yml up $detachedFlag" -Description "Starting production environment"
        }
        
        "infra" {
            Invoke-DockerCompose -Command "docker-compose -f docker/docker-compose.yml up $detachedFlag postgres redis rabbitmq" -Description "Starting infrastructure services only"
        }
        
        "services" {
            Invoke-DockerCompose -Command "docker-compose -f docker/docker-compose.yml up $detachedFlag identity-api stock-api crm-api gateway" -Description "Starting backend services only"
        }
        
        "frontend" {
            Invoke-DockerCompose -Command "docker-compose -f docker/docker-compose.yml up $detachedFlag web-client nginx" -Description "Starting frontend services only"
        }
        
        "logs" {
            Invoke-DockerCompose -Command "docker-compose -f docker/docker-compose.yml logs -f" -Description "Showing logs for all services"
        }
        
        "clean" {
            Write-Host "🧹 Cleaning Docker resources..." -ForegroundColor Yellow
            docker system prune -f
            docker volume prune -f
            docker network prune -f
            Write-Host "✅ Docker cleanup completed" -ForegroundColor Green
        }
    }
    
    Write-Host "`n🎉 Docker operation completed successfully!" -ForegroundColor Green
    
} catch {
    Write-Host "`n💥 Docker operation failed with error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
