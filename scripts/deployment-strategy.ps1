# TOSS ERP Deployment Strategy Management Script
# Handles different deployment strategies: rolling, blue/green, canary

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("rolling", "blue-green", "canary")]
    [string]$Strategy,
    
    [Parameter(Mandatory=$true)]
    [ValidateSet("development", "staging", "production")]
    [string]$Environment,
    
    [Parameter(Mandatory=$false)]
    [string]$Service = "all",
    
    [Parameter(Mandatory=$false)]
    [string]$Version = "latest",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("deploy", "promote", "rollback", "status")]
    [string]$Action = "deploy",
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun = $false,
    
    [Parameter(Mandatory=$false)]
    [int]$TimeoutMinutes = 15
)

$ErrorActionPreference = "Stop"

# Configuration
$services = @("gateway", "identity", "ai", "web", "mobile")
$namespaces = @{
    "development" = "toss-erp-dev"
    "staging" = "toss-erp-staging"
    "production" = "toss-erp-production"
}

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

function Test-Prerequisites {
    Write-Log "Checking prerequisites for $Strategy deployment..." "INFO"
    
    # Check kubectl
    try {
        kubectl version --client --output=json | Out-Null
        Write-Log "✓ kubectl is available" "SUCCESS"
    }
    catch {
        throw "kubectl is not available"
    }
    
    # Check if Argo Rollouts is available (for canary and blue/green)
    if ($Strategy -in @("canary", "blue-green")) {
        try {
            kubectl get crd rollouts.argoproj.io | Out-Null
            Write-Log "✓ Argo Rollouts CRD is available" "SUCCESS"
        }
        catch {
            throw "Argo Rollouts is not installed. Please install Argo Rollouts for advanced deployment strategies."
        }
    }
    
    # Check cluster connectivity
    try {
        kubectl cluster-info --request-timeout=10s | Out-Null
        Write-Log "✓ Kubernetes cluster is accessible" "SUCCESS"
    }
    catch {
        throw "Cannot connect to Kubernetes cluster"
    }
}

function Deploy-RollingUpdate {
    param([string]$ServiceName, [string]$Namespace, [string]$Version, [bool]$IsDryRun)
    
    Write-Log "Deploying $ServiceName using rolling update strategy..." "INFO"
    
    $deploymentName = if ($Environment -eq "production") { $ServiceName } else { "$Environment-$ServiceName" }
    $imageName = "ghcr.io/toss-online-services/toss-erp/$ServiceName`:$Version"
    
    if ($IsDryRun) {
        Write-Log "DRY RUN: Would update deployment $deploymentName to $imageName" "INFO"
        return
    }
    
    try {
        # Update deployment image
        kubectl set image deployment/$deploymentName $ServiceName=$imageName -n $Namespace
        
        # Wait for rollout to complete
        Write-Log "Waiting for rollout to complete..." "INFO"
        kubectl rollout status deployment/$deploymentName -n $Namespace --timeout=${TimeoutMinutes}m
        
        if ($LASTEXITCODE -eq 0) {
            Write-Log "✓ Rolling update completed successfully for $ServiceName" "SUCCESS"
        } else {
            throw "Rolling update failed for $ServiceName"
        }
    }
    catch {
        throw "Failed to deploy $ServiceName with rolling update: $($_.Exception.Message)"
    }
}

function Deploy-BlueGreen {
    param([string]$ServiceName, [string]$Namespace, [string]$Version, [bool]$IsDryRun)
    
    Write-Log "Deploying $ServiceName using blue/green strategy..." "INFO"
    
    $rolloutName = "$ServiceName-rollout"
    $imageName = "ghcr.io/toss-online-services/toss-erp/$ServiceName`:$Version"
    
    if ($IsDryRun) {
        Write-Log "DRY RUN: Would deploy $rolloutName to $imageName using blue/green strategy" "INFO"
        return
    }
    
    try {
        # Check if rollout exists
        $rolloutExists = $false
        try {
            kubectl get rollout $rolloutName -n $Namespace | Out-Null
            $rolloutExists = $true
        }
        catch {
            Write-Log "Rollout $rolloutName does not exist, creating..." "INFO"
        }
        
        if (-not $rolloutExists) {
            # Create rollout from template
            $rolloutManifest = Get-Content "k8s/base/deployment-strategy.yaml" -Raw
            $rolloutManifest = $rolloutManifest -replace "gateway-rollout", "$rolloutName"
            $rolloutManifest = $rolloutManifest -replace "ghcr.io/toss-online-services/toss-erp/gateway:latest", $imageName
            
            $rolloutManifest | kubectl apply -f - -n $Namespace
        } else {
            # Update existing rollout
            kubectl patch rollout $rolloutName -n $Namespace -p "{`"spec`":{`"template`":{`"spec`":{`"containers`":[{`"name`":`"$ServiceName`",`"image`":`"$imageName`"}]}}}}" --type=merge
        }
        
        # Wait for rollout to complete
        Write-Log "Waiting for blue/green rollout to complete..." "INFO"
        kubectl argo rollouts get rollout $rolloutName -n $Namespace --watch --timeout ${TimeoutMinutes}m
        
        Write-Log "✓ Blue/Green deployment completed for $ServiceName" "SUCCESS"
        Write-Log "Use 'promote' action to switch traffic to the new version" "INFO"
    }
    catch {
        throw "Failed to deploy $ServiceName with blue/green strategy: $($_.Exception.Message)"
    }
}

