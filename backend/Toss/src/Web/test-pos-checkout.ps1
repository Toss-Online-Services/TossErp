# PowerShell script to test POS Checkout endpoint
# Tests the /api/sales/pos/checkout endpoint

$backendUrl = "http://localhost:5000"
$apiUrl = "$backendUrl/api"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "POS Checkout Endpoint Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Check if backend is running
Write-Host "[Test 1] Checking backend connectivity..." -ForegroundColor Yellow
try {
    $test = Test-NetConnection -ComputerName localhost -Port 5000 -InformationLevel Quiet -WarningAction SilentlyContinue
    if ($test) {
        Write-Host "✅ Backend is running on port 5000" -ForegroundColor Green
    } else {
        Write-Host "❌ Backend is not running on port 5000" -ForegroundColor Red
        Write-Host "   Please start the backend first:" -ForegroundColor Yellow
        Write-Host "   cd backend/Toss/src/AppHost" -ForegroundColor Gray
        Write-Host "   .\start-backend.ps1" -ForegroundColor Gray
        exit 1
    }
} catch {
    Write-Host "❌ Error checking backend: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Test 2: Authenticate (get JWT token)
Write-Host "[Test 2] Authenticating..." -ForegroundColor Yellow
$token = $null
try {
    $loginData = @{
        email = "admin@toss.local"
        password = "Admin1!"
    } | ConvertTo-Json

    $loginResponse = Invoke-RestMethod -Uri "$apiUrl/v1/Auth/login" `
        -Method Post `
        -Body $loginData `
        -ContentType "application/json" `
        -ErrorAction Stop

    $token = $loginResponse.accessToken
    Write-Host "✅ Authentication successful" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Authentication failed (may need to create admin user first): $_" -ForegroundColor Yellow
    Write-Host "   Continuing without authentication (if endpoints allow it)..." -ForegroundColor Gray
}

Write-Host ""

# Test 3: Get products (we need product IDs for the checkout)
Write-Host "[Test 3] Fetching products..." -ForegroundColor Yellow
try {
    # Try versioned endpoint first
    $productsUrl = "$apiUrl/v1/Inventory/products?shopId=1&pageSize=5"
    Write-Host "   Trying: $productsUrl" -ForegroundColor Gray
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    if ($token) {
        $headers["Authorization"] = "Bearer $token"
    }

    $productsResponse = Invoke-RestMethod -Uri $productsUrl `
        -Method Get `
        -Headers $headers `
        -ErrorAction Stop

    if ($productsResponse.items.Count -eq 0) {
        Write-Host "❌ No products found! Please add products first." -ForegroundColor Red
        Write-Host "   You can add products via the Inventory API or through the admin interface" -ForegroundColor Yellow
        exit 1
    }

    $firstProduct = $productsResponse.items[0]
    Write-Host "✅ Found product: $($firstProduct.name) (ID: $($firstProduct.id), Price: R$($firstProduct.sellingPrice))" -ForegroundColor Green
    
    if ($productsResponse.items.Count -gt 1) {
        $secondProduct = $productsResponse.items[1]
        Write-Host "   Also using: $($secondProduct.name) (ID: $($secondProduct.id), Price: R$($secondProduct.sellingPrice))" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Failed to fetch products: $_" -ForegroundColor Red
    Write-Host "   Response: $($_.Exception.Response)" -ForegroundColor Gray
    exit 1
}

Write-Host ""

# Test 4: Test POS Checkout with Cash payment
Write-Host "[Test 4] Testing POS Checkout (Cash payment)..." -ForegroundColor Yellow
try {
    $checkoutData = @{
        shopId = 1
        customerId = $null
        paymentMethod = 0  # Cash = 0
        paymentReference = "test_pos_$(Get-Date -Format 'yyyyMMddHHmmss')"
        notes = "Test POS checkout - Cash payment"
        items = @(
            @{
                productId = $firstProduct.id
                quantity = 2
                unitPrice = $firstProduct.sellingPrice
                discountAmount = 0
            }
        )
    }

    if ($productsResponse.items.Count -gt 1) {
        $checkoutData.items += @{
            productId = $secondProduct.id
            quantity = 1
            unitPrice = $secondProduct.sellingPrice
            discountAmount = 5.00  # Test discount
        }
    }

    $checkoutJson = $checkoutData | ConvertTo-Json -Depth 10

    Write-Host "Request body:" -ForegroundColor Gray
    Write-Host ($checkoutJson | ConvertFrom-Json | ConvertTo-Json -Depth 10) -ForegroundColor Gray
    Write-Host ""

    # Try versioned endpoint
    $checkoutUrl = "$apiUrl/v1/Sales/pos/checkout"
    Write-Host "   Calling: $checkoutUrl" -ForegroundColor Gray
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    if ($token) {
        $headers["Authorization"] = "Bearer $token"
    }

    $checkoutResponse = Invoke-RestMethod -Uri $checkoutUrl `
        -Method Post `
        -Body $checkoutJson `
        -Headers $headers `
        -ErrorAction Stop

    Write-Host "✅ POS Checkout successful!" -ForegroundColor Green
    Write-Host "   Sale ID: $($checkoutResponse.saleId)" -ForegroundColor Cyan
    Write-Host "   Sale Number: $($checkoutResponse.saleNumber)" -ForegroundColor Cyan
    Write-Host "   Total: R$($checkoutResponse.total)" -ForegroundColor Cyan
    Write-Host "   Receipt ID: $($checkoutResponse.receiptId)" -ForegroundColor Cyan
    Write-Host "   Is New Sale: $($checkoutResponse.isNewSale)" -ForegroundColor Cyan

} catch {
    Write-Host "❌ POS Checkout failed: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "   Response: $responseBody" -ForegroundColor Gray
    }
    exit 1
}

Write-Host ""

# Test 5: Test POS Checkout with Card payment
Write-Host "[Test 5] Testing POS Checkout (Card payment)..." -ForegroundColor Yellow
try {
    $checkoutData = @{
        shopId = 1
        customerId = $null
        paymentMethod = 1  # Card = 1
        paymentReference = "test_pos_card_$(Get-Date -Format 'yyyyMMddHHmmss')"
        notes = "Test POS checkout - Card payment"
        items = @(
            @{
                productId = $firstProduct.id
                quantity = 1
                unitPrice = $firstProduct.sellingPrice
                discountAmount = 0
            }
        )
    }

    $checkoutJson = $checkoutData | ConvertTo-Json -Depth 10

    # Try versioned endpoint
    $checkoutUrl = "$apiUrl/v1/Sales/pos/checkout"
    Write-Host "   Calling: $checkoutUrl" -ForegroundColor Gray
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    if ($token) {
        $headers["Authorization"] = "Bearer $token"
    }

    $checkoutResponse = Invoke-RestMethod -Uri $checkoutUrl `
        -Method Post `
        -Body $checkoutJson `
        -Headers $headers `
        -ErrorAction Stop

    Write-Host "✅ POS Checkout (Card) successful!" -ForegroundColor Green
    Write-Host "   Sale ID: $($checkoutResponse.saleId)" -ForegroundColor Cyan
    Write-Host "   Sale Number: $($checkoutResponse.saleNumber)" -ForegroundColor Cyan
    Write-Host "   Total: R$($checkoutResponse.total)" -ForegroundColor Cyan

} catch {
    Write-Host "❌ POS Checkout (Card) failed: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "   Response: $responseBody" -ForegroundColor Gray
    }
}

Write-Host ""

# Test 6: Test POS Checkout with Mobile payment
Write-Host "[Test 6] Testing POS Checkout (Mobile payment)..." -ForegroundColor Yellow
try {
    $checkoutData = @{
        shopId = 1
        customerId = $null
        paymentMethod = 2  # MobileMoney = 2
        paymentReference = "test_pos_mobile_$(Get-Date -Format 'yyyyMMddHHmmss')"
        notes = "Test POS checkout - Mobile payment"
        items = @(
            @{
                productId = $firstProduct.id
                quantity = 1
                unitPrice = $firstProduct.sellingPrice
                discountAmount = 0
            }
        )
    }

    $checkoutJson = $checkoutData | ConvertTo-Json -Depth 10

    # Try versioned endpoint
    $checkoutUrl = "$apiUrl/v1/Sales/pos/checkout"
    Write-Host "   Calling: $checkoutUrl" -ForegroundColor Gray
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    if ($token) {
        $headers["Authorization"] = "Bearer $token"
    }

    $checkoutResponse = Invoke-RestMethod -Uri $checkoutUrl `
        -Method Post `
        -Body $checkoutJson `
        -Headers $headers `
        -ErrorAction Stop

    Write-Host "✅ POS Checkout (Mobile) successful!" -ForegroundColor Green
    Write-Host "   Sale ID: $($checkoutResponse.saleId)" -ForegroundColor Cyan
    Write-Host "   Sale Number: $($checkoutResponse.saleNumber)" -ForegroundColor Cyan
    Write-Host "   Total: R$($checkoutResponse.total)" -ForegroundColor Cyan

} catch {
    Write-Host "❌ POS Checkout (Mobile) failed: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "   Response: $responseBody" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✅ All POS Checkout tests completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Test the frontend POS page at http://localhost:3000/pos" -ForegroundColor Gray
Write-Host "2. Add items to cart and complete a sale" -ForegroundColor Gray
Write-Host "3. Verify the sale appears in recent sales" -ForegroundColor Gray

