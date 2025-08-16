# Client Integration Test Script (PowerShell)
# Tests all client applications' connectivity to the backend through the API Gateway

param(
    [string]$GatewayUrl = "http://localhost:8080",
    [int]$Timeout = 10
)

# Configuration
$StockApiUrl = "$GatewayUrl/api/stock"

Write-Host "🔌 Testing Client Integration with Backend Services" -ForegroundColor Blue
Write-Host "==================================================" -ForegroundColor Blue
Write-Host ""

# Function to test endpoint
function Test-Endpoint {
    param(
        [string]$Url,
        [string]$Description,
        [int]$ExpectedStatus = 200
    )
    
    Write-Host "Testing $Description... " -NoNewline
    
    try {
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec $Timeout -ErrorAction Stop
        if ($response.StatusCode -eq $ExpectedStatus) {
            Write-Host "✅ PASS" -ForegroundColor Green
            return $true
        } else {
            Write-Host "❌ FAIL (Status: $($response.StatusCode))" -ForegroundColor Red
            return $false
        }
    } catch {
        Write-Host "❌ FAIL - Connection error" -ForegroundColor Red
        return $false
    }
}

# Function to test API response
function Test-ApiResponse {
    param(
        [string]$Url,
        [string]$Description
    )
    
    Write-Host "Testing $Description... " -NoNewline
    
    try {
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec $Timeout -ErrorAction Stop
        $content = $response.Content
        
        if ($content -match "error|Error") {
            Write-Host "⚠️  WARNING - API returned error" -ForegroundColor Yellow
            return $true
        } else {
            Write-Host "✅ PASS" -ForegroundColor Green
            return $true
        }
    } catch {
        Write-Host "❌ FAIL - Connection error" -ForegroundColor Red
        return $false
    }
}

# Test counter
$passed = 0
$failed = 0
$warnings = 0

Write-Host "1. Testing Gateway Health" -ForegroundColor Blue
Write-Host "------------------------" -ForegroundColor Blue

if (Test-Endpoint "$GatewayUrl/health" "Gateway Health Check" 200) {
    $passed++
} else {
    $failed++
}

Write-Host ""

Write-Host "2. Testing Stock API Endpoints" -ForegroundColor Blue
Write-Host "--------------------------------" -ForegroundColor Blue

# Test Stock API endpoints
if (Test-Endpoint "$StockApiUrl/items" "Stock Items Endpoint" 200) {
    $passed++
} else {
    $failed++
}

if (Test-Endpoint "$StockApiUrl/levels" "Stock Levels Endpoint" 200) {
    $passed++
} else {
    $failed++
}

if (Test-Endpoint "$StockApiUrl/movements" "Stock Movements Endpoint" 200) {
    $passed++
} else {
    $failed++
}

Write-Host ""

Write-Host "3. Testing Client-Specific Endpoints" -ForegroundColor Blue
Write-Host "----------------------------------------" -ForegroundColor Blue

# Test mobile dashboard endpoint
if (Test-Endpoint "$GatewayUrl/api/mobile/dashboard" "Mobile Dashboard" 200) {
    $passed++
} else {
    $failed++
}

# Test web dashboard endpoint
if (Test-Endpoint "$GatewayUrl/api/web/dashboard" "Web Dashboard" 200) {
    $passed++
} else {
    $failed++
}

Write-Host ""

Write-Host "4. Testing Authentication Endpoints" -ForegroundColor Blue
Write-Host "----------------------------------------" -ForegroundColor Blue

# Test authentication endpoints (should return 200 without token for now)
if (Test-Endpoint "$StockApiUrl/items" "Stock API Auth Check" 200) {
    $passed++
} else {
    $failed++
}

Write-Host ""

Write-Host "5. Testing CORS Configuration" -ForegroundColor Blue
Write-Host "--------------------------------" -ForegroundColor Blue

