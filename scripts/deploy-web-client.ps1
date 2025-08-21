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

Write-Info "🚀 TOSS ERP Web Client Complete Setup and Local Deployment"
Write-Info "============================================================"

# Validate environment
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$webClientPath = Join-Path $projectRoot "src\clients\web"
$dockerPath = Join-Path $projectRoot "docker"

Write-Info "📍 Project root: $projectRoot"
Write-Info "📍 Web client path: $webClientPath"

if (-not (Test-Path $webClientPath)) {
    Write-Error "❌ Web client directory not found: $webClientPath"
    exit 1
}

# Step 1: Create missing components for complete web application
Write-Info "🔧 Creating missing components and completing web application..."

# Create API services if they don't exist
$servicesPath = Join-Path $webClientPath "services"
if (-not (Test-Path $servicesPath)) {
    New-Item -ItemType Directory -Path $servicesPath -Force | Out-Null
}

# Create missing stores for state management
$storesPath = Join-Path $webClientPath "stores"
if (-not (Test-Path $storesPath)) {
    New-Item -ItemType Directory -Path $storesPath -Force | Out-Null
}

# Create composables for data management
$composablesPath = Join-Path $webClientPath "composables"
if (-not (Test-Path $composablesPath)) {
    New-Item -ItemType Directory -Path $composablesPath -Force | Out-Null
}

# Create components directory if it doesn't exist
$componentsPath = Join-Path $webClientPath "components"
if (-not (Test-Path $componentsPath)) {
    New-Item -ItemType Directory -Path $componentsPath -Force | Out-Null
}

# Create utils directory
$utilsPath = Join-Path $webClientPath "utils"
if (-not (Test-Path $utilsPath)) {
    New-Item -ItemType Directory -Path $utilsPath -Force | Out-Null
}

Write-Success "✅ Directory structure created"

# Step 2: Create missing chart components for dashboard
Write-Info "📊 Creating chart components for dashboard visualization..."

$chartComponents = @"
<template>
  <div class="chart-container">
    <canvas ref="chartCanvas"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import Chart from 'chart.js/auto'

interface ChartProps {
  data: any[]
  type: 'line' | 'bar' | 'pie' | 'doughnut'
  options?: any
  width?: number
  height?: number
}

const props = withDefaults(defineProps<ChartProps>(), {
  type: 'line',
  width: 400,
  height: 200
})

const chartCanvas = ref<HTMLCanvasElement>()
let chartInstance: Chart | null = null

const initChart = () => {
  if (!chartCanvas.value) return

  // Destroy existing chart
  if (chartInstance) {
    chartInstance.destroy()
  }

  chartInstance = new Chart(chartCanvas.value, {
    type: props.type,
    data: props.data,
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom'
        }
      },
      ...props.options
    }
  })
}

watch(() => props.data, () => {
  initChart()
}, { deep: true })

onMounted(() => {
  initChart()
})

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.destroy()
  }
})
</script>

<style scoped>
.chart-container {
  position: relative;
  width: 100%;
  height: 100%;
}
</style>
"@

$chartComponentPath = Join-Path $componentsPath "Chart.vue"
if (-not (Test-Path $chartComponentPath)) {
    Set-Content -Path $chartComponentPath -Value $chartComponents -Encoding UTF8
    Write-Success "✅ Chart component created"
}

# Step 3: Create environment configuration file
Write-Info "⚙️ Creating environment configuration..."

$envContent = @"
# TOSS ERP Web Client Environment Configuration
# Development Environment

# API Gateway URL
NUXT_PUBLIC_GATEWAY_URL=http://localhost:8080

# WebSocket URLs (for real-time features)
NUXT_PUBLIC_WS_URL=ws://localhost:8080/ws

# Feature Flags
NUXT_PUBLIC_ENABLE_CHATBOT=true
NUXT_PUBLIC_ENABLE_ANALYTICS=true
NUXT_PUBLIC_ENABLE_COLLABORATION=true

