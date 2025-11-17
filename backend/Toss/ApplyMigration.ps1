# PowerShell script to apply the AI migration
# Requires .NET SDK

Write-Host "Applying AI Integration Migration..." -ForegroundColor Green

# Step 1: Mark base migration as applied
Write-Host "`n1. Marking base migration as applied..." -ForegroundColor Yellow
$markBaseSql = Get-Content "MarkBaseMigrationApplied.sql" -Raw

dotnet ef database execute --project src/Infrastructure --startup-project src/Web --sql $markBaseSql

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to mark base migration. Continuing anyway..." -ForegroundColor Red
}

# Step 2: Check migration status
Write-Host "`n2. Checking migration status..." -ForegroundColor Yellow
dotnet ef migrations list --project src/Infrastructure --startup-project src/Web

# Step 3: Apply AI migration
Write-Host "`n3. Applying AI integration migration..." -ForegroundColor Yellow
$env:SkipNSwag = "True"
dotnet ef database update --project src/Infrastructure --startup-project src/Web

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Migration applied successfully!" -ForegroundColor Green
    
    # Verify
    Write-Host "`n4. Verifying migration status..." -ForegroundColor Yellow
    dotnet ef migrations list --project src/Infrastructure --startup-project src/Web
    
    Write-Host "`n🎉 AI Integration is ready to use!" -ForegroundColor Green
} else {
    Write-Host "`n❌ Migration failed. Please check the error above." -ForegroundColor Red
    Write-Host "You can manually apply using the SQL file: AI_Migration.sql" -ForegroundColor Yellow
    Write-Host "See APPLY_MIGRATION_INSTRUCTIONS.md for details" -ForegroundColor Yellow
    exit 1
}

