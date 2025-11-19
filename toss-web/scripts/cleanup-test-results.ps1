# Cleanup script for Playwright test results
Write-Host "🧹 Cleaning up test results..." -ForegroundColor Yellow

# Remove test-results directories
if (Test-Path "test-results") {
    Write-Host "Removing test-results directory..." -ForegroundColor Cyan
    Remove-Item -Path "test-results" -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "✅ test-results directory removed" -ForegroundColor Green
}

# Remove playwright-report
if (Test-Path "playwright-report") {
    Write-Host "Removing playwright-report directory..." -ForegroundColor Cyan
    Remove-Item -Path "playwright-report" -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "✅ playwright-report directory removed" -ForegroundColor Green
}

Write-Host "✨ Cleanup complete!" -ForegroundColor Green