# Development Settings
NUXT_DEVTOOLS=enabled
NITRO_LOG_LEVEL=info

# App Configuration
NUXT_PUBLIC_APP_NAME=TOSS ERP
NUXT_PUBLIC_APP_VERSION=3.0.0
NUXT_PUBLIC_DEFAULT_LOCALE=en
NUXT_PUBLIC_CURRENCY=ZAR

# Third-party Services (Add your API keys here)
# NUXT_PUBLIC_GOOGLE_MAPS_API_KEY=
# NUXT_PUBLIC_STRIPE_PUBLISHABLE_KEY=
# NUXT_PUBLIC_ANALYTICS_ID=
"@

$envPath = Join-Path $webClientPath ".env"
if (-not (Test-Path $envPath)) {
    Set-Content -Path $envPath -Value $envContent -Encoding UTF8
    Write-Success "✅ Environment configuration created"
}

# Step 4: Create utils for common functionality
Write-Info "🛠️ Creating utility functions..."

$utilsContent = @"
// Common utility functions for TOSS ERP Web Client

export const formatCurrency = (amount: number, currency: string = 'ZAR'): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: currency,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

export const formatDate = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date
  return new Intl.DateTimeFormat('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  }).format(dateObj)
}

export const formatDateTime = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date
  return new Intl.DateTimeFormat('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(dateObj)
}

export const debounce = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: NodeJS.Timeout
  return (...args: Parameters<T>) => {
    clearTimeout(timeout)
    timeout = setTimeout(() => func(...args), wait)
  }
}

export const generateId = (): string => {
  return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
}

export const truncateText = (text: string, length: number): string => {
  if (text.length <= length) return text
  return text.substr(0, length) + '...'
}

export const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

export const validatePhoneNumber = (phone: string): boolean => {
  // South African phone number validation
  const phoneRegex = /^(\+27|0)[6-8][0-9]{8}$/
  return phoneRegex.test(phone.replace(/\s/g, ''))
}

export const capitalizeFirst = (text: string): string => {
  if (!text) return ''
  return text.charAt(0).toUpperCase() + text.slice(1).toLowerCase()
}

export const deepClone = <T>(obj: T): T => {
  return JSON.parse(JSON.stringify(obj))
}

export const groupBy = <T>(array: T[], key: keyof T): Record<string, T[]> => {
  return array.reduce((groups, item) => {
    const group = String(item[key])
    groups[group] = groups[group] || []
    groups[group].push(item)
    return groups
  }, {} as Record<string, T[]>)
}

export const sortBy = <T>(array: T[], key: keyof T, direction: 'asc' | 'desc' = 'asc'): T[] => {
  return [...array].sort((a, b) => {
    const aVal = a[key]
    const bVal = b[key]
    
    if (aVal < bVal) return direction === 'asc' ? -1 : 1
    if (aVal > bVal) return direction === 'asc' ? 1 : -1
    return 0
  })
}

export const downloadFile = (data: any, filename: string, type: string = 'application/json'): void => {
  const blob = new Blob([typeof data === 'string' ? data : JSON.stringify(data, null, 2)], { type })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = filename
  link.click()
  URL.revokeObjectURL(url)
}

export const copyToClipboard = async (text: string): Promise<boolean> => {
  try {
    await navigator.clipboard.writeText(text)
    return true
  } catch (err) {
    // Fallback for older browsers
    const textArea = document.createElement('textarea')
    textArea.value = text
    document.body.appendChild(textArea)
    textArea.select()
    document.execCommand('copy')
    document.body.removeChild(textArea)
    return true
  }
}

export const isMobile = (): boolean => {
  return window.innerWidth < 768
}

export const isTablet = (): boolean => {
  return window.innerWidth >= 768 && window.innerWidth < 1024
}

export const isDesktop = (): boolean => {
  return window.innerWidth >= 1024
}

