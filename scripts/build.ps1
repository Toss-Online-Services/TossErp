# TOSS ERP - Build Script
# This script builds the entire solution

param(
    [string]$Configuration = "Debug",
    [string]$TargetFramework = "net9.0",
    [switch]$Clean,
    [switch]$Restore,
    [switch]$Test,
    [switch]$Publish
)

Write-Host "🚀 TOSS ERP Build Script" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Target Framework: $TargetFramework" -ForegroundColor Yellow

# Set error action preference
$ErrorActionPreference = "Stop"

# Function to execute command and check result
function Invoke-Command {
    param([string]$Command, [string]$Description)
    
    Write-Host "`n📋 $Description" -ForegroundColor Cyan
    Write-Host "Executing: $Command" -ForegroundColor Gray
    
    Invoke-Expression $Command
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed: $Description" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "✅ Completed: $Description" -ForegroundColor Green
}

try {
    # Clean if requested
    if ($Clean) {
        Invoke-Command -Command "dotnet clean TossErp.sln" -Description "Cleaning solution"
    }
    
    # Restore packages
    if ($Restore) {
        Invoke-Command -Command "dotnet restore TossErp.sln" -Description "Restoring NuGet packages"
    }
    
    # Build solution
    Invoke-Command -Command "dotnet build TossErp.sln -c $Configuration -f $TargetFramework" -Description "Building solution"
    
    # Run tests if requested
    if ($Test) {
        Invoke-Command -Command "dotnet test TossErp.sln -c $Configuration --no-build" -Description "Running tests"
    }
    
    # Publish if requested
    if ($Publish) {
        Invoke-Command -Command "dotnet publish TossErp.sln -c $Configuration -f $TargetFramework -o ./publish" -Description "Publishing solution"
    }
    
    Write-Host "`n🎉 Build completed successfully!" -ForegroundColor Green
    
} catch {
    Write-Host "`n💥 Build failed with error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
