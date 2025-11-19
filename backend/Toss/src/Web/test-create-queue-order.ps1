# PowerShell script to test creating a queue-based order

$backendUrl = "https://localhost:5001"

Write-Host "=== Testing Queue Order Creation ===" -ForegroundColor Cyan

# Get some products first
Write-Host "1. Fetching products..." -ForegroundColor Yellow
$productsResponse = Invoke-RestMethod -Uri "$backendUrl/api/Products?shopId=1&pageSize=5" `
    -Method Get `
    -SkipCertificateCheck `
    -ErrorAction Stop

if ($productsResponse.items.Count -eq 0) {
    Write-Host " No products found! Please add products first." -ForegroundColor Red
    exit 1
}

$firstProduct = $productsResponse.items[0]
Write-Host " Found product: $($firstProduct.name) (ID: $($firstProduct.id))" -ForegroundColor Green

# Create a queue order
Write-Host "
2. Creating queue order..." -ForegroundColor Yellow
$orderData = @{
    shopId = 1
    customerName = "John Doe"
    customerPhone = "+27123456789"
    customerNotes = "Extra cheese please"
    items = @(
        @{
            productId = $firstProduct.id
            quantity = 2
            unitPrice = $firstProduct.price
        }
    )
    paymentType = "Cash"
    totalAmount = ($firstProduct.price * 2)
    saleType = 1  # QueueOrder
    estimatedPreparationMinutes = 15
}

$orderJson = $orderData | ConvertTo-Json -Depth 10

Write-Host "Request body:" -ForegroundColor Gray
Write-Host $orderJson -ForegroundColor Gray

$createResponse = Invoke-RestMethod -Uri "$backendUrl/api/Sales" `
    -Method Post `
    -Body $orderJson `
    -ContentType "application/json" `
    -SkipCertificateCheck `
    -ErrorAction Stop

Write-Host " Order created with ID: $($createResponse.id)" -ForegroundColor Green

# Fetch queue orders
Write-Host "
3. Fetching queue orders..." -ForegroundColor Yellow
$queueResponse = Invoke-RestMethod -Uri "$backendUrl/api/Sales/queue?shopId=1" `
    -Method Get `
    -SkipCertificateCheck `
    -ErrorAction Stop

Write-Host " Found $($queueResponse.Count) orders in queue" -ForegroundColor Green

foreach ($order in $queueResponse) {
    Write-Host "
 Order #$($order.saleNumber)" -ForegroundColor Cyan
    Write-Host "   Customer: $($order.customerName)" -ForegroundColor White
    Write-Host "   Status: $($order.status)" -ForegroundColor White
    Write-Host "   Total: R$($order.total)" -ForegroundColor White
    Write-Host "   Items: $($order.items.Count)" -ForegroundColor White
}

Write-Host "
 Test completed successfully!" -ForegroundColor Green
