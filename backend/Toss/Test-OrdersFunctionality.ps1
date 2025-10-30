# Orders Functionality Test Script
# Tests all order-related endpoints and functionality

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Orders Functionality Test Suite" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5001/api"
$shopId = 1

# Test 1: Check if backend is running
Write-Host "[Test 1] Checking backend connectivity..." -ForegroundColor Yellow
try {
    $test = Test-NetConnection -ComputerName localhost -Port 5001 -InformationLevel Quiet
    if ($test) {
        Write-Host "✅ Backend is running on port 5001" -ForegroundColor Green
    } else {
        Write-Host "❌ Backend is not running on port 5001" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ Error checking backend: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Test 2: Get Customer Orders
Write-Host "[Test 2] Testing GET /api/CustomerOrders..." -ForegroundColor Yellow
try {
    $uri = "$baseUrl/CustomerOrders?shopId=$shopId&pageNumber=1&pageSize=10"
    $response = Invoke-RestMethod -Uri $uri -Method GET -ContentType "application/json" -ErrorAction Stop
    
    Write-Host "✅ Successfully retrieved orders" -ForegroundColor Green
    Write-Host "   Found $($response.Count) orders" -ForegroundColor White
    
    if ($response.Count -gt 0) {
        Write-Host "`n   Sample Order:" -ForegroundColor Cyan
        $firstOrder = $response[0]
        Write-Host "   - ID: $($firstOrder.id)" -ForegroundColor White
        Write-Host "   - Order Number: $($firstOrder.orderNumber)" -ForegroundColor White
        Write-Host "   - Customer: $($firstOrder.customerName)" -ForegroundColor White
        Write-Host "   - Status: $($firstOrder.orderStatus)" -ForegroundColor White
        Write-Host "   - Total: R$($firstOrder.orderTotal)" -ForegroundColor White
        Write-Host "   - Items: $($firstOrder.itemCount)" -ForegroundColor White
        Write-Host "   - Date: $($firstOrder.orderDate)" -ForegroundColor White
        
        # Verify data structure
        $requiredFields = @("id", "orderNumber", "customerName", "orderStatus", "orderTotal", "itemCount")
        $missingFields = $requiredFields | Where-Object { -not $firstOrder.PSObject.Properties.Name.Contains($_) }
        
        if ($missingFields.Count -eq 0) {
            Write-Host "   ✅ All required fields are present" -ForegroundColor Green
        } else {
            Write-Host "   ⚠️  Missing fields: $($missingFields -join ', ')" -ForegroundColor Yellow
        }
        
        # Check status mapping
        $statuses = $response | ForEach-Object { $_.orderStatus } | Select-Object -Unique
        Write-Host "`n   Order Statuses found:" -ForegroundColor Cyan
        foreach ($status in $statuses) {
            Write-Host "   - $status" -ForegroundColor White
        }
    } else {
        Write-Host "   ⚠️  No orders found. Make sure database is seeded." -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ Error retrieving orders: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "   Status Code: $statusCode" -ForegroundColor Red
    }
}

Write-Host ""

# Test 3: Filter by Status
Write-Host "[Test 3] Testing order filtering by status..." -ForegroundColor Yellow
$statuses = @("Pending", "Processing", "Complete", "Cancelled")
foreach ($status in $statuses) {
    try {
        $uri = "$baseUrl/CustomerOrders?shopId=$shopId&status=$status&pageNumber=1&pageSize=10"
        $response = Invoke-RestMethod -Uri $uri -Method GET -ContentType "application/json" -ErrorAction Stop
        Write-Host "   ✅ Status '$status': $($response.Count) orders" -ForegroundColor Green
    } catch {
        Write-Host "   ⚠️  Status '$status': Error - $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""

# Test 4: Pagination
Write-Host "[Test 4] Testing pagination..." -ForegroundColor Yellow
try {
    $uri = "$baseUrl/CustomerOrders?shopId=$shopId&pageNumber=1&pageSize=5"
    $response = Invoke-RestMethod -Uri $uri -Method GET -ContentType "application/json" -ErrorAction Stop
    Write-Host "   ✅ Page 1 (size 5): $($response.Count) orders" -ForegroundColor Green
    
    if ($response.Count -eq 5) {
        Write-Host "   ✅ Pagination working correctly" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  Expected 5 orders, got $($response.Count)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ❌ Error testing pagination: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 5: Filter by Customer
Write-Host "[Test 5] Testing customer filter..." -ForegroundColor Yellow
try {
    # First, get all orders to find a customer ID
    $uri = "$baseUrl/CustomerOrders?shopId=$shopId&pageNumber=1&pageSize=1"
    $response = Invoke-RestMethod -Uri $uri -Method GET -ContentType "application/json" -ErrorAction Stop
    
    if ($response.Count -gt 0) {
        $customerId = $response[0].customerId
        $uri = "$baseUrl/CustomerOrders?shopId=$shopId&customerId=$customerId&pageNumber=1&pageSize=10"
        $filteredResponse = Invoke-RestMethod -Uri $uri -Method GET -ContentType "application/json" -ErrorAction Stop
        Write-Host "   ✅ Customer ID ${customerId}: $($filteredResponse.Count) orders" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  No orders found to test customer filter" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ❌ Error testing customer filter: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 6: Verify Frontend Compatibility
Write-Host "[Test 6] Verifying frontend data compatibility..." -ForegroundColor Yellow
try {
    $uri = "$baseUrl/CustomerOrders?shopId=$shopId&pageNumber=1&pageSize=1"
    $response = Invoke-RestMethod -Uri $uri -Method GET -ContentType "application/json" -ErrorAction Stop
    
    if ($response.Count -gt 0) {
        $order = $response[0]
        
        # Check if orderStatus is a string (as expected by frontend)
        $statusType = $order.orderStatus.GetType().Name
        if ($statusType -eq "String") {
            Write-Host "   ✅ orderStatus is a string (frontend compatible)" -ForegroundColor Green
        } else {
            Write-Host "   ⚠️  orderStatus is a $statusType (may need conversion)" -ForegroundColor Yellow
        }
        
        # Check date format
        $dateStr = $order.orderDate
        Write-Host "   ✅ orderDate format: $dateStr" -ForegroundColor Green
        
        # Verify required fields for frontend mapping
        $frontendRequired = @{
            "id" = "number"
            "orderNumber" = "string"
            "customerName" = "string"
            "orderStatus" = "string"
            "orderTotal" = "number"
            "itemCount" = "number"
        }
        
        Write-Host "`n   Frontend field compatibility:" -ForegroundColor Cyan
        foreach ($field in $frontendRequired.Keys) {
            if ($order.PSObject.Properties.Name.Contains($field)) {
                $actualType = $order.$field.GetType().Name
                $expectedType = $frontendRequired[$field]
                if ($actualType -eq $expectedType -or ($expectedType -eq "number" -and ($actualType -eq "Int32" -or $actualType -eq "Decimal"))) {
                    Write-Host "   ✅ $field : $actualType" -ForegroundColor Green
                } else {
                    Write-Host "   ⚠️  $field : $actualType (expected $expectedType)" -ForegroundColor Yellow
                }
            } else {
                Write-Host "   ❌ $field : Missing" -ForegroundColor Red
            }
        }
    }
} catch {
    Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 7: Test Frontend URL
Write-Host "[Test 7] Checking frontend accessibility..." -ForegroundColor Yellow
try {
    $frontendTest = Test-NetConnection -ComputerName localhost -Port 3001 -InformationLevel Quiet
    if ($frontendTest) {
        Write-Host "   ✅ Frontend is running on port 3001" -ForegroundColor Green
        Write-Host "   📍 Navigate to: http://localhost:3001/sales/orders" -ForegroundColor Cyan
    } else {
        Write-Host "   ⚠️  Frontend is not running on port 3001" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ⚠️  Could not check frontend: $_" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Test Suite Complete" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Open browser: http://localhost:3001/sales/orders" -ForegroundColor White
Write-Host "   2. Check browser console for errors" -ForegroundColor White
Write-Host "   3. Verify orders load correctly" -ForegroundColor White
Write-Host "   4. Test filtering and search functionality" -ForegroundColor White
Write-Host "   5. Test order expansion/details" -ForegroundColor White