function Deploy-Canary {
    param([string]$ServiceName, [string]$Namespace, [string]$Version, [bool]$IsDryRun)
    
    Write-Log "Deploying $ServiceName using canary strategy..." "INFO"
    
    $rolloutName = "$ServiceName-rollout"
    $imageName = "ghcr.io/toss-online-services/toss-erp/$ServiceName`:$Version"
    
    if ($IsDryRun) {
        Write-Log "DRY RUN: Would deploy $rolloutName to $imageName using canary strategy" "INFO"
        return
    }
    
    try {
        # Update rollout with canary strategy
        kubectl patch rollout $rolloutName -n $Namespace -p "{`"spec`":{`"template`":{`"spec`":{`"containers`":[{`"name`":`"$ServiceName`",`"image`":`"$imageName`"}]}}}}" --type=merge
        
        # Start canary deployment
        Write-Log "Starting canary deployment..." "INFO"
        kubectl argo rollouts get rollout $rolloutName -n $Namespace --watch --timeout ${TimeoutMinutes}m
        
        Write-Log "✓ Canary deployment started for $ServiceName" "SUCCESS"
        Write-Log "Monitor metrics and use 'promote' action to continue or 'rollback' to abort" "INFO"
    }
    catch {
        throw "Failed to deploy $ServiceName with canary strategy: $($_.Exception.Message)"
    }
}

function Promote-Deployment {
    param([string]$ServiceName, [string]$Namespace)
    
    Write-Log "Promoting $ServiceName deployment..." "INFO"
    
    $rolloutName = "$ServiceName-rollout"
    
    try {
        kubectl argo rollouts promote $rolloutName -n $Namespace
        Write-Log "✓ Deployment promoted for $ServiceName" "SUCCESS"
    }
    catch {
        throw "Failed to promote deployment for $ServiceName`: $($_.Exception.Message)"
    }
}

function Rollback-Deployment {
    param([string]$ServiceName, [string]$Namespace, [string]$Strategy)
    
    Write-Log "Rolling back $ServiceName deployment..." "INFO"
    
    try {
        if ($Strategy -eq "rolling") {
            $deploymentName = if ($Environment -eq "production") { $ServiceName } else { "$Environment-$ServiceName" }
            kubectl rollout undo deployment/$deploymentName -n $Namespace
        } else {
            $rolloutName = "$ServiceName-rollout"
            kubectl argo rollouts abort $rolloutName -n $Namespace
            kubectl argo rollouts undo $rolloutName -n $Namespace
        }
        
        Write-Log "✓ Rollback completed for $ServiceName" "SUCCESS"
    }
    catch {
        throw "Failed to rollback deployment for $ServiceName`: $($_.Exception.Message)"
    }
}

function Get-DeploymentStatus {
    param([string]$ServiceName, [string]$Namespace, [string]$Strategy)
    
    Write-Log "Getting deployment status for $ServiceName..." "INFO"
    
    try {
        if ($Strategy -eq "rolling") {
            $deploymentName = if ($Environment -eq "production") { $ServiceName } else { "$Environment-$ServiceName" }
            kubectl get deployment $deploymentName -n $Namespace -o wide
            kubectl rollout status deployment/$deploymentName -n $Namespace
        } else {
            $rolloutName = "$ServiceName-rollout"
            kubectl argo rollouts get rollout $rolloutName -n $Namespace
            kubectl argo rollouts status $rolloutName -n $Namespace
        }
    }
    catch {
        Write-Log "Failed to get status for $ServiceName`: $($_.Exception.Message)" "ERROR"
    }
}

function Process-Service {
    param([string]$ServiceName)
    
    $namespace = $namespaces[$Environment]
    
    Write-Log "Processing $ServiceName in $Environment environment..." "INFO"
    
    switch ($Action) {
        "deploy" {
            switch ($Strategy) {
                "rolling" {
                    Deploy-RollingUpdate $ServiceName $namespace $Version $DryRun
                }
                "blue-green" {
                    Deploy-BlueGreen $ServiceName $namespace $Version $DryRun
                }
                "canary" {
                    Deploy-Canary $ServiceName $namespace $Version $DryRun
                }
            }
        }
        "promote" {
            if ($Strategy -in @("blue-green", "canary")) {
                Promote-Deployment $ServiceName $namespace
            } else {
                Write-Log "Promote action is only available for blue-green and canary strategies" "WARN"
            }
        }
        "rollback" {
            Rollback-Deployment $ServiceName $namespace $Strategy
        }
        "status" {
            Get-DeploymentStatus $ServiceName $namespace $Strategy
        }
    }
}

# Main execution
try {
    Write-Log "Starting deployment strategy management..." "INFO"
    Write-Log "Strategy: $Strategy" "INFO"
    Write-Log "Environment: $Environment" "INFO"
    Write-Log "Service: $Service" "INFO"
    Write-Log "Version: $Version" "INFO"
    Write-Log "Action: $Action" "INFO"
    
    # Check prerequisites
    Test-Prerequisites
    
    # Process services
    $servicesToProcess = if ($Service -eq "all") { $services } else { @($Service) }
    
    foreach ($serviceName in $servicesToProcess) {
        if ($serviceName -notin $services) {
            Write-Log "Unknown service: $serviceName. Available services: $($services -join ', ')" "WARN"
            continue
        }
        
        Process-Service $serviceName
    }
    
    Write-Log "Deployment strategy management completed successfully!" "SUCCESS"
}
catch {
    Write-Log "Deployment strategy management failed: $($_.Exception.Message)" "ERROR"
    exit 1
}
