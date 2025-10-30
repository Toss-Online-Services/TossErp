Write-Host "🚀 Starting TOSS Backend..." -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host ""

Set-Location "backend\Toss\src\AppHost"

Write-Host "📍 Location: $(Get-Location)" -ForegroundColor Cyan
Write-Host "⏳ Starting .NET Aspire AppHost..." -ForegroundColor Yellow
Write-Host ""

dotnet run

Write-Host ""
Write-Host "❌ Backend stopped or failed" -ForegroundColor Red
Read-Host "Press Enter to close"
