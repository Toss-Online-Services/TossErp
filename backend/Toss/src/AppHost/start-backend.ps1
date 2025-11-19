# ====================================
# TOSS Backend Startup Script
# ====================================
# This script ensures only one instance of the backend runs at a time
# by killing any existing processes before starting a new one.

Write-Host "🚀 TOSS Backend Startup Script" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Function to kill existing backend processes
function Stop-ExistingBackend {
    Write-Host "🔍 Checking for existing backend processes..." -ForegroundColor Yellow
    
    # Find all dotnet processes that might be running the Web project
    $webProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
        $_.Path -like "*Web*" -or 
        $_.Path -like "*AppHost*" -or
        $_.MainWindowTitle -like "*Toss*" -or
        $_.CommandLine -like "*Web.dll*" -or
        $_.CommandLine -like "*AppHost.dll*"
    }
    
    # Also check for processes using ports 5000, 5001, 15010, 17078
    $portsToCheck = @(5000, 5001, 15010, 17078)
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
        Write-Host "⚠️  Found $($allProcesses.Count) existing backend process(es):" -ForegroundColor Yellow
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
        Write-Host "✅ No existing backend processes found" -ForegroundColor Green
        Write-Host ""
    }
}

# Kill existing processes
Stop-ExistingBackend

# Navigate to AppHost directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host "📂 Working Directory: $scriptPath" -ForegroundColor Cyan
Write-Host ""

# Start the backend
Write-Host "🚀 Starting TOSS Backend (AppHost)..." -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""
Write-Host "Backend will be available at:" -ForegroundColor Cyan
Write-Host "  • HTTP:  http://localhost:5000" -ForegroundColor White
Write-Host "  • HTTPS: https://localhost:5001" -ForegroundColor White
Write-Host "  • Swagger: http://localhost:5000/swagger/index.html" -ForegroundColor White
Write-Host "  • Aspire Dashboard: https://localhost:17078" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the backend" -ForegroundColor Gray
Write-Host "================================" -ForegroundColor Green
Write-Host ""

# Start the backend
try {
    dotnet run --project AppHost.csproj
}
catch {
    Write-Host ""
    Write-Host "❌ Failed to start backend: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Ensure you're in the correct directory" -ForegroundColor Gray
    Write-Host "2. Run 'dotnet restore' first" -ForegroundColor Gray
    Write-Host "3. Check if PostgreSQL is running" -ForegroundColor Gray
    Write-Host "4. Verify no port conflicts exist" -ForegroundColor Gray
    exit 1
}

