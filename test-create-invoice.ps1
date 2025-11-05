[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

Write-Host "`n=== Creating Test Invoice ===" -ForegroundColor Cyan

$body = @{
    saleId = 10
    documentType = 1
    customerId = 10
    shopId = 1
    subtotal = 500
    taxAmount = 75
    totalAmount = 575
    dueDate = (Get-Date).AddDays(30).ToString("o")
    notes = "Test invoice - Customer order for office supplies"
} | ConvertTo-Json

try {
    Write-Host "Creating invoice for Sale #10..." -NoNewline
    $result = Invoke-RestMethod -Uri "https://localhost:5001/api/Sales/documents" -Method Post -Body $body -ContentType "application/json"
    Write-Host "  Invoice created with ID: $($result.id)" -ForegroundColor Green
}
catch {
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== Querying All Sales Documents ===" -ForegroundColor Cyan
try {
    $docs = Invoke-RestMethod -Uri "https://localhost:5001/api/Sales/documents?shopId=1" -Method Get
    Write-Host " Found $($docs.items.Count) documents total" -ForegroundColor Green
    if ($docs.items.Count -gt 0) {
        Write-Host "`nDocuments:" -ForegroundColor White
        $docs.items | Format-Table -Property id, documentNumber, documentType, totalAmount, isPaid -AutoSize
    }
}
catch {
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Response: $($_.ErrorDetails.Message)" -ForegroundColor DarkRed
}

Write-Host "`n=== Open the invoices page ===" -ForegroundColor Yellow
Write-Host "http://localhost:3000/sales/invoices" -ForegroundColor Cyan
