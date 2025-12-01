# Database restore script for PostgreSQL (PowerShell)
# Usage: .\restore-database.ps1 -BackupFile <path> [-DatabaseName] [-Host] [-Port] [-User] [-Password]

param(
    [Parameter(Mandatory=$true)]
    [string]$BackupFile,
    [string]$DatabaseName = "TossErp",
    [string]$Host = "localhost",
    [int]$Port = 5432,
    [string]$User = "postgres",
    [string]$Password = "postgres"
)

$ErrorActionPreference = "Stop"

# Check if backup file exists
if (-not (Test-Path $BackupFile)) {
    Write-Error "Backup file not found: $BackupFile"
    exit 1
}

Write-Host "Starting restore of database: $DatabaseName"
Write-Host "Backup file: $BackupFile"

# Set environment variable for psql
$env:PGPASSWORD = $Password

try {
    # Check if backup is compressed
    $TempFile = $BackupFile
    $IsCompressed = $false
    
    if ($BackupFile -match '\.(gz|zip)$') {
        $IsCompressed = $true
        $TempFile = Join-Path $env:TEMP "restore_$(Get-Date -Format 'yyyyMMddHHmmss').sql"
        
        if ($BackupFile -match '\.gz$') {
            # Requires 7-Zip or similar for .gz files on Windows
            Write-Host "Decompressing backup file..."
            # Note: This requires 7-Zip or similar tool
            # Expand-Archive doesn't support .gz, so this is a placeholder
            Write-Warning "GZ decompression not implemented. Please decompress manually or use 7-Zip."
            exit 1
        }
        elseif ($BackupFile -match '\.zip$') {
            Write-Host "Extracting backup file..."
            Expand-Archive -Path $BackupFile -DestinationPath $env:TEMP -Force
            $TempFile = Join-Path $env:TEMP (Get-ChildItem -Path $env:TEMP -Filter "*.sql" | Select-Object -First 1).Name
        }
    }

    # Confirm before dropping database
    $Confirm = Read-Host "This will drop the existing database '$DatabaseName'. Continue? (yes/no)"
    if ($Confirm -ne "yes") {
        Write-Host "Restore cancelled"
        exit 0
    }

    # Drop and recreate database
    Write-Host "Dropping existing database..."
    & psql -h $Host -p $Port -U $User -d postgres -c "DROP DATABASE IF EXISTS `"$DatabaseName`";" 2>&1 | Out-Null
    
    Write-Host "Creating new database..."
    & psql -h $Host -p $Port -U $User -d postgres -c "CREATE DATABASE `"$DatabaseName`";"

    # Restore from backup
    Write-Host "Restoring database..."
    Get-Content $TempFile | & psql -h $Host -p $Port -U $User -d $DatabaseName

    Write-Host "Restore completed successfully"
}
finally {
    # Cleanup temp file if created
    if ($IsCompressed -and (Test-Path $TempFile)) {
        Remove-Item $TempFile -Force -ErrorAction SilentlyContinue
    }
    
    # Clear password from environment
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
}

