# TOSS ERP - Docker Compose Launch Script
# This script launches all services using Docker Compose

param(
    [switch]$FullStack,
    [switch]$DevOnly,
    [switch]$SkipBuild,
    [switch]$OpenBrowser
)

# Set console title
$Host.UI.RawUI.WindowTitle = "TOSS ERP - Docker Compose"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "    TOSS ERP - Docker Compose Launch" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Function to check if Docker is running
function Test-DockerRunning {
    try {
        docker info | Out-Null
        return $true
    } catch {
        return $false
    }
}

# Function to check if Docker Compose is available
function Test-DockerCompose {
    try {
        docker-compose --version | Out-Null
        return $true
    } catch {
        try {
            docker compose version | Out-Null
            return $true
        } catch {
            return $false
        }
    }
}

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Blue
Write-Host ""

if (-not (Test-DockerRunning)) {
    Write-Host "❌ Docker is not running. Please start Docker Desktop first." -ForegroundColor Red
    Write-Host "Download from: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
} else {
    Write-Host "✅ Docker is running" -ForegroundColor Green
}

if (-not (Test-DockerCompose)) {
    Write-Host "❌ Docker Compose not found. Please install Docker Compose." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
} else {
    Write-Host "✅ Docker Compose is available" -ForegroundColor Green
}

Write-Host ""
Write-Host "All prerequisites are satisfied!" -ForegroundColor Green
Write-Host ""

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir

# Determine which compose file to use
$composeFile = if ($FullStack) {
    "docker-compose.local.yml"
} elseif ($DevOnly) {
    "docker-compose.dev.yml"
} else {
    "docker-compose.dev.yml"
}

$composeFilePath = Join-Path $projectRoot $composeFile

