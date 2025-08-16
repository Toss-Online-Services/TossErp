# TOSS ERP - Launch All Applications (PowerShell)
# This script launches all client applications and backend services

param(
    [switch]$SkipPrerequisites,
    [switch]$OpenBrowser
)

# Set console title
$Host.UI.RawUI.WindowTitle = "TOSS ERP - All Applications"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "    TOSS ERP - Launch All Applications" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Function to check if command exists
function Test-Command {
    param([string]$Command)
    try {
        Get-Command $Command -ErrorAction Stop | Out-Null
        return $true
    } catch {
        return $false
    }
}

# Function to create environment file
function Create-EnvironmentFile {
    param(
        [string]$ClientPath,
        [string]$ClientName
    )
    
    $templatePath = Join-Path $ClientPath "env.template"
    $envPath = Join-Path $ClientPath ".env"
    
    if (Test-Path $templatePath) {
        if (-not (Test-Path $envPath)) {
            Copy-Item $templatePath $envPath
            Write-Host "  ✅ $ClientName environment file created" -ForegroundColor Green
        } else {
            Write-Host "  ✅ $ClientName environment file already exists" -ForegroundColor Green
        }
    } else {
        Write-Host "  ⚠️  $ClientName template file not found" -ForegroundColor Yellow
    }
}

# Check prerequisites
if (-not $SkipPrerequisites) {
    Write-Host "Checking prerequisites..." -ForegroundColor Blue
    Write-Host ""
    
    $prerequisites = @{
        ".NET SDK" = "dotnet"
        "Node.js" = "node"
        "Flutter" = "flutter"
    }
    
    $allSatisfied = $true
    
    foreach ($prereq in $prerequisites.GetEnumerator()) {
        if (Test-Command $prereq.Value) {
            Write-Host "✅ $($prereq.Key) found" -ForegroundColor Green
        } else {
            Write-Host "❌ $($prereq.Key) not found" -ForegroundColor Red
            $allSatisfied = $false
        }
    }
    
    if (-not $allSatisfied) {
        Write-Host ""
        Write-Host "Please install the missing prerequisites:" -ForegroundColor Red
        Write-Host "  .NET SDK: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
        Write-Host "  Node.js: https://nodejs.org/" -ForegroundColor Yellow
        Write-Host "  Flutter: https://flutter.dev/docs/get-started/install" -ForegroundColor Yellow
        Write-Host ""
        Read-Host "Press Enter to exit"
        exit 1
    }
    
    Write-Host ""
    Write-Host "All prerequisites are satisfied!" -ForegroundColor Green
    Write-Host ""
}

# Get script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir

Write-Host "Starting applications..." -ForegroundColor Blue
Write-Host ""

# Create environment files
Write-Host "Creating environment files..." -ForegroundColor Blue
Create-EnvironmentFile (Join-Path $projectRoot "src\clients\mobile") "Mobile"
Create-EnvironmentFile (Join-Path $projectRoot "src\clients\web") "Web"
Create-EnvironmentFile (Join-Path $projectRoot "src\clients\admin") "Admin"
Write-Host ""

# Function to start application in new window
function Start-Application {
    param(
        [string]$Title,
        [string]$WorkingDirectory,
        [string]$Command,
        [string]$Arguments = ""
    )
    
    Write-Host "🚀 Starting $Title..." -ForegroundColor Green
    
    $startInfo = New-Object System.Diagnostics.ProcessStartInfo
    $startInfo.FileName = "cmd.exe"
    $startInfo.Arguments = "/k `"cd /d `"$WorkingDirectory`" && echo Starting $Title... && $Command $Arguments`""
    $startInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Normal
    
    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $startInfo
    $process.Start() | Out-Null
    
    # Wait a moment for the application to start
    Start-Sleep -Seconds 5
}

# Start all applications
try {
    # Start Gateway
    Start-Application "API Gateway" (Join-Path $projectRoot "src\Gateway") "dotnet run"
    
    # Start Stock API
    Start-Application "Stock API" (Join-Path $projectRoot "src\Services\Stock\Stock.API") "dotnet run"
    
    # Start Mobile Client
    Start-Application "Mobile Client" (Join-Path $projectRoot "src\clients\mobile") "flutter run -d web-server --web-port 5000"
    
    # Start Web Client
    Start-Application "Web Client" (Join-Path $projectRoot "src\clients\web") "npm run dev"
    
    # Start Admin Client
    Start-Application "Admin Client" (Join-Path $projectRoot "src\clients\admin") "npm start"
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "    All Applications Started!" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "🌐 Applications will be available at:" -ForegroundColor White
    Write-Host "    Gateway:        http://localhost:8080" -ForegroundColor Yellow
    Write-Host "    Stock API:      http://localhost:5001" -ForegroundColor Yellow
    Write-Host "    Mobile Client:  http://localhost:5000" -ForegroundColor Yellow
    Write-Host "    Web Client:     http://localhost:5173" -ForegroundColor Yellow
    Write-Host "    Admin Client:   http://localhost:3000" -ForegroundColor Yellow
    Write-Host ""
    
    Write-Host "📱 Mobile Client (Flutter Web) will open in your browser" -ForegroundColor White
    Write-Host "🌐 Web Client (Nuxt) will open in your browser" -ForegroundColor White
    Write-Host "⚙️  Admin Client (React) will open in your browser" -ForegroundColor White
    Write-Host ""
    Write-Host "🔌 Gateway and Stock API are running in background" -ForegroundColor White
    Write-Host ""
    
    if ($OpenBrowser) {
        Write-Host "Opening applications in browser..." -ForegroundColor Blue
        Start-Process "http://localhost:8080/health"
        Start-Sleep -Seconds 2
        Start-Process "http://localhost:5000"
        Start-Sleep -Seconds 2
        Start-Process "http://localhost:5173"
        Start-Sleep -Seconds 2
        Start-Process "http://localhost:3000"
    } else {
        Write-Host "Press Enter to open all applications in your browser..." -ForegroundColor Cyan
        Read-Host
        Write-Host "Opening applications in browser..." -ForegroundColor Blue
        Start-Process "http://localhost:8080/health"
        Start-Sleep -Seconds 2
        Start-Process "http://localhost:5000"
        Start-Sleep -Seconds 2
        Start-Process "http://localhost:5173"
        Start-Sleep -Seconds 2
        Start-Process "http://localhost:3000"
    }
    
    Write-Host ""
    Write-Host "🎉 All applications are now running!" -ForegroundColor Green
    Write-Host ""
    Write-Host "To stop all applications:" -ForegroundColor White
    Write-Host "1. Close each console window" -ForegroundColor Yellow
    Write-Host "2. Or press Ctrl+C in each window" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Happy coding! 🚀" -ForegroundColor Cyan
    
} catch {
    Write-Host ""
    Write-Host "❌ Error starting applications: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Please check the console output above for more details." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Press Enter to exit..."
Read-Host
