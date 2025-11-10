# PowerShell script to test creating a queue-based order

$backendUrl = 'http://localhost:5000'

Write-Host '=== Testing Queue Order Creation ===' -ForegroundColor Cyan

# Get some products first
Write-Host '1. Fetching products...' -ForegroundColor Yellow
try {
    $productsResponse = Invoke-RestMethod -Uri "$backendUrl/api/Products?shopId=1&pageSize=5" -Method Get
    
    if (!$productsResponse.items -or $productsResponse.items.Count -eq 0) {
        Write-Host 'No products found! Please add products first.' -ForegroundColor Red
        exit 1
    }
    
    $firstProduct = $productsResponse.items[0]
    Write-Host "Found product: $($firstProduct.name) (ID: $($firstProduct.id), Price: R$($firstProduct.price))" -ForegroundColor Green
}
catch {
    Write-Host "Error fetching products: $_" -ForegroundColor Red
    Write-Host 'Make sure the backend is running on http://localhost:5000' -ForegroundColor Yellow
    exit 1
}

# Create a queue order
Write-Host '2. Creating queue order...' -ForegroundColor Yellow
$orderData = @{
    shopId = 1
    customerName = 'John Doe'
    customerPhone = '+27123456789'
    customerNotes = 'Extra cheese please'
    items = @(
        @{
            productId = $firstProduct.id
            quantity = 2
            unitPrice = $firstProduct.price
        }
    )
    paymentType = 'Cash'
    totalAmount = ([double]$firstProduct.price * 2)
    saleType = 1
    estimatedPreparationMinutes = 15
}

$orderJson = $orderData | ConvertTo-Json -Depth 10

Write-Host 'Request body:' -ForegroundColor Gray
Write-Host $orderJson -ForegroundColor Gray

try {
    $createResponse = Invoke-RestMethod -Uri "$backendUrl/api/Sales" -Method Post -Body $orderJson -ContentType 'application/json'
    Write-Host "Order created with ID: $($createResponse.id)" -ForegroundColor Green
}
catch {
    Write-Host "Error creating order: $_" -ForegroundColor Red
    exit 1
}

# Fetch queue orders
Write-Host '3. Fetching queue orders...' -ForegroundColor Yellow
try {
    $queueResponse = Invoke-RestMethod -Uri "$backendUrl/api/Sales/queue?shopId=1" -Method Get
    Write-Host "Found $($queueResponse.Count) orders in queue" -ForegroundColor Green
    
    foreach ($order in $queueResponse) {
        Write-Host '' 
        Write-Host "Order #$($order.saleNumber)" -ForegroundColor Cyan
        Write-Host "   Customer: $($order.customerName)" -ForegroundColor White
        Write-Host "   Status: $($order.status)" -ForegroundColor White
        Write-Host "   Total: R$($order.total)" -ForegroundColor White
        Write-Host "   Items: $($order.items.Count)" -ForegroundColor White
        if ($order.customerNotes) {
            Write-Host "   Notes: $($order.customerNotes)" -ForegroundColor Gray
        }
    }
}
catch {
    Write-Host "Error fetching queue: $_" -ForegroundColor Red
    exit 1
}

Write-Host '' 
Write-Host 'Test completed successfully!' -ForegroundColor Green
Write-Host 'You can now view the order at: http://localhost:3000/sales/orders/queue' -ForegroundColor Cyan