if (-not (Test-Path $composeFilePath)) {
    Write-Host "❌ Docker Compose file not found: $composeFile" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Using Docker Compose file: $composeFile" -ForegroundColor Blue
Write-Host ""

# Function to check service health
function Test-ServiceHealth {
    param([string]$ServiceName, [string]$Url, [int]$MaxRetries = 30)
    
    Write-Host "Checking $ServiceName health..." -ForegroundColor Yellow
    
    for ($i = 1; $i -le $MaxRetries; $i++) {
        try {
            $response = Invoke-WebRequest -Uri $Url -TimeoutSec 5 -ErrorAction Stop
            if ($response.StatusCode -eq 200) {
                Write-Host "  ✅ $ServiceName is healthy" -ForegroundColor Green
                return $true
            }
        } catch {
            # Continue trying
        }
        
        Write-Host "  ⏳ Attempt $i/$MaxRetries - Waiting for $ServiceName..." -ForegroundColor Yellow
        Start-Sleep -Seconds 2
    }
    
    Write-Host "  ❌ $ServiceName failed to become healthy after $MaxRetries attempts" -ForegroundColor Red
    return $false
}

# Function to show service URLs
function Show-ServiceUrls {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "    Service URLs" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    
    if ($FullStack) {
        Write-Host "🌐 Main Application:" -ForegroundColor White
        Write-Host "  http://localhost (Nginx Reverse Proxy)" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "📱 Client Applications:" -ForegroundColor White
        Write-Host "  Mobile Client:  http://localhost/mobile/" -ForegroundColor Yellow
        Write-Host "  Web Client:     http://localhost/web/" -ForegroundColor Yellow
        Write-Host "  Admin Client:   http://localhost/admin/" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "🔌 Backend Services:" -ForegroundColor White
        Write-Host "  API Gateway:    http://localhost/api/" -ForegroundColor Yellow
        Write-Host "  Stock API:      http://localhost/stock/" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "📊 Monitoring:" -ForegroundColor White
        Write-Host "  Prometheus:     http://localhost/monitoring/prometheus/" -ForegroundColor Yellow
        Write-Host "  Grafana:        http://localhost/monitoring/grafana/" -ForegroundColor Yellow
        Write-Host "  RabbitMQ:       http://localhost/rabbitmq/" -ForegroundColor Yellow
    } else {
        Write-Host "🔌 Backend Services:" -ForegroundColor White
        Write-Host "  API Gateway:    http://localhost:8080" -ForegroundColor Yellow
        Write-Host "  Stock API:      http://localhost:5001" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "🗄️  Infrastructure:" -ForegroundColor White
        Write-Host "  PostgreSQL:     localhost:5432" -ForegroundColor Yellow
        Write-Host "  Redis:          localhost:6379" -ForegroundColor Yellow
        Write-Host "  RabbitMQ:       http://localhost:15672" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "🔑 Default Credentials:" -ForegroundColor White
    Write-Host "  PostgreSQL: postgres/postgres123" -ForegroundColor Yellow
    Write-Host "  RabbitMQ:   guest/guest" -ForegroundColor Yellow
    if ($FullStack) {
        Write-Host "  Monitoring: admin/admin123" -ForegroundColor Yellow
    }
}

# Launch services
try {
    Write-Host "Starting Docker Compose services..." -ForegroundColor Blue
    Write-Host ""
    
    # Change to project root directory
    Set-Location $projectRoot
    
    # Build and start services
    if ($SkipBuild) {
        Write-Host "Starting services (skipping build)..." -ForegroundColor Green
        docker-compose -f $composeFile up -d
    } else {
        Write-Host "Building and starting services..." -ForegroundColor Green
        docker-compose -f $composeFile up -d --build
    }
    
    if ($LASTEXITCODE -ne 0) {
        throw "Docker Compose failed to start services"
    }
    
    Write-Host ""
    Write-Host "✅ Services started successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Wait for services to be ready
    Write-Host "Waiting for services to be ready..." -ForegroundColor Blue
    Start-Sleep -Seconds 10
    
    # Check service health
    Write-Host ""
    Write-Host "Checking service health..." -ForegroundColor Blue
    
    $allHealthy = $true
    
    if ($FullStack) {
        $allHealthy = $allHealthy -and (Test-ServiceHealth "Gateway" "http://localhost/api/health")
        $allHealthy = $allHealthy -and (Test-ServiceHealth "Mobile Client" "http://localhost/mobile/")
        $allHealthy = $allHealthy -and (Test-ServiceHealth "Web Client" "http://localhost/web/")
        $allHealthy = $allHealthy -and (Test-ServiceHealth "Admin Client" "http://localhost/admin/")
    } else {
        $allHealthy = $allHealthy -and (Test-ServiceHealth "Gateway" "http://localhost:8080/health")
        $allHealthy = $allHealthy -and (Test-ServiceHealth "Stock API" "http://localhost:5001/health")
    }
    
    # Show service URLs
    Show-ServiceUrls
    
    if ($allHealthy) {
        Write-Host ""
        Write-Host "🎉 All services are healthy and ready!" -ForegroundColor Green
        
        if ($OpenBrowser) {
            Write-Host ""
            Write-Host "Opening services in browser..." -ForegroundColor Blue
            
            if ($FullStack) {
                Start-Process "http://localhost"
                Start-Process "http://localhost/api/health"
            } else {
                Start-Process "http://localhost:8080/health"
                Start-Process "http://localhost:5001/health"
                Start-Process "http://localhost:15672"
            }
        } else {
            Write-Host ""
            Write-Host "Press Enter to open services in browser..." -ForegroundColor Cyan
            Read-Host
            Write-Host "Opening services in browser..." -ForegroundColor Blue
            
            if ($FullStack) {
                Start-Process "http://localhost"
                Start-Process "http://localhost/api/health"
            } else {
                Start-Process "http://localhost:8080/health"
                Start-Process "http://localhost:5001/health"
                Start-Process "http://localhost:15672"
            }
        }
    } else {
        Write-Host ""
        Write-Host "⚠️  Some services may not be fully ready yet." -ForegroundColor Yellow
        Write-Host "Check the Docker Compose logs for more details:" -ForegroundColor White
        Write-Host "  docker-compose -f $composeFile logs" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "    Management Commands" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "📊 View logs:" -ForegroundColor White
    Write-Host "  docker-compose -f $composeFile logs -f" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "🔄 Restart services:" -ForegroundColor White
    Write-Host "  docker-compose -f $composeFile restart" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "🛑 Stop services:" -ForegroundColor White
    Write-Host "  docker-compose -f $composeFile down" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "🧹 Clean up (removes volumes):" -ForegroundColor White
    Write-Host "  docker-compose -f $composeFile down -v" -ForegroundColor Yellow
    
} catch {
    Write-Host ""
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Check if Docker Desktop is running" -ForegroundColor White
    Write-Host "2. Check available ports (8080, 5001, 5432, 6379, 15672)" -ForegroundColor White
    Write-Host "3. View logs: docker-compose -f $composeFile logs" -ForegroundColor White
    Write-Host "4. Ensure you have sufficient disk space and memory" -ForegroundColor White
}

Write-Host ""
Write-Host "Press Enter to exit..."
Read-Host