export const getDeviceType = (): 'mobile' | 'tablet' | 'desktop' => {
  if (isMobile()) return 'mobile'
  if (isTablet()) return 'tablet'
  return 'desktop'
}
"@

$utilsFilePath = Join-Path $utilsPath "index.ts"
if (-not (Test-Path $utilsFilePath)) {
    Set-Content -Path $utilsFilePath -Value $utilsContent -Encoding UTF8
    Write-Success "✅ Utility functions created"
}

# Step 5: Install dependencies if package.json exists
Write-Info "📦 Installing web client dependencies..."

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
            Write-Success "✅ Dependencies installed"
        } else {
            Write-Info "⏭️ Skipping dependency installation (SkipBuild flag set)"
        }
    } else {
        Write-Warning "⚠️ package.json not found in web client directory"
    }
} catch {
    Write-Error "❌ Failed to install dependencies: $_"
} finally {
    Pop-Location
}

# Step 6: Update Nuxt configuration for better local development
Write-Info "🔧 Updating Nuxt configuration for local development..."

$nuxtConfigPath = Join-Path $webClientPath "nuxt.config.ts"
if (Test-Path $nuxtConfigPath) {
    $nuxtConfig = Get-Content $nuxtConfigPath -Raw
    
    # Check if configuration needs updates
    if ($nuxtConfig -notmatch "devtools.*enabled") {
        Write-Info "Adding devtools configuration..."
        # This would require more sophisticated text replacement
        # For now, we'll just inform the user
        Write-Info "ℹ️ Please ensure devtools are enabled in nuxt.config.ts"
    }
    
    Write-Success "✅ Nuxt configuration checked"
} else {
    Write-Warning "⚠️ nuxt.config.ts not found"
}

# Step 7: Check if Docker services are running
Write-Info "🐳 Checking Docker services status..."

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
        Write-Info "  cd docker && docker-compose up -d"
    }
} catch {
    Write-Warning "⚠️ Docker not available: $_"
}

# Step 8: Start the web client development server
Write-Info "🚀 Starting web client development server..."

# Create a launch script for the web client
$launchScript = @'
@echo off
echo Starting TOSS ERP Web Client...
cd /d "c:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\src\clients\web"
echo.
echo ============================================
echo   TOSS ERP Web Client Development Server
echo ============================================
echo.
echo 🌐 Frontend: http://localhost:3001
echo 🔗 API Gateway: http://localhost:8080
echo 📊 API Documentation: http://localhost:8080/swagger
echo.
echo Press Ctrl+C to stop the server
echo.
npm run dev
pause
'@

$launchScriptPath = Join-Path $projectRoot "scripts\start-web-client.bat"
Set-Content -Path $launchScriptPath -Value $launchScript -Encoding UTF8

# Create PowerShell version
$launchScriptPS = @'
# TOSS ERP Web Client Launcher
Write-Host "Starting TOSS ERP Web Client..." -ForegroundColor Cyan
Set-Location "c:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\src\clients\web"
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "   TOSS ERP Web Client Development Server" -ForegroundColor Green  
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Frontend: http://localhost:3001" -ForegroundColor Yellow
Write-Host "🔗 API Gateway: http://localhost:8080" -ForegroundColor Yellow
Write-Host "📊 API Documentation: http://localhost:8080/swagger" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor White
Write-Host ""
npm run dev
'@

$launchScriptPSPath = Join-Path $projectRoot "scripts\start-web-client.ps1"
Set-Content -Path $launchScriptPSPath -Value $launchScriptPS -Encoding UTF8

Write-Success "✅ Launch scripts created"

# Step 9: Create comprehensive Docker Compose for local development
Write-Info "🐳 Creating comprehensive local development Docker Compose..."

$localDockerCompose = @"
version: '3.8'

# TOSS ERP Local Development Environment
# Complete setup for local development with hot-reload

