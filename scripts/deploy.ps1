# TOSS ERP Deployment Script
# Handles deployment to different environments with proper validation and rollback capabilities

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("development", "staging", "production")]
    [string]$Environment,
    
    [Parameter(Mandatory=$false)]
    [string]$Version = "latest",
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipTests = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipMigrations = $false,
    
    [Parameter(Mandatory=$false)]
    [string]$Namespace = "",
    
    [Parameter(Mandatory=$false)]
    [int]$TimeoutMinutes = 15
)

$ErrorActionPreference = "Stop"

# Configuration
$environments = @{
    "development" = @{
        Namespace = "toss-erp-dev"
        KustomizePath = "k8s/overlays/development"
        RequiresApproval = $false
        HealthCheckUrls = @(
            "http://dev-api.toss-erp.local/health",
            "http://dev-app.toss-erp.local/health"
        )
    }
    "staging" = @{
        Namespace = "toss-erp-staging"
        KustomizePath = "k8s/overlays/staging"
        RequiresApproval = $true
        HealthCheckUrls = @(
            "https://staging-api.toss-erp.com/health",
            "https://staging-app.toss-erp.com/health"
        )
    }
    "production" = @{
        Namespace = "toss-erp-production"
        KustomizePath = "k8s/overlays/production"
        RequiresApproval = $true
        HealthCheckUrls = @(
            "https://api.toss-erp.com/health",
            "https://app.toss-erp.com/health"
        )
    }
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
    Write-Log "Checking prerequisites..." "INFO"
    
    # Check kubectl
    try {
        kubectl version --client --output=json | Out-Null
        Write-Log "✓ kubectl is available" "SUCCESS"
    }
    catch {
        throw "kubectl is not available or not configured properly"
    }
    
    # Check kustomize
    try {
        kubectl kustomize --help | Out-Null
        Write-Log "✓ kustomize is available" "SUCCESS"
    }
    catch {
        throw "kustomize is not available"
    }
    
    # Check cluster connectivity
    try {
        kubectl cluster-info --request-timeout=10s | Out-Null
        Write-Log "✓ Kubernetes cluster is accessible" "SUCCESS"
    }
    catch {
        throw "Cannot connect to Kubernetes cluster"
    }
    
    # Check if namespace exists
    $targetNamespace = if ($Namespace) { $Namespace } else { $environments[$Environment].Namespace }
    try {
        kubectl get namespace $targetNamespace | Out-Null
        Write-Log "✓ Namespace '$targetNamespace' exists" "SUCCESS"
    }
    catch {
        Write-Log "Creating namespace '$targetNamespace'..." "INFO"
        kubectl create namespace $targetNamespace
        Write-Log "✓ Namespace '$targetNamespace' created" "SUCCESS"
    }
}

function Get-CurrentDeployment {
    param([string]$Namespace)
    
    try {
        $deployments = kubectl get deployments -n $Namespace -o json | ConvertFrom-Json
        $currentState = @{}
        
        foreach ($deployment in $deployments.items) {
            $currentState[$deployment.metadata.name] = @{
                Replicas = $deployment.status.readyReplicas
                Image = $deployment.spec.template.spec.containers[0].image
                Generation = $deployment.metadata.generation
            }
        }
        
        return $currentState
    }
    catch {
        Write-Log "Could not get current deployment state: $($_.Exception.Message)" "WARN"
        return @{}
    }
}

function Request-Approval {
    param([string]$Environment, [string]$Version)
    
    Write-Log "Deployment to $Environment requires approval." "WARN"
    Write-Log "Environment: $Environment" "INFO"
    Write-Log "Version: $Version" "INFO"
    Write-Log "Namespace: $($environments[$Environment].Namespace)" "INFO"
    
    do {
        $response = Read-Host "Do you want to proceed? (yes/no)"
    } while ($response -notin @("yes", "no", "y", "n"))
    
    if ($response -in @("no", "n")) {
        throw "Deployment cancelled by user"
    }
    
    Write-Log "Deployment approved by user" "SUCCESS"
}

