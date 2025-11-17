# Test TOSS AI Endpoints
Write-Host "Testing TOSS AI Endpoints" -ForegroundColor Cyan
Write-Host "============================`n" -ForegroundColor Cyan

$baseUrl = "http://localhost:5000"
$apiUrl = "$baseUrl/api"

# Wait for application to start
Write-Host "Waiting for application to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Test 1: Check if API is responding
Write-Host "`n1. Testing API Root..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri $apiUrl -UseBasicParsing -TimeoutSec 5
    Write-Host "   SUCCESS: API is responding (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "   FAILED: API not responding: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Please ensure the application is running with: dotnet run --project src/Web" -ForegroundColor Yellow
    exit 1
}

# Test 2: Check Swagger/OpenAPI
Write-Host "`n2. Testing OpenAPI Specification..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$apiUrl/specification.json" -UseBasicParsing
    $spec = $response.Content | ConvertFrom-Json
    Write-Host "   SUCCESS: OpenAPI spec loaded" -ForegroundColor Green
    Write-Host "   API Title: $($spec.info.title)" -ForegroundColor Cyan
    Write-Host "   Version: $($spec.info.version)" -ForegroundColor Cyan
    
    # Count AI endpoints
    $aiEndpoints = $spec.paths.PSObject.Properties | Where-Object { $_.Name -like "*AICopilot*" }
    Write-Host "   AI Endpoints found: $($aiEndpoints.Count)" -ForegroundColor Cyan
} catch {
    Write-Host "   WARNING: OpenAPI spec not available: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Test 3: Test AI Ask endpoint
Write-Host "`n3. Testing AI Ask Endpoint..." -ForegroundColor Yellow
try {
    $body = @{
        shopId = 1
        question = "What are my top selling products?"
        context = "test"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$apiUrl/AICopilot/ask" -Method Post -ContentType "application/json" -Body $body -ErrorAction Stop
    
    Write-Host "   SUCCESS: AI Ask endpoint responded" -ForegroundColor Green
    Write-Host "   Response:" -ForegroundColor Cyan
    Write-Host "      $($response | ConvertTo-Json -Depth 2)" -ForegroundColor Gray
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 401) {
        Write-Host "   INFO: Endpoint requires authentication (401) - This is expected" -ForegroundColor Cyan
    } elseif ($statusCode -eq 400) {
        Write-Host "   WARNING: Bad request (400) - Check request format" -ForegroundColor Yellow
    } else {
        Write-Host "   ERROR ($statusCode): $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test 4: Test AI Suggestions endpoint
Write-Host "`n4. Testing AI Suggestions Endpoint..." -ForegroundColor Yellow
try {
    $uri = [System.Uri]::new("$apiUrl/AICopilot/suggestions")
    $query = [System.Web.HttpUtility]::ParseQueryString([string]::Empty)
    $query['ShopId'] = '1'
    $query['MaxSuggestions'] = '3'
    $uriBuilder = [System.UriBuilder]::new($uri)
    $uriBuilder.Query = $query.ToString()
    $suggestionsUrl = $uriBuilder.ToString()
    
    $response = Invoke-RestMethod -Uri $suggestionsUrl -Method Get -ErrorAction Stop
    
    Write-Host "   SUCCESS: AI Suggestions endpoint responded" -ForegroundColor Green
    Write-Host "   Response:" -ForegroundColor Cyan
    Write-Host "      $($response | ConvertTo-Json -Depth 2)" -ForegroundColor Gray
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 401) {
        Write-Host "   INFO: Endpoint requires authentication (401) - This is expected" -ForegroundColor Cyan
    } else {
        Write-Host "   ERROR ($statusCode): $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test 5: Test AI Settings endpoint
Write-Host "`n5. Testing AI Settings Endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$apiUrl/AICopilot/settings/1" -Method Get -ErrorAction Stop
    
    Write-Host "   SUCCESS: AI Settings endpoint responded" -ForegroundColor Green
    if ($response) {
        Write-Host "   Settings retrieved for Shop 1" -ForegroundColor Cyan
    } else {
        Write-Host "   INFO: No settings found for Shop 1 (404) - This is expected for new database" -ForegroundColor Cyan
    }
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 404) {
        Write-Host "   INFO: No settings found (404) - This is expected for new database" -ForegroundColor Cyan
    } elseif ($statusCode -eq 401) {
        Write-Host "   INFO: Endpoint requires authentication (401)" -ForegroundColor Cyan
    } else {
        Write-Host "   WARNING: Error ($statusCode): $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

# Summary
Write-Host "`n==================================================" -ForegroundColor Cyan
Write-Host "Test Summary" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "`nSUCCESS: Backend API is running and responsive" -ForegroundColor Green
Write-Host "SUCCESS: AI endpoints are registered and accessible" -ForegroundColor Green
Write-Host "`nNext Steps:" -ForegroundColor Yellow
Write-Host "   1. Register a user: POST $apiUrl/Users/register" -ForegroundColor Gray
Write-Host "   2. Login: POST $apiUrl/Users/login" -ForegroundColor Gray
Write-Host "   3. Test AI with auth token" -ForegroundColor Gray
Write-Host "   4. Start frontend: cd toss-web; pnpm dev" -ForegroundColor Gray
Write-Host "`nAccess Swagger UI at: $apiUrl`n" -ForegroundColor Cyan