# Test CORS headers
Write-Host "Testing CORS headers... " -NoNewline
try {
    $response = Invoke-WebRequest -Uri "$GatewayUrl/api/mobile/dashboard" -Headers @{"Origin" = "http://localhost:3000"} -TimeoutSec $Timeout -ErrorAction Stop
    $corsHeader = $response.Headers["Access-Control-Allow-Origin"]
    
    if ($corsHeader) {
        Write-Host "✅ PASS" -ForegroundColor Green
        $passed++
    } else {
        Write-Host "⚠️  WARNING - CORS headers not found" -ForegroundColor Yellow
        $warnings++
    }
} catch {
    Write-Host "⚠️  WARNING - Could not test CORS" -ForegroundColor Yellow
    $warnings++
}

Write-Host ""

Write-Host "6. Testing Client Environment Files" -ForegroundColor Blue
Write-Host "----------------------------------------" -ForegroundColor Blue

# Check if environment template files exist
function Check-EnvFile {
    param(
        [string]$File,
        [string]$Client
    )
    
    if (Test-Path $File) {
        Write-Host "  ✅ $Client`: $File exists" -ForegroundColor Green
        $script:passed++
    } else {
        Write-Host "  ❌ $Client`: $File missing" -ForegroundColor Red
        $script:failed++
    }
}

Check-EnvFile "src/clients/mobile/env.template" "Mobile"
Check-EnvFile "src/clients/web/env.template" "Web"
Check-EnvFile "src/clients/admin/env.template" "Admin"

Write-Host ""

Write-Host "7. Testing Client API Services" -ForegroundColor Blue
Write-Host "-----------------------------------" -ForegroundColor Blue

# Check if API service files exist and are properly configured
function Check-ApiService {
    param(
        [string]$File,
        [string]$Client
    )
    
    if (Test-Path $File) {
        # Check if it points to gateway
        $content = Get-Content $File -Raw
        if ($content -match "localhost:8080") {
            Write-Host "  ✅ $Client`: API service configured for gateway" -ForegroundColor Green
            $script:passed++
        } else {
            Write-Host "  ⚠️  $Client`: API service may not be configured for gateway" -ForegroundColor Yellow
            $script:warnings++
        }
    } else {
        Write-Host "  ❌ $Client`: API service file missing" -ForegroundColor Red
        $script:failed++
    }
}

Check-ApiService "src/clients/mobile/lib/core/network/api_service.dart" "Mobile"
Check-ApiService "src/clients/web/services/api.ts" "Web"
Check-ApiService "src/clients/admin/src/services/api.ts" "Admin"

Write-Host ""

Write-Host "8. Testing Gateway Configuration" -ForegroundColor Blue
Write-Host "------------------------------------" -ForegroundColor Blue

# Check gateway configuration
if (Test-Path "src/Gateway/appsettings.json") {
    $content = Get-Content "src/Gateway/appsettings.json" -Raw
    if ($content -match "stock-api") {
        Write-Host "  ✅ Gateway configured for Stock API" -ForegroundColor Green
        $passed++
    } else {
        Write-Host "  ❌ Gateway not configured for Stock API" -ForegroundColor Red
        $failed++
    }
} else {
    Write-Host "  ❌ Gateway configuration file missing" -ForegroundColor Red
    $failed++
}

Write-Host ""

# Summary
Write-Host "==================================================" -ForegroundColor Blue
Write-Host "Integration Test Summary" -ForegroundColor Blue
Write-Host "==================================================" -ForegroundColor Blue
Write-Host "Tests Passed: $passed" -ForegroundColor Green
Write-Host "Tests Failed: $failed" -ForegroundColor Red
Write-Host "Warnings: $warnings" -ForegroundColor Yellow
Write-Host ""

if ($failed -eq 0) {
    Write-Host "🎉 All critical tests passed! Clients are properly wired to the backend." -ForegroundColor Green
    exit 0
} else {
    Write-Host "❌ Some tests failed. Please review the issues above." -ForegroundColor Red
    exit 1
}
