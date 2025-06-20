# Fix MudBlazor component type inference issues
$files = @(
    "Pages/CustomerDialog.razor",
    "Pages/ReturnDialog.razor",
    "Pages/Customers.razor",
    "Pages/Dashboard.razor",
    "Pages/InvoiceDialog.razor",
    "Pages/LoyaltyDetailsDialog.razor",
    "Pages/OrderHistoryDialog.razor",
    "Pages/PaymentDialog.razor",
    "Pages/Sales.razor",
    "Pages/Stock.razor",
    "Pages/StockHistoryDialog.razor"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        
        # Fix MudCheckBox components
        $content = $content -replace '<MudCheckBox @bind-Checked=', '<MudCheckBox T="bool" @bind-Checked='
        $content = $content -replace '<MudCheckBox @bind-Value=', '<MudCheckBox T="bool" @bind-Value='
        
        # Fix MudChip components
        $content = $content -replace '<MudChip Color=', '<MudChip T="string" Color='
        
        # Fix MudList components
        $content = $content -replace '<MudList', '<MudList T="string"'
        
        # Fix MudListItem components
        $content = $content -replace '<MudListItem', '<MudListItem T="string"'
        
        Set-Content $file $content -NoNewline
        Write-Host "Fixed $file"
    }
} 