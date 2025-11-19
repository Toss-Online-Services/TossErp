[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

Write-Host "`n=== Querying Existing Sales ===" -ForegroundColor Cyan
try {
    $sales = Invoke-RestMethod -Uri "https://localhost:5001/api/Sales?ShopId=1&PageSize=10" -Method Get
    Write-Host "Total sales found: $($sales.items.Count)" -ForegroundColor Green
    
    if ($sales.items.Count -gt 0) {
        Write-Host "`nFirst 10 Sales:" -ForegroundColor White
        $sales.items | Select-Object -First 10 | Format-Table -Property id, saleNumber, total, customerId -AutoSize
        
        Write-Host "`n=== Creating Invoice for First Sale ===" -ForegroundColor Cyan
        $firstSale = $sales.items[0]
        
        # Check if this sale has a customer
        if ($null -eq $firstSale.customerId) {
            Write-Host "  First sale has no customer. Trying to find a sale with a customer..." -ForegroundColor Yellow
            $saleWithCustomer = $sales.items | Where-Object { $null -ne $_.customerId } | Select-Object -First 1
            
            if ($saleWithCustomer) {
                $firstSale = $saleWithCustomer
                Write-Host " Found sale $($firstSale.id) with customer $($firstSale.customerId)" -ForegroundColor Green
            } else {
                Write-Host " No sales with customers found. Cannot create invoice." -ForegroundColor Red
                exit
            }
        }
        
        $body = @{
            saleId = $firstSale.id
            documentType = 1  # Invoice
            dueDate = (Get-Date).AddDays(30).ToString("o")
            notes = "Test invoice for sale $($firstSale.saleNumber)"
        } | ConvertTo-Json
        
        Write-Host "Creating invoice for Sale ID: $($firstSale.id), SaleNumber: $($firstSale.saleNumber)" -NoNewline
        $result = Invoke-RestMethod -Uri "https://localhost:5001/api/Sales/documents" -Method Post -Body $body -ContentType "application/json"
        Write-Host "   Invoice ID: $($result.id)" -ForegroundColor Green
    }
}
catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
}
