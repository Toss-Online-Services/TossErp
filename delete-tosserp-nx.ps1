# Script to delete the locked tosserp-nx folder
# Run this after closing Cursor, File Explorer, or any other programs that might have the folder open

$folderPath = "tosserp-nx"
$maxRetries = 10
$retryDelay = 2

Write-Host "Attempting to delete $folderPath..." -ForegroundColor Yellow

for ($i = 1; $i -le $maxRetries; $i++) {
    try {
        # Stop any Node/Nx processes that might be locking files
        Get-Process | Where-Object {$_.ProcessName -like "*node*" -or $_.ProcessName -like "*nx*"} | Stop-Process -Force -ErrorAction SilentlyContinue
        
        # Take ownership and grant full control
        takeown /f $folderPath /r /d y 2>&1 | Out-Null
        icacls $folderPath /grant "${env:USERNAME}:F" /t 2>&1 | Out-Null
        
        Start-Sleep -Seconds 1
        
        # Try to delete
        Remove-Item -Path $folderPath -Recurse -Force -ErrorAction Stop
        
        Write-Host "Successfully deleted $folderPath!" -ForegroundColor Green
        exit 0
    }
    catch {
        if ($i -lt $maxRetries) {
            Write-Host "Attempt $i failed. Retrying in $retryDelay seconds..." -ForegroundColor Yellow
            Start-Sleep -Seconds $retryDelay
        }
        else {
            Write-Host "Failed to delete after $maxRetries attempts." -ForegroundColor Red
            Write-Host "Please ensure:" -ForegroundColor Yellow
            Write-Host "  1. Cursor/VS Code is closed" -ForegroundColor Yellow
            Write-Host "  2. File Explorer windows with this folder are closed" -ForegroundColor Yellow
            Write-Host "  3. Any Nx daemon processes are stopped" -ForegroundColor Yellow
            Write-Host "  4. No other programs are accessing files in this folder" -ForegroundColor Yellow
            exit 1
        }
    }
}









