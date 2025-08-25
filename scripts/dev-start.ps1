# TOSS ERP III - Development Environment Startup Script (Windows PowerShell)
# This script sets up and starts the complete development environment

$ErrorActionPreference = "Stop"

Write-Host "🚀 Starting TOSS ERP III Development Environment..." -ForegroundColor Green

# Check if Docker is installed
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker is not installed. Please install Docker first." -ForegroundColor Red
    Write-Host "   Download from: https://www.docker.com/get-started" -ForegroundColor Yellow
    exit 1
}

# Check if Docker Compose is installed
if (-not (Get-Command docker-compose -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker Compose is not installed. Please install Docker Compose first." -ForegroundColor Red
    exit 1
}

# Create .env file if it doesn't exist
if (-not (Test-Path .env)) {
    Write-Host "📝 Creating .env file..." -ForegroundColor Yellow
    
    # Generate random secrets
    $jwtSecret = [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes([System.Guid]::NewGuid().ToString()))
    $refreshSecret = [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes([System.Guid]::NewGuid().ToString()))
    
    $envContent = @"
# TOSS ERP III Development Environment Variables

# OpenAI API Key (required for AI features)
OPENAI_API_KEY=your-openai-api-key-here

# JWT Secrets (auto-generated for development)
JWT_SECRET=$jwtSecret
REFRESH_TOKEN_SECRET=$refreshSecret

# Email Configuration (using MailHog for development)
EMAIL_FROM=noreply@toss-erp.com
EMAIL_SMTP_HOST=mailhog
EMAIL_SMTP_PORT=1025

# Database Configuration
POSTGRES_USER=postgres
POSTGRES_PASSWORD=password
POSTGRES_DB=toss_erp

# Redis Configuration
REDIS_URL=redis://redis:6379

# Node Environment
NODE_ENV=development
"@
    
    $envContent | Out-File -FilePath .env -Encoding UTF8
    Write-Host "✅ Created .env file with default values" -ForegroundColor Green
    Write-Host "⚠️  Please update OPENAI_API_KEY in .env file for AI features to work" -ForegroundColor Yellow
}

# Create necessary directories
Write-Host "📁 Creating necessary directories..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path logs | Out-Null
New-Item -ItemType Directory -Force -Path data\postgres | Out-Null
New-Item -ItemType Directory -Force -Path data\redis | Out-Null

# Pull latest images
Write-Host "📦 Pulling Docker images..." -ForegroundColor Yellow
docker-compose -f docker-compose.dev.yml pull

# Build services
Write-Host "🔨 Building services..." -ForegroundColor Yellow
docker-compose -f docker-compose.dev.yml build

# Start the environment
Write-Host "🚀 Starting development environment..." -ForegroundColor Yellow
docker-compose -f docker-compose.dev.yml up -d

# Wait for services to be ready
Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Check service health
Write-Host "🔍 Checking service health..." -ForegroundColor Yellow

$services = @(
    @{name="postgres"; port=5432},
    @{name="redis"; port=6379},
    @{name="auth-service"; port=3001},
    @{name="api-gateway"; port=3000},
    @{name="frontend"; port=3100}
)

foreach ($service in $services) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$($service.port)/health" -TimeoutSec 5 -ErrorAction SilentlyContinue
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ $($service.name) is healthy" -ForegroundColor Green
        } else {
            Write-Host "⚠️  $($service.name) might not be ready yet" -ForegroundColor Yellow
        }
    } catch {
        try {
            $tcpClient = New-Object System.Net.Sockets.TcpClient
            $tcpClient.Connect("localhost", $service.port)
            $tcpClient.Close()
            Write-Host "✅ $($service.name) is reachable" -ForegroundColor Green
        } catch {
            Write-Host "⚠️  $($service.name) might not be ready yet" -ForegroundColor Yellow
        }
    }
}

Write-Host ""
Write-Host "🎉 TOSS ERP III Development Environment is starting up!" -ForegroundColor Green
Write-Host ""
Write-Host "📍 Service URLs:" -ForegroundColor Cyan
Write-Host "   🌐 Frontend (Nuxt):     http://localhost:3100" -ForegroundColor White
Write-Host "   🔌 API Gateway:         http://localhost:3000" -ForegroundColor White
Write-Host "   🔐 Auth Service:        http://localhost:3001" -ForegroundColor White
Write-Host "   📊 CRM Service:         http://localhost:3002" -ForegroundColor White
Write-Host "   📦 Inventory Service:   http://localhost:3003" -ForegroundColor White
Write-Host "   💰 Accounting Service:  http://localhost:3004" -ForegroundColor White
Write-Host "   💼 Sales Service:       http://localhost:3005" -ForegroundColor White
Write-Host "   🤖 AI Copilot Service:  http://localhost:3010" -ForegroundColor White
Write-Host ""
Write-Host "🛠️  Management Tools:" -ForegroundColor Cyan
Write-Host "   📧 MailHog (Email):     http://localhost:8025" -ForegroundColor White
Write-Host "   🗄️  Adminer (Database): http://localhost:8080" -ForegroundColor White
Write-Host ""
Write-Host "📚 API Documentation:" -ForegroundColor Cyan
Write-Host "   Auth API:               http://localhost:3001/docs" -ForegroundColor White
Write-Host "   CRM API:                http://localhost:3002/docs" -ForegroundColor White
Write-Host "   Inventory API:          http://localhost:3003/docs" -ForegroundColor White
Write-Host ""
Write-Host "💡 Useful Commands:" -ForegroundColor Cyan
Write-Host "   📋 View logs:           docker-compose -f docker-compose.dev.yml logs -f [service-name]" -ForegroundColor White
Write-Host "   🔄 Restart service:     docker-compose -f docker-compose.dev.yml restart [service-name]" -ForegroundColor White
Write-Host "   🛑 Stop all:            docker-compose -f docker-compose.dev.yml down" -ForegroundColor White
Write-Host "   🗑️  Clean up:           docker-compose -f docker-compose.dev.yml down -v" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  Notes:" -ForegroundColor Yellow
Write-Host "   - Update OPENAI_API_KEY in .env for AI features" -ForegroundColor White
Write-Host "   - Services may take a few minutes to be fully ready" -ForegroundColor White
Write-Host "   - Check logs if any service fails to start" -ForegroundColor White
Write-Host ""
Write-Host "Happy coding! 🚀" -ForegroundColor Green
