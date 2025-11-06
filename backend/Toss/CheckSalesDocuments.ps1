# Check SalesDocuments from API
$baseURL = "http://localhost:5000"
$shopId = 1

Write-Host "🔍 Checking SalesDocuments from API..." -ForegroundColor Cyan
Write-Host "API: $baseURL/Sales/documents" -ForegroundColor Gray
Write-Host "ShopId: $shopId" -ForegroundColor Gray
Write-Host ""

try {
    # Call the API - properly escape the URL
    $uri = "$baseURL/Sales/documents?shopId=$shopId`&pageNumber=1`&pageSize=100"
    $response = Invoke-RestMethod -Uri $uri -Method Get -ContentType "application/json"
    
    Write-Host "✅ API Response Received!" -ForegroundColor Green
    Write-Host ""
    
    # Check response structure
    if ($response.Items) {
        $documents = $response.Items
        $totalCount = $response.TotalCount
        $pageNumber = $response.PageNumber
        $totalPages = $response.TotalPages
        
        Write-Host "📊 API Response Summary:" -ForegroundColor Yellow
        Write-Host "─────────────────────────────────────"
        Write-Host "Total Count (from API): $totalCount"
        Write-Host "Items Returned: $($documents.Count)"
        Write-Host "Page Number: $pageNumber"
        Write-Host "Total Pages: $totalPages"
        Write-Host ""
        
        # Break down by document type
        $invoices = $documents | Where-Object { $_.DocumentType -eq 1 -or $_.documentType -eq 1 }
        $receipts = $documents | Where-Object { $_.DocumentType -eq 2 -or $_.documentType -eq 2 }
        
        Write-Host "📋 Document Breakdown:" -ForegroundColor Yellow
        Write-Host "─────────────────────────────────────"
        Write-Host "Invoices (Type 1): $($invoices.Count)"
        Write-Host "Receipts (Type 2): $($receipts.Count)"
        Write-Host "Total Documents: $($documents.Count)"
        Write-Host ""
        
        # Show all documents
        Write-Host "📄 All Documents Details:" -ForegroundColor Yellow
        Write-Host "─────────────────────────────────────────────────────────────────────────────────────"
        $counter = 1
        foreach ($doc in $documents) {
            $id = if ($doc.Id) { $doc.Id } else { $doc.id }
            $docNumber = if ($doc.DocumentNumber) { $doc.DocumentNumber } else { $doc.documentNumber }
            $docType = if ($doc.DocumentType) { $doc.DocumentType } else { $doc.documentType }
            $customerId = if ($doc.CustomerId) { $doc.CustomerId } else { $doc.customerId }
            $saleId = if ($doc.SaleId) { $doc.SaleId } else { $doc.saleId }
            $total = if ($doc.TotalAmount) { $doc.TotalAmount } elseif ($doc.Total) { $doc.Total } else { $doc.totalAmount -or $doc.total }
            $date = if ($doc.DocumentDate) { $doc.DocumentDate } else { $doc.documentDate }
            $isPaid = if ($doc.IsPaid) { $doc.IsPaid } else { $doc.isPaid }
            
            $typeName = if ($docType -eq 1) { "INV" } elseif ($docType -eq 2) { "RCT" } else { "UNK" }
            
            $line = "{0:D2}. [{1}] {2} | CustomerId: {3} | SaleId: {4} | Date: {5} | Total: R{6:F2} | Paid: {7}" -f `
                $counter, $typeName, $docNumber, $customerId, $saleId, $date, $total, $isPaid
            Write-Host $line
            $counter++
        }
        
        Write-Host ""
        Write-Host "✅ Analysis complete!" -ForegroundColor Green
        Write-Host ""
        Write-Host "💡 These documents are coming from the SalesDocuments table in the database" -ForegroundColor Cyan
        Write-Host "   filtered by ShopId = $shopId" -ForegroundColor Gray
    }
    else {
        Write-Host "⚠️ Unexpected response structure" -ForegroundColor Red
        $response | ConvertTo-Json -Depth 3
    }
}
catch {
    Write-Host "❌ Error calling API: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "💡 Make sure the backend API is running on $baseURL" -ForegroundColor Yellow
    Write-Host "   Try: cd backend\Toss\src\Web && dotnet run" -ForegroundColor Gray
}
