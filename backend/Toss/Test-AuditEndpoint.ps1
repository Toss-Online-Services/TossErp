# Test script for Audit endpoint
Write-Host "Testing Audit Endpoint" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5000"
$endpoint = "$baseUrl/api/Audit/log"

# Test 1: Valid request
Write-Host "Test 1: Valid Audit Event (Info)" -ForegroundColor Yellow
$body = @{
    action = "UserLogin"
    severity = "info"
    resource = "Authentication"
    success = $true
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body -ContentType "application/json"
    Write-Host "✅ Success!" -ForegroundColor Green
    Write-Host "Response: $($response | ConvertTo-Json -Depth 3)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

# Test 2: Missing required field
Write-Host "Test 2: Missing Action Field (Should Fail)" -ForegroundColor Yellow
$body = @{
    severity = "warning"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body -ContentType "application/json"
    Write-Host "❌ Should have failed but didn't!" -ForegroundColor Red
    Write-Host ""
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "✅ Correctly returned 400 Bad Request" -ForegroundColor Green
        Write-Host "Error: $($_.ErrorDetails.Message)" -ForegroundColor Gray
    } else {
        Write-Host "❌ Unexpected error: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

# Test 3: Invalid severity
Write-Host "Test 3: Invalid Severity Value (Should Fail)" -ForegroundColor Yellow
$body = @{
    action = "TestAction"
    severity = "invalid"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body -ContentType "application/json"
    Write-Host "❌ Should have failed but didn't!" -ForegroundColor Red
    Write-Host ""
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "✅ Correctly returned 400 Bad Request" -ForegroundColor Green
        Write-Host "Error: $($_.ErrorDetails.Message)" -ForegroundColor Gray
    } else {
        Write-Host "❌ Unexpected error: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

# Test 4: Critical severity with error
Write-Host "Test 4: Critical Audit Event with Error" -ForegroundColor Yellow
$body = @{
    action = "FailedLoginAttempt"
    severity = "critical"
    resource = "Authentication"
    success = $false
    errorMessage = "Invalid credentials"
    details = @{
        username = "testuser"
        attemptCount = 3
    }
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body -ContentType "application/json"
    Write-Host "✅ Success!" -ForegroundColor Green
    Write-Host "Response: $($response | ConvertTo-Json -Depth 3)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

# Test 5: Full audit event with all fields
Write-Host "Test 5: Complete Audit Event with All Fields" -ForegroundColor Yellow
$body = @{
    action = "DataExport"
    severity = "warning"
    userId = "user-123"
    userEmail = "test@example.com"
    ipAddress = "192.168.1.100"
    userAgent = "Mozilla/5.0 (Test Browser)"
    resource = "Reports"
    resourceId = "report-456"
    success = $true
    details = @{
        reportType = "Sales"
        recordCount = 1000
        format = "CSV"
    }
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body -ContentType "application/json"
    Write-Host "✅ Success!" -ForegroundColor Green
    Write-Host "Response: $($response | ConvertTo-Json -Depth 3)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

Write-Host "======================" -ForegroundColor Cyan
Write-Host "Testing Complete!" -ForegroundColor Cyan