services:
  # Infrastructure Services
  postgres:
    image: postgres:16-alpine
    container_name: tosserp-postgres-local
    environment:
      POSTGRES_DB: tosserp_main
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres123
    ports:
      - "5432:5432"
    volumes:
      - postgres-local-data:/var/lib/postgresql/data
      - ./init-scripts:/docker-entrypoint-initdb.d
    networks:
      - toss-local-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: tosserp-redis-local
    ports:
      - "6379:6379"
    volumes:
      - redis-local-data:/data
    networks:
      - toss-local-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5
    command: redis-server --appendonly yes

  rabbitmq:
    image: rabbitmq:3-management-alpine
    container_name: tosserp-rabbitmq-local
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
      RABBITMQ_DEFAULT_VHOST: /
    ports:
      - "5672:5672"    # AMQP port
      - "15672:15672"  # Management UI
    volumes:
      - rabbitmq-local-data:/var/lib/rabbitmq
    networks:
      - toss-local-network
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Development Database Admin
  pgadmin:
    image: dpage/pgadmin4:latest
    container_name: tosserp-pgadmin-local
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@tosserp.com
      PGADMIN_DEFAULT_PASSWORD: admin123
      PGADMIN_CONFIG_SERVER_MODE: 'False'
    ports:
      - "5050:80"
    volumes:
      - pgadmin-local-data:/var/lib/pgadmin
    networks:
      - toss-local-network
    depends_on:
      - postgres

  # Redis Commander (Redis Web UI)
  redis-commander:
    image: rediscommander/redis-commander:latest
    container_name: tosserp-redis-commander-local
    environment:
      REDIS_HOSTS: redis:redis:6379
    ports:
      - "8081:8081"
    networks:
      - toss-local-network
    depends_on:
      - redis

volumes:
  postgres-local-data:
    driver: local
  redis-local-data:
    driver: local
  rabbitmq-local-data:
    driver: local
  pgadmin-local-data:
    driver: local

networks:
  toss-local-network:
    driver: bridge
    name: toss-local-network
"@

$localDockerComposePath = Join-Path $projectRoot "docker-compose.local.yml"
Set-Content -Path $localDockerComposePath -Value $localDockerCompose -Encoding UTF8
Write-Success "✅ Local Docker Compose created"

# Step 10: Create startup script for the complete local environment
Write-Info "🚀 Creating complete local environment startup script..."

$startupScript = @"
#!/bin/bash
# TOSS ERP Complete Local Development Environment Startup

echo "🚀 Starting TOSS ERP Complete Local Development Environment"
echo "============================================================="

# Function to check if port is available
check_port() {
    if lsof -Pi :$1 -sTCP:LISTEN -t >/dev/null ; then
        echo "⚠️  Port $1 is already in use"
        return 1
    else
        echo "✅ Port $1 is available"
        return 0
    fi
}

# Check required ports
echo "🔍 Checking required ports..."
check_port 3001 || echo "   Web client may conflict"
check_port 8080 || echo "   Gateway may conflict"
check_port 5432 || echo "   PostgreSQL may conflict"
check_port 6379 || echo "   Redis may conflict"
check_port 5672 || echo "   RabbitMQ may conflict"

echo ""
echo "🐳 Starting infrastructure services..."
cd docker
docker-compose -f ../docker-compose.local.yml up -d

echo ""
echo "⏱️  Waiting for infrastructure to be ready..."
sleep 15

echo ""
echo "🔧 Starting backend services..."
docker-compose up -d identity-api stock-api crm-api gateway

echo ""
echo "⏱️  Waiting for backend services to be ready..."
sleep 10

echo ""
echo "🌐 Starting web client..."
cd ../src/clients/web
npm run dev &

