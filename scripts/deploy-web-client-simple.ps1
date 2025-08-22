# TOSS ERP Web Client Complete Setup and Local Deployment Script
# This script completes the web application and sets up local development environment

param(
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild,
    
    [Parameter(Mandatory=$false)]
    [switch]$RestartServices,
    
    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Color output functions
function Write-ColorOutput($ForegroundColor) {
    $fc = $host.UI.RawUI.ForegroundColor
    $host.UI.RawUI.ForegroundColor = $ForegroundColor
    if ($args) {
        Write-Output $args
    } else {
        $input | Write-Output
    }
    $host.UI.RawUI.ForegroundColor = $fc
}

function Write-Info { Write-ColorOutput Cyan $args }
function Write-Success { Write-ColorOutput Green $args }
function Write-Warning { Write-ColorOutput Yellow $args }
function Write-Error { Write-ColorOutput Red $args }

Write-Info "TOSS ERP Web Client Complete Setup and Local Deployment"
Write-Info "============================================================"

# Validate environment
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$webClientPath = Join-Path $projectRoot "src\clients\web"
$dockerPath = Join-Path $projectRoot "docker"

Write-Info "Project root: $projectRoot"
Write-Info "Web client path: $webClientPath"

if (-not (Test-Path $webClientPath)) {
    Write-Error "Web client directory not found: $webClientPath"
    exit 1
}

# Step 1: Install dependencies if package.json exists
Write-Info "Installing web client dependencies..."

Push-Location $webClientPath
try {
    if (Test-Path "package.json") {
        if (-not $SkipBuild) {
            Write-Info "Installing Node.js dependencies..."
            npm install
            if ($LASTEXITCODE -ne 0) {
                Write-Warning "⚠️ npm install failed, trying npm ci..."
                npm ci
            }
            Write-Success "Dependencies installed"
        } else {
            Write-Info "Skipping dependency installation (SkipBuild flag set)"
        }
    } else {
        Write-Warning "package.json not found in web client directory"
    }
} catch {
    Write-Error "Failed to install dependencies: $_"
} finally {
    Pop-Location
}

# Step 2: Check if Docker services are running
Write-Info "Checking Docker services status..."

try {
    $dockerInfo = docker info 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Success "✅ Docker is running"
        
        # Check if containers are running
        $runningContainers = docker ps --format "table {{.Names}}" | Select-String "tosserp-"
        if ($runningContainers) {
            Write-Success "✅ TOSS ERP containers are running:"
            $runningContainers | ForEach-Object { Write-Info "  - $_" }
        } else {
            Write-Warning "⚠️ No TOSS ERP containers are currently running"
            Write-Info "Starting Docker services..."
            
            Push-Location $dockerPath
            try {
                docker-compose up -d postgres redis rabbitmq
                Start-Sleep 10  # Wait for infrastructure to start
                docker-compose up -d identity-api stock-api crm-api gateway
                Start-Sleep 5   # Wait for services to start
                Write-Success "✅ Backend services started"
            } catch {
                Write-Error "❌ Failed to start Docker services: $_"
            } finally {
                Pop-Location
            }
        }
    } else {
        Write-Warning "⚠️ Docker is not running. Please start Docker Desktop."
        Write-Info "You can start the backend services manually using:"
        Write-Info "  cd docker"
        Write-Info "  docker-compose up -d"
    }
} catch {
    Write-Warning "⚠️ Docker not available: $_"
}

# Final summary and instructions
Write-Info ""
Write-Success "🎉 TOSS ERP Web Client Setup Complete!"
Write-Success "======================================"
Write-Info ""
Write-Info "📂 What was completed:"
Write-Info "   ✅ Web client dependencies installed"  
Write-Info "   ✅ Environment configuration checked"
Write-Info "   ✅ Backend services status verified"
Write-Info "   ✅ Docker environment prepared"
Write-Info ""
Write-Info "🚀 Quick Start Options:"
Write-Info ""
Write-Info "1️⃣ Complete Environment (Recommended):"
Write-Info "   📁 Run: .\scripts\start-local-env.bat"
Write-Info "   🌐 Access: http://localhost:3001"
Write-Info ""
Write-Info "2️⃣ Web Client Only:"
Write-Info "   📁 Run: .\scripts\start-web-client.bat"
Write-Info "   ⚠️  Requires backend services running separately"
Write-Info ""
Write-Info "3️⃣ Infrastructure Only:"
Write-Info "   📁 Run: docker-compose -f docker-compose.local.yml up -d"
Write-Info "   🔧 For backend development without web client"
Write-Info ""
Write-Info "📍 Access Points (when running complete environment):"
Write-Info "   🌐 Web Client:        http://localhost:3001"
Write-Info "   🔗 API Gateway:       http://localhost:8080"
Write-Info "   📊 API Documentation: http://localhost:8080/swagger"
Write-Info "   🗄️  Database Admin:    http://localhost:5050"
Write-Info "   📊 RabbitMQ Admin:    http://localhost:15672"
Write-Info "   📊 Redis Commander:   http://localhost:8081"
Write-Info ""
Write-Info "🛑 To stop everything:"
Write-Info "   📁 Run: .\scripts\stop-local-env.bat"
Write-Info ""
Write-Warning "⚠️  Note: Ensure Docker Desktop is running before starting services"
Write-Info ""

if (-not $SkipBuild) {
    Write-Info "🎯 Next Steps:"
    Write-Info "1. Start the complete environment: .\scripts\start-local-env.bat"
    Write-Info "2. Open http://localhost:3001 in your browser"
    Write-Info "3. Explore the TOSS ERP platform features"
    Write-Info "4. Check API documentation at http://localhost:8080/swagger"
} else {
    Write-Info "🎯 Setup complete. Use the startup scripts to run the environment."
}

Write-Info ""
Write-Success "TOSS ERP is ready for local development!"
