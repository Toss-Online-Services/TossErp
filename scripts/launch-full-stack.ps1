# TOSS ERP - Full Stack Launch Script
# This script launches the complete TOSS ERP system with all client applications

param(
    [switch]$SkipBuild,
    [switch]$OpenBrowser,
    [switch]$ShowLogs
)

# Set console title
$Host.UI.RawUI.WindowTitle = "TOSS ERP - Full Stack Launch"

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "    TOSS ERP - Full Stack Launch" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
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
        return $false
    }
}

# Check Docker status
Write-Host "🔍 Checking Docker status..." -ForegroundColor Yellow
if (-not (Test-DockerRunning)) {
    Write-Host "❌ Docker is not running. Please start Docker Desktop first." -ForegroundColor Red
    Write-Host "   Download from: https://www.docker.com/products/docker-desktop" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
} else {
    Write-Host "✅ Docker is running" -ForegroundColor Green
}

# Check Docker Compose
Write-Host "🔍 Checking Docker Compose..." -ForegroundColor Yellow
if (-not (Test-DockerCompose)) {
    Write-Host "❌ Docker Compose not found. Please install Docker Compose." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
} else {
    Write-Host "✅ Docker Compose is available" -ForegroundColor Green
}

Write-Host ""
Write-Host "🚀 Starting TOSS ERP Full Stack..." -ForegroundColor Green

# Stop any existing containers
Write-Host "🛑 Stopping existing containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.full-stack.yml down 2>$null

# Launch the full stack
if ($SkipBuild) {
    Write-Host "📦 Launching services (skipping build)..." -ForegroundColor Yellow
    docker-compose -f docker-compose.full-stack.yml up -d
} else {
    Write-Host "🔨 Building and launching services..." -ForegroundColor Yellow
    docker-compose -f docker-compose.full-stack.yml up -d --build
}

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "🎉 TOSS ERP Full Stack launched successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Wait for services to be ready
    Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    # Show service status
    Write-Host "📊 Service Status:" -ForegroundColor Cyan
    docker-compose -f docker-compose.full-stack.yml ps
    
    Write-Host ""
    Write-Host "🌐 Access Points:" -ForegroundColor Cyan
    Write-Host "   Main Portal:     http://localhost/" -ForegroundColor White
    Write-Host "   Mobile Client:   http://localhost:3000/" -ForegroundColor White
    Write-Host "   Web Client:      http://localhost:3001/" -ForegroundColor White
    Write-Host "   API Gateway:     http://localhost:8080/" -ForegroundColor White
    Write-Host "   RabbitMQ:        http://localhost:15672/" -ForegroundColor White
    Write-Host "   PostgreSQL:      localhost:5432" -ForegroundColor White
    Write-Host "   Redis:           localhost:6379" -ForegroundColor White
    
    Write-Host ""
    Write-Host "📱 Web Client includes comprehensive admin functionality" -ForegroundColor Green
    
    # Open browser if requested
    if ($OpenBrowser) {
        Write-Host "🌐 Opening main portal in browser..." -ForegroundColor Yellow
        Start-Process "http://localhost"
    }
    
    # Show logs if requested
    if ($ShowLogs) {
        Write-Host "📋 Showing service logs..." -ForegroundColor Yellow
        docker-compose -f docker-compose.full-stack.yml logs -f
    } else {
        Write-Host ""
        Write-Host "💡 Use 'docker-compose -f docker-compose.full-stack.yml logs -f' to view logs" -ForegroundColor Gray
        Write-Host "💡 Use 'docker-compose -f docker-compose.full-stack.yml down' to stop services" -ForegroundColor Gray
    }
    
} else {
    Write-Host "❌ Failed to launch TOSS ERP Full Stack" -ForegroundColor Red
    Write-Host "Check the logs above for errors" -ForegroundColor Red
    exit 1
}
