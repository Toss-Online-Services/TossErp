# TOSS E2E Test Runner Script
# This script starts backend, frontend, and runs E2E tests

Write-Host "`n🚀 TOSS End-to-End Test Runner" -ForegroundColor Cyan
Write-Host "================================`n" -ForegroundColor Cyan

$ErrorActionPreference = "Continue"

# Configuration
$BACKEND_DIR = "C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss"
$FRONTEND_DIR = "C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\toss-web"
$BACKEND_URL = "https://localhost:5001"
$FRONTEND_URL = "http://localhost:3001"
$MAX_WAIT = 60 # seconds

# Function to check if a URL is responding
function Test-UrlReady {
    param($url)
    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
        return $true
    } catch {
        return $false
    }
}

# Function to wait for service
function Wait-ForService {
    param($url, $name, $maxWait)
    
    Write-Host "⏳ Waiting for $name to start..." -ForegroundColor Yellow
    $waited = 0
    
    while ($waited -lt $maxWait) {
        if (Test-UrlReady $url) {
            Write-Host "✅ $name is ready!" -ForegroundColor Green
            return $true
        }
        Start-Sleep -Seconds 2
        $waited += 2
        Write-Host "   Still waiting... ($waited/$maxWait seconds)" -ForegroundColor Gray
    }
    
    Write-Host "❌ $name failed to start within $maxWait seconds" -ForegroundColor Red
    return $false
}

# Step 1: Check if servers are already running
Write-Host "`n📡 Checking server status..." -ForegroundColor Cyan

$backendRunning = Test-UrlReady "$BACKEND_URL/health"
$frontendRunning = Test-UrlReady $FRONTEND_URL

if ($backendRunning) {
    Write-Host "✅ Backend is already running" -ForegroundColor Green
} else {
    Write-Host "⚠️  Backend is not running" -ForegroundColor Yellow
    Write-Host "   Starting backend..." -ForegroundColor Gray
    
    # Start backend in background
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$BACKEND_DIR'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --project src/Web/Web.csproj" -WindowStyle Minimized
    
    # Wait for backend
    if (-not (Wait-ForService "$BACKEND_URL/health" "Backend" $MAX_WAIT)) {
        Write-Host "❌ Cannot proceed without backend" -ForegroundColor Red
        exit 1
    }
}

if ($frontendRunning) {
    Write-Host "✅ Frontend is already running" -ForegroundColor Green
} else {
    Write-Host "⚠️  Frontend is not running" -ForegroundColor Yellow
    Write-Host "   Starting frontend..." -ForegroundColor Gray
    
    # Start frontend in background
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$FRONTEND_DIR'; npm run dev" -WindowStyle Minimized
    
    # Wait for frontend
    if (-not (Wait-ForService $FRONTEND_URL "Frontend" $MAX_WAIT)) {
        Write-Host "❌ Cannot proceed without frontend" -ForegroundColor Red
        exit 1
    }
}

# Step 2: Run E2E tests
Write-Host "`n🧪 Running E2E Tests..." -ForegroundColor Cyan
Write-Host "================================`n" -ForegroundColor Cyan

cd $FRONTEND_DIR

# Run tests
Write-Host "Executing: npx playwright test tests/e2e/toss-complete-flow.e2e.test.ts`n" -ForegroundColor Gray

npx playwright test tests/e2e/toss-complete-flow.e2e.test.ts --headed

$testResult = $LASTEXITCODE

# Step 3: Report results
Write-Host "`n================================" -ForegroundColor Cyan

if ($testResult -eq 0) {
    Write-Host "✅ ALL TESTS PASSED!" -ForegroundColor Green
    Write-Host "`n🎉 TOSS is fully operational!" -ForegroundColor Cyan
} else {
    Write-Host "❌ SOME TESTS FAILED" -ForegroundColor Red
    Write-Host "   Check the test report above for details" -ForegroundColor Yellow
}

Write-Host "`n📊 Test Report:" -ForegroundColor Cyan
Write-Host "   Backend: $BACKEND_URL" -ForegroundColor Gray
Write-Host "   Frontend: $FRONTEND_URL" -ForegroundColor Gray
Write-Host "   Swagger: $BACKEND_URL/swagger`n" -ForegroundColor Gray

# Ask if user wants to keep servers running
Write-Host "Press any key to continue (servers will remain running)..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

exit $testResult

