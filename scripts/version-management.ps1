# TOSS ERP Version Management Script
# Handles semantic versioning and container registry operations

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("bump", "tag", "push", "list", "cleanup")]
    [string]$Action,
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("major", "minor", "patch", "prerelease")]
    [string]$BumpType = "patch",
    
    [Parameter(Mandatory=$false)]
    [string]$Version = "",
    
    [Parameter(Mandatory=$false)]
    [string]$Registry = "ghcr.io/toss-online-services/toss-erp",
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun = $false,
    
    [Parameter(Mandatory=$false)]
    [int]$KeepVersions = 10
)

$ErrorActionPreference = "Stop"

# Configuration
$services = @("gateway", "identity", "ai", "web", "mobile")
$versionFile = "VERSION"
$changelogFile = "CHANGELOG.md"

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $(
        switch ($Level) {
            "ERROR" { "Red" }
            "WARN" { "Yellow" }
            "SUCCESS" { "Green" }
            "INFO" { "Cyan" }
            default { "White" }
        }
    )
}

function Get-CurrentVersion {
    if (Test-Path $versionFile) {
        $version = Get-Content $versionFile -Raw
        return $version.Trim()
    } else {
        Write-Log "Version file not found, starting with 0.1.0" "INFO"
        return "0.1.0"
    }
}

function Set-CurrentVersion {
    param([string]$NewVersion)
    
    $NewVersion | Set-Content $versionFile -NoNewline
    Write-Log "Version updated to $NewVersion" "SUCCESS"
}

function Get-NextVersion {
    param([string]$CurrentVersion, [string]$BumpType)
    
    if (-not ($CurrentVersion -match '^(\d+)\.(\d+)\.(\d+)(?:-(.+))?$')) {
        throw "Invalid version format: $CurrentVersion"
    }
    
    $major = [int]$matches[1]
    $minor = [int]$matches[2]
    $patch = [int]$matches[3]
    $prerelease = $matches[4]
    
    switch ($BumpType) {
        "major" {
            $major++
            $minor = 0
            $patch = 0
            $prerelease = $null
        }
        "minor" {
            $minor++
            $patch = 0
            $prerelease = $null
        }
        "patch" {
            $patch++
            $prerelease = $null
        }
        "prerelease" {
            if ($prerelease) {
                if ($prerelease -match '^(.+)\.(\d+)$') {
                    $prereleaseBase = $matches[1]
                    $prereleaseNumber = [int]$matches[2] + 1
                    $prerelease = "$prereleaseBase.$prereleaseNumber"
                } else {
                    $prerelease = "$prerelease.1"
                }
            } else {
                $prerelease = "alpha.1"
            }
        }
    }
    
    $newVersion = "$major.$minor.$patch"
    if ($prerelease) {
        $newVersion += "-$prerelease"
    }
    
    return $newVersion
}

function Update-Changelog {
    param([string]$Version, [string]$PreviousVersion)
    
    $date = Get-Date -Format "yyyy-MM-dd"
    $changelogEntry = @"
## [$Version] - $date

### Added
- New features and enhancements

### Changed
- Updated dependencies and configurations

### Fixed
- Bug fixes and improvements

### Security
- Security updates and patches

"@
    
    if (Test-Path $changelogFile) {
        $existingContent = Get-Content $changelogFile -Raw
        $newContent = "# Changelog`n`nAll notable changes to this project will be documented in this file.`n`n$changelogEntry`n$existingContent"
        $newContent | Set-Content $changelogFile
    } else {
        $initialContent = @"
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

$changelogEntry
"@
        $initialContent | Set-Content $changelogFile
    }
    
    Write-Log "Changelog updated for version $Version" "SUCCESS"
}

function Create-GitTag {
    param([string]$Version, [bool]$IsDryRun)
    
    if ($IsDryRun) {
        Write-Log "DRY RUN: Would create Git tag v$Version" "INFO"
        return
    }
    
    try {
        # Check if tag already exists
        $existingTag = git tag -l "v$Version"
        if ($existingTag) {
            throw "Tag v$Version already exists"
        }
        
        # Create annotated tag
        git tag -a "v$Version" -m "Release version $Version"
        Write-Log "Created Git tag v$Version" "SUCCESS"
        
        # Push tag to remote
        git push origin "v$Version"
        Write-Log "Pushed Git tag v$Version to remote" "SUCCESS"
    }
    catch {
        throw "Failed to create Git tag: $($_.Exception.Message)"
    }
}

