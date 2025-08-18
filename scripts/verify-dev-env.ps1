# TOSS ERP Development Environment Verification Script

Write-Host "🔍 TOSS ERP Development Environment Verification" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green

$allHealthy = $true

# Check Docker
Write-Host "`n📦 Checking Docker..." -ForegroundColor Yellow
try {
    docker version | Out-Null
    Write-Host "✅ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker is not running" -ForegroundColor Red
    $allHealthy = $false
}

# Check if containers are running
Write-Host "`n🐳 Checking Docker containers..." -ForegroundColor Yellow
$containers = @("postgres", "redis", "rabbitmq")

foreach ($container in $containers) {
    $status = docker-compose ps $container 2>$null
    if ($status -match "Up|healthy") {
        Write-Host "✅ $container is running" -ForegroundColor Green
    } else {
        Write-Host "❌ $container is not running" -ForegroundColor Red
        $allHealthy = $false
    }
}

# Check service connectivity
Write-Host "`n🔗 Checking service connectivity..." -ForegroundColor Yellow

# PostgreSQL
try {
    $env:PGPASSWORD = "postgres"
    $result = & psql -h localhost -U postgres -d toss_erp -c "SELECT 1;" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ PostgreSQL connection successful" -ForegroundColor Green
    } else {
        Write-Host "❌ PostgreSQL connection failed" -ForegroundColor Red
        $allHealthy = $false
    }
} catch {
    Write-Host "❌ PostgreSQL connection failed (psql not found)" -ForegroundColor Red
    $allHealthy = $false
}

# Redis
try {
    $result = & redis-cli -h localhost -p 6379 ping 2>$null
    if ($result -eq "PONG") {
        Write-Host "✅ Redis connection successful" -ForegroundColor Green
    } else {
        Write-Host "❌ Redis connection failed" -ForegroundColor Red
        $allHealthy = $false
    }
} catch {
    Write-Host "❌ Redis connection failed (redis-cli not found)" -ForegroundColor Red
    $allHealthy = $false
}

# RabbitMQ Management
try {
    $response = Invoke-WebRequest -Uri "http://localhost:15672" -Method GET -TimeoutSec 10 2>$null
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ RabbitMQ Management UI accessible" -ForegroundColor Green
    } else {
        Write-Host "❌ RabbitMQ Management UI not accessible" -ForegroundColor Red
        $allHealthy = $false
    }
} catch {
    Write-Host "❌ RabbitMQ Management UI not accessible" -ForegroundColor Red
    $allHealthy = $false
}

# Check .NET SDK
Write-Host "`n🏗️  Checking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET SDK not found" -ForegroundColor Red
    $allHealthy = $false
}

# Check Node.js (for web client)
Write-Host "`n📦 Checking Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js version: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Node.js not found" -ForegroundColor Red
    $allHealthy = $false
}

# Check Flutter (for mobile client)
Write-Host "`n📱 Checking Flutter..." -ForegroundColor Yellow
try {
    $flutterVersion = flutter --version | Select-String "Flutter" | Select-Object -First 1
    Write-Host "✅ $flutterVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Flutter not found" -ForegroundColor Red
    $allHealthy = $false
}

# Summary
Write-Host "`n📋 Verification Summary" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan

if ($allHealthy) {
    Write-Host "✅ All checks passed! Development environment is ready." -ForegroundColor Green
    Write-Host ""
    Write-Host "🚀 You can now:" -ForegroundColor Cyan
    Write-Host "  • Run services: docker-compose up -d" -ForegroundColor White
    Write-Host "  • Access API Gateway: http://localhost:5000" -ForegroundColor White
    Write-Host "  • Access Web Client: http://localhost:3000" -ForegroundColor White
    Write-Host "  • Access RabbitMQ Management: http://localhost:15672 (guest/guest)" -ForegroundColor White
    exit 0
} else {
    Write-Host "❌ Some checks failed. Please resolve the issues above." -ForegroundColor Red
    Write-Host ""
    Write-Host "💡 Common solutions:" -ForegroundColor Cyan
    Write-Host "  • Start Docker Desktop" -ForegroundColor White
    Write-Host "  • Run: docker-compose up -d" -ForegroundColor White
    Write-Host "  • Install missing tools (psql, redis-cli, .NET SDK, Node.js, Flutter)" -ForegroundColor White
    exit 1
}
