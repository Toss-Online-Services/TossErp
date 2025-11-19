# Create test invoice script
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

# First, let's create a few test invoices
Write-Host "`n=== Creating Test Invoices ===" -ForegroundColor Cyan

for ($i = 1; $i -le 5; $i++) {
    $body = @{
        saleId = $i
        documentType = 1  # Invoice
        customerId = $i
        shopId = 1
        subtotal = (100 * $i)
        taxAmount = (15 * $i)
        totalAmount = (115 * $i)
        dueDate = (Get-Date).AddDays(30).ToString("o")
        notes = "Test invoice #$i - Created for testing"
    } | ConvertTo-Json

    try {
        Write-Host "Creating invoice for Sale $i..." -NoNewline
        $result = Invoke-RestMethod -Uri "https://localhost:5001/api/Sales/documents" -Method Post -Body $body -ContentType "application/json"
        Write-Host "  Invoice ID: $($result.id)" -ForegroundColor Green
    }
    catch {
        if ($_.Exception.Message -like "*already exists*" -or $_.Exception.Message -like "*duplicate*") {
            Write-Host "  Already exists (skipped)" -ForegroundColor Yellow
        }
        else {
            Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    Start-Sleep -Milliseconds 200
}

Write-Host "`n=== Querying Invoices ===" -ForegroundColor Cyan
try {
    $docs = Invoke-RestMethod -Uri "https://localhost:5001/api/Sales/documents?shopId=1" -Method Get
    Write-Host "Total documents found: $($docs.items.Count)" -ForegroundColor Green
    Write-Host "`nInvoices:" -ForegroundColor White
    $docs.items | Format-Table -Property id, documentNumber, documentType, totalAmount, isPaid -AutoSize
}
catch {
    Write-Host "Error querying documents: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== Done! ===" -ForegroundColor Cyan
Write-Host "Open http://localhost:3000/sales/invoices to view them" -ForegroundColor Yellow