function Build-And-Push-Images {
    param([string]$Version, [string]$Registry, [bool]$IsDryRun)
    
    Write-Log "Building and pushing container images for version $Version..." "INFO"
    
    foreach ($service in $services) {
        $imageName = "$Registry/$service"
        $imageTag = "$imageName:$Version"
        $latestTag = "$imageName:latest"
        
        Write-Log "Processing service: $service" "INFO"
        
        if ($IsDryRun) {
            Write-Log "DRY RUN: Would build and push $imageTag" "INFO"
            continue
        }
        
        try {
            # Determine Dockerfile path
            $dockerfilePath = switch ($service) {
                "gateway" { "src/Gateway/Dockerfile" }
                "identity" { "src/Services/identity/Dockerfile" }
                "ai" { "src/Services/ai/Dockerfile" }
                "web" { "src/clients/web/Dockerfile" }
                "mobile" { "src/clients/mobile/Dockerfile" }
            }
            
            # Build image
            Write-Log "Building image: $imageTag" "INFO"
            docker build -t $imageTag -t $latestTag -f $dockerfilePath .
            
            if ($LASTEXITCODE -ne 0) {
                throw "Docker build failed for $service"
            }
            
            # Push versioned tag
            Write-Log "Pushing image: $imageTag" "INFO"
            docker push $imageTag
            
            if ($LASTEXITCODE -ne 0) {
                throw "Docker push failed for $imageTag"
            }
            
            # Push latest tag
            Write-Log "Pushing image: $latestTag" "INFO"
            docker push $latestTag
            
            if ($LASTEXITCODE -ne 0) {
                throw "Docker push failed for $latestTag"
            }
            
            Write-Log "Successfully built and pushed $service:$Version" "SUCCESS"
        }
        catch {
            throw "Failed to build/push $service: $($_.Exception.Message)"
        }
    }
}

function List-Versions {
    param([string]$Registry)
    
    Write-Log "Listing available versions..." "INFO"
    
    foreach ($service in $services) {
        Write-Log "Service: $service" "INFO"
        
        try {
            # List tags using Docker Hub API or registry API
            # For GitHub Container Registry
            $apiUrl = "https://api.github.com/orgs/toss-online-services/packages/container/toss-erp%2F$service/versions"
            
            $headers = @{
                "Accept" = "application/vnd.github.v3+json"
                "Authorization" = "token $env:GITHUB_TOKEN"
            }
            
            $response = Invoke-RestMethod -Uri $apiUrl -Headers $headers -Method Get
            
            $versions = $response | ForEach-Object { $_.metadata.container.tags } | Sort-Object -Descending
            
            $versions | Select-Object -First 10 | ForEach-Object {
                Write-Log "  - $_" "INFO"
            }
        }
        catch {
            Write-Log "Could not list versions for $service`: $($_.Exception.Message)" "WARN"
        }
    }
}

function Cleanup-OldVersions {
    param([string]$Registry, [int]$KeepVersions, [bool]$IsDryRun)
    
    Write-Log "Cleaning up old versions (keeping latest $KeepVersions)..." "INFO"
    
    foreach ($service in $services) {
        Write-Log "Processing service: $service" "INFO"
        
        try {
            # Get all versions for the service
            $apiUrl = "https://api.github.com/orgs/toss-online-services/packages/container/toss-erp%2F$service/versions"
            
            $headers = @{
                "Accept" = "application/vnd.github.v3+json"
                "Authorization" = "token $env:GITHUB_TOKEN"
            }
            
            $response = Invoke-RestMethod -Uri $apiUrl -Headers $headers -Method Get
            
            # Sort by creation date and skip the ones to keep
            $versionsToDelete = $response | 
                Sort-Object created_at -Descending | 
                Select-Object -Skip $KeepVersions
            
            foreach ($version in $versionsToDelete) {
                if ($IsDryRun) {
                    Write-Log "DRY RUN: Would delete version $($version.id) of $service" "INFO"
                } else {
                    Write-Log "Deleting version $($version.id) of $service..." "INFO"
                    
                    $deleteUrl = "https://api.github.com/orgs/toss-online-services/packages/container/toss-erp%2F$service/versions/$($version.id)"
                    Invoke-RestMethod -Uri $deleteUrl -Headers $headers -Method Delete
                    
                    Write-Log "Deleted version $($version.id) of $service" "SUCCESS"
                }
            }
        }
        catch {
            Write-Log "Could not cleanup versions for $service`: $($_.Exception.Message)" "WARN"
        }
    }
}

# Main execution
try {
    Write-Log "Starting version management process..." "INFO"
    Write-Log "Action: $Action" "INFO"
    
    switch ($Action) {
        "bump" {
            $currentVersion = Get-CurrentVersion
            $newVersion = Get-NextVersion $currentVersion $BumpType
            
            Write-Log "Current version: $currentVersion" "INFO"
            Write-Log "New version: $newVersion" "INFO"
            
            if ($DryRun) {
                Write-Log "DRY RUN: Would update version to $newVersion" "INFO"
            } else {
                Set-CurrentVersion $newVersion
                Update-Changelog $newVersion $currentVersion
                
                # Commit version changes
                git add $versionFile $changelogFile
                git commit -m "chore: bump version to $newVersion"
                
                Write-Log "Version bumped to $newVersion" "SUCCESS"
            }
        }
        
        "tag" {
            $versionToTag = if ($Version) { $Version } else { Get-CurrentVersion }
            Create-GitTag $versionToTag $DryRun
        }
        
        "push" {
            $versionToPush = if ($Version) { $Version } else { Get-CurrentVersion }
            Build-And-Push-Images $versionToPush $Registry $DryRun
        }
        
        "list" {
            List-Versions $Registry
        }
        
        "cleanup" {
            Cleanup-OldVersions $Registry $KeepVersions $DryRun
        }
    }
    
    Write-Log "Version management completed successfully!" "SUCCESS"
}
catch {
    Write-Log "Version management failed: $($_.Exception.Message)" "ERROR"
    exit 1
}
