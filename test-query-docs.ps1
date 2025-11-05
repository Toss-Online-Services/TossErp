[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

# Query ALL sales documents
Write-Host "Querying all sales documents..." -ForegroundColor Yellow
$docs = Invoke-RestMethod -Uri "https://localhost:5001/api/Sales/documents?shopId=1" -Method Get
Write-Host "Found $($docs.items.Count) total documents" -ForegroundColor Green
$docs.items | Format-Table -Property id, documentNumber, documentType, totalAmount, isPaid
