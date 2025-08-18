# TOSS ERP Development Environment Setup Script
# This script sets up the local development environment

param(
    [switch]$SkipDocker = $false,
    [switch]$SkipBuild = $false,
    [switch]$SkipDatabase = $false
)

Write-Host "🚀 TOSS ERP Development Environment Setup" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green

# Check if Docker is running
if (-not $SkipDocker) {
    Write-Host "📦 Checking Docker..." -ForegroundColor Yellow
    try {
        docker version | Out-Null
        Write-Host "✅ Docker is running" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
        exit 1
    }

    # Start infrastructure services
    Write-Host "🏗️  Starting infrastructure services..." -ForegroundColor Yellow
    docker-compose up -d postgres redis rabbitmq
    
    # Wait for services to be healthy
    Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    # Check service health
    $maxRetries = 10
    $retry = 0
    
    do {
        $postgresHealthy = docker-compose ps postgres | Select-String "healthy"
        $redisHealthy = docker-compose ps redis | Select-String "healthy"
        $rabbitmqHealthy = docker-compose ps rabbitmq | Select-String "healthy"
        
        if ($postgresHealthy -and $redisHealthy -and $rabbitmqHealthy) {
            Write-Host "✅ All infrastructure services are healthy" -ForegroundColor Green
            break
        }
        
        $retry++
        if ($retry -ge $maxRetries) {
            Write-Host "❌ Services failed to become healthy within expected time" -ForegroundColor Red
            docker-compose logs postgres redis rabbitmq
            exit 1
        }
        
        Write-Host "⏳ Waiting for services... (attempt $retry/$maxRetries)" -ForegroundColor Yellow
        Start-Sleep -Seconds 10
    } while ($true)
}

# Restore .NET packages
if (-not $SkipBuild) {
    Write-Host "📦 Restoring .NET packages..." -ForegroundColor Yellow
    dotnet restore
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed to restore .NET packages" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ .NET packages restored" -ForegroundColor Green
}

# Run database migrations (when we have them)
if (-not $SkipDatabase) {
    Write-Host "🗄️  Setting up database..." -ForegroundColor Yellow
    # For now, the init-db.sql script handles the initial setup
    # In the future, we'll add EF Core migrations here
    Write-Host "✅ Database setup completed via init-db.sql" -ForegroundColor Green
}

# Install Node.js dependencies for web client
Write-Host "📦 Installing web client dependencies..." -ForegroundColor Yellow
if (Test-Path "src/clients/web/package.json") {
    Push-Location "src/clients/web"
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed to install web client dependencies" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    Pop-Location
    Write-Host "✅ Web client dependencies installed" -ForegroundColor Green
}

# Install Flutter dependencies for mobile client
Write-Host "📦 Installing mobile client dependencies..." -ForegroundColor Yellow
if (Test-Path "src/clients/mobile/pubspec.yaml") {
    Push-Location "src/clients/mobile"
    flutter pub get
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed to install mobile client dependencies" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    Pop-Location
    Write-Host "✅ Mobile client dependencies installed" -ForegroundColor Green
}

Write-Host ""
Write-Host "🎉 Development environment setup completed!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next steps:" -ForegroundColor Cyan
Write-Host "  1. Run 'docker-compose up -d' to start all services" -ForegroundColor White
Write-Host "  2. Visit http://localhost:5000 for the API Gateway" -ForegroundColor White
Write-Host "  3. Visit http://localhost:3000 for the Web Client" -ForegroundColor White
Write-Host "  4. Visit http://localhost:15672 for RabbitMQ Management (guest/guest)" -ForegroundColor White
Write-Host ""
Write-Host "🔧 Available services:" -ForegroundColor Cyan
Write-Host "  - PostgreSQL: localhost:5432 (postgres/postgres)" -ForegroundColor White
Write-Host "  - Redis: localhost:6379" -ForegroundColor White
Write-Host "  - RabbitMQ: localhost:5672 (AMQP) / localhost:15672 (Management)" -ForegroundColor White
Write-Host "  - API Gateway: localhost:5000" -ForegroundColor White
Write-Host "  - Web Client: localhost:3000" -ForegroundColor White
