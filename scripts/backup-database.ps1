# Database backup script for PostgreSQL (PowerShell)
# Usage: .\backup-database.ps1 [-DatabaseName] [-BackupDir] [-Host] [-Port] [-User] [-Password]

param(
    [string]$DatabaseName = "TossErp",
    [string]$BackupDir = ".\backups",
    [string]$Host = "localhost",
    [int]$Port = 5432,
    [string]$User = "postgres",
    [string]$Password = "postgres"
)

$ErrorActionPreference = "Stop"

# Ensure backup directory exists
if (-not (Test-Path $BackupDir)) {
    New-Item -ItemType Directory -Path $BackupDir -Force | Out-Null
}

$Timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$BackupFile = Join-Path $BackupDir "toss_${DatabaseName}_${Timestamp}.sql"

Write-Host "Starting backup of database: $DatabaseName"
Write-Host "Backup file: $BackupFile"

# Set environment variable for pg_dump
$env:PGPASSWORD = $Password

try {
    # Perform backup
    & pg_dump -h $Host -p $Port -U $User -d $DatabaseName `
        --format=plain `
        --no-owner `
        --no-privileges `
        --clean `
        --if-exists `
        | Out-File -FilePath $BackupFile -Encoding UTF8

    # Compress backup (requires 7-Zip or similar)
    if (Get-Command Compress-Archive -ErrorAction SilentlyContinue) {
        $CompressedFile = "${BackupFile}.zip"
        Compress-Archive -Path $BackupFile -DestinationPath $CompressedFile -Force
        Remove-Item $BackupFile
        $BackupFile = $CompressedFile
    }

    $FileSize = (Get-Item $BackupFile).Length / 1MB
    Write-Host "Backup completed: $BackupFile"
    Write-Host "Backup size: $([math]::Round($FileSize, 2)) MB"

    # Keep only last 30 days of backups
    $CutoffDate = (Get-Date).AddDays(-30)
    Get-ChildItem -Path $BackupDir -Filter "toss_${DatabaseName}_*" | 
        Where-Object { $_.LastWriteTime -lt $CutoffDate } | 
        Remove-Item -Force

    Write-Host "Backup cleanup completed (kept last 30 days)"
}
finally {
    # Clear password from environment
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
}

