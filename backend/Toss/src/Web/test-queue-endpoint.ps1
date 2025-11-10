$response = Invoke-RestMethod -Uri "http://localhost:5000/api/sales/queue?shopId=1" -Method Get -ErrorAction Stop
Write-Host " Queue endpoint works!" -ForegroundColor Green
Write-Host "Response:" -ForegroundColor Cyan
$response | ConvertTo-Json -Depth 3
