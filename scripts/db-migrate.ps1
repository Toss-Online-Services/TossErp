# Database Migration Script for TOSS ERP
# This script handles database migrations for all services

param(
    [Parameter(Mandatory=$true)]
    [string]$Environment,
    
    [Parameter(Mandatory=$false)]
    [string]$Service = "all",
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$Rollback = $false,
    
    [Parameter(Mandatory=$false)]
    [string]$TargetMigration = ""
)

$ErrorActionPreference = "Stop"

# Configuration
$services = @(
    @{
        Name = "Identity"
        Path = "src/Services/identity"
        Project = "Identity.API.csproj"
        Context = "IdentityDbContext"
    },
    @{
        Name = "Stock"
        Path = "src/Services/Stock/Stock.API"
        Project = "Stock.API.csproj"
        Context = "StockDbContext"
    },
    @{
        Name = "Sales"
        Path = "src/Services/sales"
        Project = "Sales.API.csproj"
        Context = "SalesDbContext"
    },
    @{
        Name = "Accounting"
        Path = "src/Services/accounting"
        Project = "Accounting.API.csproj"
        Context = "AccountingDbContext"
    },
    @{
        Name = "CRM"
        Path = "src/Services/crm"
        Project = "CRM.API.csproj"
        Context = "CRMDbContext"
    }
)

# Environment-specific connection strings
$connectionStrings = @{
    "development" = "Host=localhost;Database=toss_erp_dev;Username=postgres;Password=postgres"
    "staging" = "Host=staging-db.toss-erp.com;Database=toss_erp_staging;Username=toss_user;Password=${env:STAGING_DB_PASSWORD}"
    "production" = "Host=prod-db.toss-erp.com;Database=toss_erp_prod;Username=toss_user;Password=${env:PRODUCTION_DB_PASSWORD}"
}

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $(
        switch ($Level) {
            "ERROR" { "Red" }
            "WARN" { "Yellow" }
            "SUCCESS" { "Green" }
            default { "White" }
        }
    )
}

function Test-ServiceExists {
    param([string]$ServicePath)
    return Test-Path (Join-Path $ServicePath "*.csproj")
}

function Run-Migration {
    param(
        [object]$ServiceConfig,
        [string]$ConnectionString,
        [bool]$IsDryRun,
        [bool]$IsRollback,
        [string]$Target
    )
    
    $servicePath = $ServiceConfig.Path
    $projectFile = $ServiceConfig.Project
    $contextName = $ServiceConfig.Context
    $serviceName = $ServiceConfig.Name
    
    if (-not (Test-ServiceExists $servicePath)) {
        Write-Log "Service $serviceName not found at $servicePath, skipping..." "WARN"
        return $true
    }
    
    Write-Log "Processing migrations for $serviceName..." "INFO"
    
    try {
        Push-Location $servicePath
        
        # Set connection string
        $env:ConnectionStrings__DefaultConnection = $ConnectionString
        
        if ($IsDryRun) {
            Write-Log "DRY RUN: Would run migrations for $serviceName" "INFO"
            
            # List pending migrations
            $pendingMigrations = dotnet ef migrations list --context $contextName --json | ConvertFrom-Json
            if ($pendingMigrations) {
                Write-Log "Pending migrations for $serviceName:" "INFO"
                $pendingMigrations | ForEach-Object { Write-Log "  - $($_.Name)" "INFO" }
            } else {
                Write-Log "No pending migrations for $serviceName" "INFO"
            }
        }
        elseif ($IsRollback) {
            if ($Target) {
                Write-Log "Rolling back $serviceName to migration: $Target" "WARN"
                dotnet ef database update $Target --context $contextName --verbose
            } else {
                Write-Log "Rolling back $serviceName to initial state" "WARN"
                dotnet ef database update 0 --context $contextName --verbose
            }
        }
        else {
            Write-Log "Applying migrations for $serviceName..." "INFO"
            
            if ($Target) {
                dotnet ef database update $Target --context $contextName --verbose
            } else {
                dotnet ef database update --context $contextName --verbose
            }
            
            Write-Log "Successfully applied migrations for $serviceName" "SUCCESS"
        }
        
        return $true
    }
    catch {
        Write-Log "Failed to process migrations for $serviceName`: $($_.Exception.Message)" "ERROR"
        return $false
    }
    finally {
        Pop-Location
        Remove-Item Env:ConnectionStrings__DefaultConnection -ErrorAction SilentlyContinue
    }
}

function Backup-Database {
    param([string]$ConnectionString, [string]$Environment)
    
    if ($Environment -eq "production") {
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $backupFile = "backup_${Environment}_${timestamp}.sql"
        
        Write-Log "Creating database backup: $backupFile" "INFO"
        
        # Extract connection details
        $connectionParts = $ConnectionString -split ";"
        $host = ($connectionParts | Where-Object { $_ -like "Host=*" }) -replace "Host=", ""
        $database = ($connectionParts | Where-Object { $_ -like "Database=*" }) -replace "Database=", ""
        $username = ($connectionParts | Where-Object { $_ -like "Username=*" }) -replace "Username=", ""
        
        # Create backup using pg_dump
        $env:PGPASSWORD = $env:PRODUCTION_DB_PASSWORD
        pg_dump -h $host -U $username -d $database -f $backupFile
        
        if ($LASTEXITCODE -eq 0) {
            Write-Log "Database backup created successfully: $backupFile" "SUCCESS"
        } else {
            Write-Log "Database backup failed" "ERROR"
            throw "Database backup failed"
        }
    }
}

# Main execution
try {
    Write-Log "Starting database migration process..." "INFO"
    Write-Log "Environment: $Environment" "INFO"
    Write-Log "Service: $Service" "INFO"
    Write-Log "Dry Run: $DryRun" "INFO"
    Write-Log "Rollback: $Rollback" "INFO"
    
    if (-not $connectionStrings.ContainsKey($Environment)) {
        throw "Unknown environment: $Environment. Valid environments: $($connectionStrings.Keys -join ', ')"
    }
    
    $connectionString = $connectionStrings[$Environment]
    
    # Create backup for production
    if ($Environment -eq "production" -and -not $DryRun -and -not $Rollback) {
        Backup-Database $connectionString $Environment
    }
    
    $servicesToProcess = if ($Service -eq "all") { $services } else { $services | Where-Object { $_.Name -eq $Service } }
    
    if (-not $servicesToProcess) {
        throw "Service '$Service' not found. Available services: $($services.Name -join ', ')"
    }
    
    $allSuccessful = $true
    
    foreach ($serviceConfig in $servicesToProcess) {
        $success = Run-Migration $serviceConfig $connectionString $DryRun $Rollback $TargetMigration
        if (-not $success) {
            $allSuccessful = $false
        }
    }
    
    if ($allSuccessful) {
        Write-Log "All database operations completed successfully!" "SUCCESS"
        exit 0
    } else {
        Write-Log "Some database operations failed. Check logs above." "ERROR"
        exit 1
    }
}
catch {
    Write-Log "Migration process failed: $($_.Exception.Message)" "ERROR"
    exit 1
}
