# PowerShell script to start the TOSS Web API
# This script will:
# 1. Kill any existing Web API processes
# 2. Navigate to the Web project directory
# 3. Start the Web API

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  TOSS Web API Starter" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Function to stop existing Web API processes
function Stop-ExistingWebAPI {
    Write-Host "🔍 Checking for existing Web API processes..." -ForegroundColor Yellow
    Write-Host ""
    
    # Find any dotnet processes running Web.dll or from the Web directory
    $webProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
        $_.Path -like "*Toss.Web*" -or 
        $_.MainModule.FileName -like "*Toss.Web*"
    }
    
    if ($webProcesses) {
        Write-Host "Found $($webProcesses.Count) existing Web API process(es)" -ForegroundColor Yellow
        Write-Host ""
        
        foreach ($process in $webProcesses) {
            Write-Host "  📦 Stopping process ID: $($process.Id)" -ForegroundColor Yellow
            Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        }
        
        # Wait a moment for processes to fully stop
        Start-Sleep -Seconds 2
        
        Write-Host ""
        Write-Host "✅ All existing Web API processes stopped" -ForegroundColor Green
        Write-Host ""
    }
    else {
        Write-Host "✅ No existing Web API processes found" -ForegroundColor Green
        Write-Host ""
    }
}

# Kill existing processes
Stop-ExistingWebAPI

# Navigate to Web directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host "📂 Working Directory: $scriptPath" -ForegroundColor Cyan
Write-Host ""

# Start the Web API
Write-Host "🚀 Starting TOSS Web API..." -ForegroundColor Green
Write-Host "============================" -ForegroundColor Green
Write-Host ""
Write-Host "Web API will be available at:" -ForegroundColor Cyan
Write-Host "  • HTTP:  http://localhost:5000" -ForegroundColor White
Write-Host "  • HTTPS: https://localhost:5001" -ForegroundColor White
Write-Host "  • Swagger: http://localhost:5000/swagger/index.html" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the Web API" -ForegroundColor Gray
Write-Host "============================" -ForegroundColor Green
Write-Host ""

# Start the Web API
try {
    dotnet run --urls "http://localhost:5000;https://localhost:5001"
}
catch {
    Write-Host ""
    Write-Host "❌ Failed to start Web API: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Ensure you are in the correct directory" -ForegroundColor Gray
    Write-Host "2. Run dotnet build first" -ForegroundColor Gray
    Write-Host "3. Check if PostgreSQL is running" -ForegroundColor Gray
    Write-Host "4. Verify no port conflicts exist" -ForegroundColor Gray
    Write-Host "5. Check connection string in appsettings.json" -ForegroundColor Gray
    exit 1
}
