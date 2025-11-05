[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

# Test creating a sales document
$body = @{
    saleId = 1
    documentType = 1
    customerId = 1
    shopId = 1
    subtotal = 100
    taxAmount = 15
    totalAmount = 115
    dueDate = (Get-Date).AddDays(30).ToString("o")
    notes = "Test invoice created via API"
} | ConvertTo-Json

Write-Host "Creating test invoice..." -ForegroundColor Yellow
$result = Invoke-RestMethod -Uri "https://localhost:5001/api/Sales/documents" -Method Post -Body $body -ContentType "application/json"
Write-Host "Invoice created with ID: $($result.id)" -ForegroundColor Green

# Query sales documents
Write-Host "`nQuerying sales documents..." -ForegroundColor Yellow
$docs = Invoke-RestMethod -Uri "https://localhost:5001/api/Sales/documents?shopId=1&type=1" -Method Get
Write-Host "Found $($docs.items.Count) invoices" -ForegroundColor Green
$docs.items | Format-Table -Property id, documentNumber, totalAmount, isPaid