function Run-PreDeploymentTests {
    Write-Log "Running pre-deployment tests..." "INFO"
    
    try {
        # Run unit tests
        Write-Log "Running unit tests..." "INFO"
        dotnet test --configuration Release --no-build --verbosity quiet
        
        # Run integration tests if they exist
        if (Test-Path "tests") {
            Write-Log "Running integration tests..." "INFO"
            dotnet test tests --configuration Release --verbosity quiet
        }
        
        Write-Log "✓ All tests passed" "SUCCESS"
    }
    catch {
        throw "Pre-deployment tests failed: $($_.Exception.Message)"
    }
}

function Run-DatabaseMigrations {
    param([string]$Environment, [string]$Namespace)
    
    Write-Log "Running database migrations for $Environment..." "INFO"
    
    try {
        # Create migration job
        $migrationJob = @"
apiVersion: batch/v1
kind: Job
metadata:
  name: db-migration-$(Get-Date -Format 'yyyyMMdd-HHmmss')
  namespace: $Namespace
spec:
  template:
    spec:
      restartPolicy: Never
      containers:
      - name: migration
        image: ghcr.io/toss-online-services/toss-erp/migration:$Version
        env:
        - name: ENVIRONMENT
          value: "$Environment"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: toss-secrets
              key: database-connection-string
  backoffLimit: 3
"@
        
        $migrationJob | kubectl apply -f -
        
        # Wait for migration to complete
        $jobName = "db-migration-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        Write-Log "Waiting for migration job '$jobName' to complete..." "INFO"
        
        $timeout = 300 # 5 minutes
        $elapsed = 0
        $interval = 10
        
        do {
            Start-Sleep $interval
            $elapsed += $interval
            
            $jobStatus = kubectl get job $jobName -n $Namespace -o jsonpath='{.status.conditions[0].type}'
            
            if ($jobStatus -eq "Complete") {
                Write-Log "✓ Database migrations completed successfully" "SUCCESS"
                break
            }
            elseif ($jobStatus -eq "Failed") {
                throw "Database migration job failed"
            }
            
            Write-Log "Migration in progress... ($elapsed/$timeout seconds)" "INFO"
        } while ($elapsed -lt $timeout)
        
        if ($elapsed -ge $timeout) {
            throw "Database migration timed out"
        }
    }
    catch {
        throw "Database migration failed: $($_.Exception.Message)"
    }
}

function Deploy-Application {
    param([string]$Environment, [string]$Version, [string]$Namespace, [bool]$IsDryRun)
    
    $kustomizePath = $environments[$Environment].KustomizePath
    $targetNamespace = if ($Namespace) { $Namespace } else { $environments[$Environment].Namespace }
    
    Write-Log "Deploying to $Environment environment..." "INFO"
    Write-Log "Kustomize path: $kustomizePath" "INFO"
    Write-Log "Target namespace: $targetNamespace" "INFO"
    Write-Log "Version: $Version" "INFO"
    
    try {
        # Update image tags if version is specified and not latest
        if ($Version -ne "latest") {
            Write-Log "Updating image tags to version $Version..." "INFO"
            
            # Create a temporary kustomization patch
            $imagePatch = @"
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization

images:
- name: ghcr.io/toss-online-services/toss-erp/gateway
  newTag: $Version
- name: ghcr.io/toss-online-services/toss-erp/identity
  newTag: $Version
- name: ghcr.io/toss-online-services/toss-erp/ai
  newTag: $Version
- name: ghcr.io/toss-online-services/toss-erp/web
  newTag: $Version
- name: ghcr.io/toss-online-services/toss-erp/mobile
  newTag: $Version
"@
            $imagePatch | Out-File -FilePath "$kustomizePath/version-patch.yaml" -Encoding UTF8
        }
        
        if ($IsDryRun) {
            Write-Log "DRY RUN: Would apply the following manifests:" "INFO"
            kubectl kustomize $kustomizePath
            return
        }
        
        # Apply the manifests
        Write-Log "Applying Kubernetes manifests..." "INFO"
        kubectl apply -k $kustomizePath
        
        # Wait for rollout to complete
        $deployments = @("gateway", "identity", "ai", "web", "mobile")
        
        foreach ($deployment in $deployments) {
            $deploymentName = if ($Environment -eq "production") { $deployment } else { "$Environment-$deployment" }
            
            Write-Log "Waiting for rollout of $deploymentName..." "INFO"
            kubectl rollout status deployment/$deploymentName -n $targetNamespace --timeout=${TimeoutMinutes}m
            
            if ($LASTEXITCODE -ne 0) {
                throw "Rollout failed for deployment: $deploymentName"
            }
        }
        
        Write-Log "✓ All deployments rolled out successfully" "SUCCESS"
        
        # Clean up version patch file
        if ($Version -ne "latest" -and (Test-Path "$kustomizePath/version-patch.yaml")) {
            Remove-Item "$kustomizePath/version-patch.yaml" -Force
        }
    }
    catch {
        throw "Deployment failed: $($_.Exception.Message)"
    }
}

