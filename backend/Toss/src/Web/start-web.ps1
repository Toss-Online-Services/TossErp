# ====================================
# TOSS Web API Startup Script
# ====================================
# This script ensures only one instance of the Web API runs at a time
# by killing any existing processes before starting a new one.

Write-Host "🚀 TOSS Web API Startup Script" -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan
Write-Host ""

# Function to kill existing Web API processes
function Stop-ExistingWebAPI {
    Write-Host "🔍 Checking for existing Web API processes..." -ForegroundColor Yellow
    
    # Find all dotnet processes that might be running the Web project
    $webProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
        $_.Path -like "*Web*" -or
        $_.MainWindowTitle -like "*Toss*Web*" -or
        $_.CommandLine -like "*Web.dll*"
    }
    
    # Also check for processes using ports 5000, 5001
    $portsToCheck = @(5000, 5001)
    $portProcesses = @()
    
    foreach ($port in $portsToCheck) {
        try {
            $connections = netstat -ano | Select-String ":$port\s" | Select-String "LISTENING"
            if ($connections) {
                foreach ($connection in $connections) {
                    $processId = ($connection -split '\s+')[-1]
                    if ($processId -and $processId -match '^\d+$') {
                        $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
                        if ($process) {
                            $portProcesses += $process
                        }
                    }
                }
            }
        }
        catch {
            # Ignore errors for individual ports
        }
    }
    
    # Combine and deduplicate processes
    $allProcesses = @($webProcesses) + @($portProcesses) | Sort-Object Id -Unique
    
    if ($allProcesses.Count -gt 0) {
        Write-Host "⚠️  Found $($allProcesses.Count) existing Web API process(es):" -ForegroundColor Yellow
        $allProcesses | ForEach-Object {
            Write-Host "   • PID $($_.Id): $($_.ProcessName)" -ForegroundColor Gray
        }
        
        Write-Host ""
        Write-Host "🔪 Terminating existing processes..." -ForegroundColor Red
        
        foreach ($process in $allProcesses) {
            try {
                Stop-Process -Id $process.Id -Force -ErrorAction Stop
                Write-Host "   ✅ Killed PID $($process.Id)" -ForegroundColor Green
            }
            catch {
                Write-Host "   ⚠️  Could not kill PID $($process.Id): $_" -ForegroundColor Yellow
            }
        }
        
        # Wait for processes to fully terminate
        Start-Sleep -Seconds 2
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
    dotnet run --no-build
}
catch {
    Write-Host ""
    Write-Host "❌ Failed to start Web API: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Ensure you're in the correct directory" -ForegroundColor Gray
    Write-Host "2. Run 'dotnet build' first" -ForegroundColor Gray
    Write-Host "3. Check if PostgreSQL is running" -ForegroundColor Gray
    Write-Host "4. Verify no port conflicts exist" -ForegroundColor Gray
    Write-Host "5. Check connection string in appsettings.json" -ForegroundColor Gray
    exit 1
}