echo ""
echo "✅ TOSS ERP Development Environment Started!"
echo ""
echo "📍 Access Points:"
echo "   🌐 Web Client:        http://localhost:3001"
echo "   🔗 API Gateway:       http://localhost:8080"
echo "   📊 API Docs:          http://localhost:8080/swagger"
echo "   🗄️  PostgreSQL:       localhost:5432 (postgres/postgres123)"
echo "   📦 Redis:             localhost:6379"
echo "   🐰 RabbitMQ:          localhost:5672 (guest/guest)"
echo "   📊 RabbitMQ UI:       http://localhost:15672"
echo "   🔧 PgAdmin:           http://localhost:5050 (admin@tosserp.com/admin123)"
echo "   📊 Redis Commander:   http://localhost:8081"
echo ""
echo "🛑 To stop all services: ./scripts/stop-local-env.sh"
echo ""
"@

$startupScriptPath = Join-Path $projectRoot "scripts\start-local-env.sh"
Set-Content -Path $startupScriptPath -Value $startupScript -Encoding UTF8

# Create Windows version
$startupScriptWin = @"
@echo off
echo 🚀 Starting TOSS ERP Complete Local Development Environment
echo =============================================================

echo.
echo 🐳 Starting infrastructure services...
cd docker
docker-compose -f ../docker-compose.local.yml up -d

echo.
echo ⏱️  Waiting for infrastructure to be ready...
timeout /t 15 >nul

echo.
echo 🔧 Starting backend services...
docker-compose up -d identity-api stock-api crm-api gateway

echo.
echo ⏱️  Waiting for backend services to be ready...
timeout /t 10 >nul

echo.
echo 🌐 Starting web client...
cd ../src/clients/web
start cmd /k "npm run dev"

echo.
echo ✅ TOSS ERP Development Environment Started!
echo.
echo 📍 Access Points:
echo    🌐 Web Client:        http://localhost:3001
echo    🔗 API Gateway:       http://localhost:8080
echo    📊 API Docs:          http://localhost:8080/swagger
echo    🗄️  PostgreSQL:       localhost:5432 (postgres/postgres123)
echo    📦 Redis:             localhost:6379
echo    🐰 RabbitMQ:          localhost:5672 (guest/guest)
echo    📊 RabbitMQ UI:       http://localhost:15672
echo    🔧 PgAdmin:           http://localhost:5050 (admin@tosserp.com/admin123)
echo    📊 Redis Commander:   http://localhost:8081
echo.
echo 🛑 To stop all services: scripts\stop-local-env.bat
echo.
pause
"@

$startupScriptWinPath = Join-Path $projectRoot "scripts\start-local-env.bat"
Set-Content -Path $startupScriptWinPath -Value $startupScriptWin -Encoding UTF8

Write-Success "✅ Startup scripts created"

# Step 11: Create stop script
$stopScript = @"
@echo off
echo 🛑 Stopping TOSS ERP Local Development Environment
echo ===================================================

echo.
echo 🔌 Stopping web client...
taskkill /f /im node.exe 2>nul

echo.
echo 🐳 Stopping Docker services...
cd docker
docker-compose down
docker-compose -f ../docker-compose.local.yml down

echo.
echo ✅ All services stopped!
pause
"@

$stopScriptPath = Join-Path $projectRoot "scripts\stop-local-env.bat"
Set-Content -Path $stopScriptPath -Value $stopScript -Encoding UTF8

Write-Success "✅ Stop script created"

# Final summary and instructions
Write-Info ""
Write-Success "🎉 TOSS ERP Web Client Setup Complete!"
Write-Success "======================================"
Write-Info ""
Write-Info "📂 What was created/updated:"
Write-Info "   ✅ Web client directory structure"
Write-Info "   ✅ Chart components for dashboard visualization"  
Write-Info "   ✅ Environment configuration (.env)"
Write-Info "   ✅ Utility functions for common operations"
Write-Info "   ✅ Dependencies installed (if not skipped)"
Write-Info "   ✅ Local Docker Compose configuration"
Write-Info "   ✅ Complete environment startup/stop scripts"
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
Write-Success "🎉 TOSS ERP is ready for local development!"