function Test-HealthChecks {
    param([string]$Environment)
    
    $healthCheckUrls = $environments[$Environment].HealthCheckUrls
    
    Write-Log "Running health checks..." "INFO"
    
    foreach ($url in $healthCheckUrls) {
        Write-Log "Checking health endpoint: $url" "INFO"
        
        $maxRetries = 10
        $retryCount = 0
        $success = $false
        
        do {
            try {
                $response = Invoke-RestMethod -Uri $url -Method Get -TimeoutSec 10
                Write-Log "✓ Health check passed for $url" "SUCCESS"
                $success = $true
                break
            }
            catch {
                $retryCount++
                if ($retryCount -lt $maxRetries) {
                    Write-Log "Health check failed for $url, retrying in 30 seconds... ($retryCount/$maxRetries)" "WARN"
                    Start-Sleep 30
                } else {
                    Write-Log "Health check failed for $url after $maxRetries attempts" "ERROR"
                }
            }
        } while ($retryCount -lt $maxRetries)
        
        if (-not $success) {
            throw "Health checks failed for $url"
        }
    }
    
    Write-Log "✓ All health checks passed" "SUCCESS"
}

# Main execution
try {
    Write-Log "Starting deployment process..." "INFO"
    Write-Log "Environment: $Environment" "INFO"
    Write-Log "Version: $Version" "INFO"
    Write-Log "Dry Run: $DryRun" "INFO"
    
    # Check prerequisites
    Test-Prerequisites
    
    # Get target namespace
    $targetNamespace = if ($Namespace) { $Namespace } else { $environments[$Environment].Namespace }
    
    # Get current deployment state for rollback
    $previousState = Get-CurrentDeployment $targetNamespace
    
    # Request approval if required
    if ($environments[$Environment].RequiresApproval -and -not $DryRun) {
        Request-Approval $Environment $Version
    }
    
    # Run pre-deployment tests
    if (-not $SkipTests -and -not $DryRun) {
        Run-PreDeploymentTests
    }
    
    # Run database migrations
    if (-not $SkipMigrations -and -not $DryRun) {
        Run-DatabaseMigrations $Environment $targetNamespace
    }
    
    # Deploy application
    Deploy-Application $Environment $Version $targetNamespace $DryRun
    
    # Run health checks
    if (-not $DryRun) {
        Start-Sleep 30  # Wait for services to start
        Test-HealthChecks $Environment
    }
    
    Write-Log "Deployment completed successfully!" "SUCCESS"
    
    if (-not $DryRun) {
        Write-Log "Deployment summary:" "INFO"
        Write-Log "  Environment: $Environment" "INFO"
        Write-Log "  Namespace: $targetNamespace" "INFO"
        Write-Log "  Version: $Version" "INFO"
        Write-Log "  Timestamp: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" "INFO"
    }
}
catch {
    Write-Log "Deployment failed: $($_.Exception.Message)" "ERROR"
    
    if (-not $DryRun -and $previousState.Count -gt 0) {
        Write-Log "Consider running rollback if needed..." "WARN"
        Write-Log "Previous deployment state was captured for potential rollback" "INFO"
    }
    
    exit 1
}
