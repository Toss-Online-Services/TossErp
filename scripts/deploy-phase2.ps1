# TOSS Platform Phase 2 Deployment Script
# Deploy DevOps & Infrastructure + AI Integration Platform

param(
    [Parameter(Mandatory=$true)]
    [string]$Environment = "production",
    
    [Parameter(Mandatory=$true)]
    [string]$SubscriptionId,
    
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName = "toss-$Environment-rg",
    
    [Parameter(Mandatory=$true)]
    [string]$Location = "eastus",
    
    [Parameter(Mandatory=$false)]
    [string]$TerraformBackendStorageAccount,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipInfrastructure = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipKubernetes = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipGitOps = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun = $false
)

# Error handling
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

# Logging function
function Write-Log {
    param(
        [string]$Message,
        [string]$Level = "INFO"
    )
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $(
        switch ($Level) {
            "ERROR" { "Red" }
            "WARNING" { "Yellow" }
            "SUCCESS" { "Green" }
            default { "White" }
        }
    )
}

# Initialize deployment
Write-Log "Starting TOSS Platform Phase 2 Deployment" "SUCCESS"
Write-Log "Environment: $Environment"
Write-Log "Subscription: $SubscriptionId"
Write-Log "Resource Group: $ResourceGroupName"
Write-Log "Location: $Location"

# Validate prerequisites
Write-Log "Validating prerequisites..."

# Check Azure CLI
try {
    $azVersion = az version --output json | ConvertFrom-Json
    Write-Log "Azure CLI version: $($azVersion.'azure-cli')"
} catch {
    Write-Log "Azure CLI not found. Please install Azure CLI." "ERROR"
    exit 1
}

# Check Terraform
try {
    $tfVersion = terraform version -json | ConvertFrom-Json
    Write-Log "Terraform version: $($tfVersion.terraform_version)"
} catch {
    Write-Log "Terraform not found. Please install Terraform." "ERROR"
    exit 1
}

# Check kubectl
try {
    $kubectlVersion = kubectl version --client=true -o json | ConvertFrom-Json
    Write-Log "kubectl version: $($kubectlVersion.clientVersion.gitVersion)"
} catch {
    Write-Log "kubectl not found. Please install kubectl." "ERROR"
    exit 1
}

# Check Helm
try {
    $helmVersion = helm version --short
    Write-Log "Helm version: $helmVersion"
} catch {
    Write-Log "Helm not found. Please install Helm." "ERROR"
    exit 1
}

# Azure authentication
Write-Log "Authenticating with Azure..."
az login --output none
az account set --subscription $SubscriptionId

# Verify subscription
$currentSub = az account show --query "id" -o tsv
if ($currentSub -ne $SubscriptionId) {
    Write-Log "Failed to set subscription to $SubscriptionId" "ERROR"
    exit 1
}

Write-Log "Successfully authenticated with subscription: $SubscriptionId" "SUCCESS"

# Phase 1: Infrastructure Deployment
if (-not $SkipInfrastructure) {
    Write-Log "Phase 1: Deploying Infrastructure with Terraform" "SUCCESS"
    
    Push-Location "$PSScriptRoot\..\infra\terraform"
    
    try {
        # Initialize Terraform
        Write-Log "Initializing Terraform..."
        if ($TerraformBackendStorageAccount) {
            terraform init `
                -backend-config="storage_account_name=$TerraformBackendStorageAccount" `
                -backend-config="container_name=tfstate" `
                -backend-config="key=$Environment/terraform.tfstate"
        } else {
            terraform init
        }
        
        # Create terraform.tfvars
        Write-Log "Creating terraform.tfvars..."
        $tfVars = @"
environment = "$Environment"
location = "$Location"
resource_group_name = "$ResourceGroupName"
subscription_id = "$SubscriptionId"
tenant_id = "$(az account show --query tenantId -o tsv)"

# Kubernetes configuration
kubernetes_version = "1.28"
node_count = 3
node_vm_size = "Standard_D4s_v3"
ai_node_count = 2
ai_node_vm_size = "Standard_NC6s_v3"

# Database configuration
database_admin_username = "tossadmin"
database_sku_name = "GP_Standard_D4s"
database_storage_mb = 102400
database_backup_retention_days = 7
database_geo_redundant_backup_enabled = true

# Redis configuration
redis_sku_name = "Premium"
redis_family = "P"
redis_capacity = 1

# Security configuration
enable_private_cluster = true
enable_network_policy = true
authorized_ip_ranges = []

# AI Services configuration
ai_services_sku = "S0"
enable_cognitive_services = true
enable_openai_service = true
enable_form_recognizer = true
enable_text_analytics = true

# Backup configuration
backup_retention_days = 30
enable_point_in_time_restore = true

# Monitoring configuration
log_analytics_retention_days = 30
enable_container_insights = true

# Cost optimization
enable_cluster_autoscaler = true
enable_node_auto_repair = true
enable_node_auto_upgrade = true

# Compliance
enable_azure_defender = true
enable_policy = true

# Tags
tags = {
  Environment = "$Environment"
  Project = "TOSS"
  Owner = "Platform-Team"
  CostCenter = "IT-Infrastructure"
  Compliance = "SOC2-GDPR-HIPAA"
}
"@
        
        Set-Content -Path "terraform.tfvars" -Value $tfVars
        
        # Plan deployment
        Write-Log "Planning Terraform deployment..."
        if ($DryRun) {
            terraform plan -var-file="terraform.tfvars" -out="tfplan"
            Write-Log "Dry run completed. Terraform plan saved to tfplan" "SUCCESS"
        } else {
            terraform plan -var-file="terraform.tfvars" -out="tfplan"
            
            # Apply deployment
            Write-Log "Applying Terraform deployment..."
            terraform apply "tfplan"
            
            Write-Log "Infrastructure deployment completed successfully!" "SUCCESS"
        }
        
    } catch {
        Write-Log "Infrastructure deployment failed: $($_.Exception.Message)" "ERROR"
        exit 1
    } finally {
        Pop-Location
    }
}

# Phase 2: Kubernetes Configuration
if (-not $SkipKubernetes) {
    Write-Log "Phase 2: Configuring Kubernetes cluster" "SUCCESS"
    
    try {
        # Get AKS credentials
        Write-Log "Getting AKS credentials..."
        $clusterName = "toss-aks-$Environment"
        az aks get-credentials --resource-group $ResourceGroupName --name $clusterName --overwrite-existing
        
        # Verify cluster connectivity
        Write-Log "Verifying cluster connectivity..."
        $nodes = kubectl get nodes --no-headers | Measure-Object
        Write-Log "Connected to cluster with $($nodes.Count) nodes" "SUCCESS"
        
        # Install required operators and controllers
        Write-Log "Installing Kubernetes operators..."
        
        # Install Secrets Store CSI Driver
        Write-Log "Installing Secrets Store CSI Driver..."
        helm repo add secrets-store-csi-driver https://kubernetes-sigs.github.io/secrets-store-csi-driver/charts
        helm repo update
        helm upgrade --install csi-secrets-store secrets-store-csi-driver/secrets-store-csi-driver `
            --namespace kube-system `
            --set syncSecret.enabled=true
        
        # Install Azure Key Vault Provider
        Write-Log "Installing Azure Key Vault Provider..."
        helm upgrade --install csi-secrets-store-provider-azure secrets-store-csi-driver/secrets-store-csi-driver-provider-azure `
            --namespace kube-system
        
        # Install NVIDIA GPU Operator (if GPU nodes exist)
        Write-Log "Checking for GPU nodes..."
        $gpuNodes = kubectl get nodes -l accelerator=nvidia-gpu --no-headers 2>$null
        if ($gpuNodes) {
            Write-Log "Installing NVIDIA GPU Operator..."
            helm repo add nvidia https://nvidia.github.io/gpu-operator
            helm repo update
            helm upgrade --install gpu-operator nvidia/gpu-operator `
                --namespace gpu-operator `
                --create-namespace `
                --set driver.enabled=false
        }
        
        # Install Istio Service Mesh
        Write-Log "Installing Istio Service Mesh..."
        helm repo add istio https://istio-release.storage.googleapis.com/charts
        helm repo update
        
        # Install Istio Base
        helm upgrade --install istio-base istio/base `
            --namespace istio-system `
            --create-namespace `
            --set defaultRevision=default
        
        # Install Istiod
        helm upgrade --install istiod istio/istiod `
            --namespace istio-system `
            --wait
        
        # Install Istio Gateway
        helm upgrade --install istio-gateway istio/gateway `
            --namespace istio-system
        
        # Apply namespace and security configurations
        Write-Log "Applying namespace and security configurations..."
        kubectl apply -f "$PSScriptRoot\..\infra\k8s\namespaces.yaml"
        kubectl apply -f "$PSScriptRoot\..\infra\k8s\secret-providers.yaml"
        
        # Wait for namespaces to be ready
        Write-Log "Waiting for namespaces to be ready..."
        $namespaces = @("toss-ai", "toss-data", "toss-gateway", "toss-monitoring", "toss-security", "toss-workflows", "toss-integration")
        foreach ($ns in $namespaces) {
            kubectl wait --for=condition=Ready namespace/$ns --timeout=60s
        }
        
        # Deploy AI services
        Write-Log "Deploying AI services..."
        kubectl apply -f "$PSScriptRoot\..\infra\k8s\ai-service.yaml"
        
        # Deploy monitoring stack
        Write-Log "Deploying monitoring stack..."
        kubectl apply -f "$PSScriptRoot\..\infra\k8s\monitoring.yaml"
        
        Write-Log "Kubernetes configuration completed successfully!" "SUCCESS"
        
    } catch {
        Write-Log "Kubernetes configuration failed: $($_.Exception.Message)" "ERROR"
        exit 1
    }
}

# Phase 3: GitOps Setup
if (-not $SkipGitOps) {
    Write-Log "Phase 3: Setting up GitOps with ArgoCD" "SUCCESS"
    
    try {
        # Install ArgoCD
        Write-Log "Installing ArgoCD..."
        kubectl create namespace argocd --dry-run=client -o yaml | kubectl apply -f -
        kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml
        
        # Wait for ArgoCD to be ready
        Write-Log "Waiting for ArgoCD to be ready..."
        kubectl wait --for=condition=Available deployment/argocd-server -n argocd --timeout=300s
        
        # Install ArgoCD CLI
        Write-Log "Installing ArgoCD CLI..."
        if (Get-Command "choco" -ErrorAction SilentlyContinue) {
            choco install argocd-cli -y
        } else {
            Write-Log "Chocolatey not found. Please install ArgoCD CLI manually." "WARNING"
        }
        
        # Apply ArgoCD applications
        Write-Log "Applying ArgoCD applications..."
        kubectl apply -f "$PSScriptRoot\..\infra\gitops\argocd-applications.yaml"
        
        # Get ArgoCD admin password
        $argoCDPassword = kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | ForEach-Object { [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($_)) }
        
        Write-Log "ArgoCD installed successfully!" "SUCCESS"
        Write-Log "ArgoCD Admin Password: $argoCDPassword" "WARNING"
        Write-Log "Access ArgoCD UI: kubectl port-forward svc/argocd-server -n argocd 8080:443" "INFO"
        
    } catch {
        Write-Log "GitOps setup failed: $($_.Exception.Message)" "ERROR"
        exit 1
    }
}

# Final validation and status report
Write-Log "Performing final validation..." "SUCCESS"

try {
    # Check cluster status
    Write-Log "Cluster Status:"
    kubectl cluster-info
    
    # Check node status
    Write-Log "Node Status:"
    kubectl get nodes -o wide
    
    # Check namespace status
    Write-Log "Namespace Status:"
    kubectl get namespaces
    
    # Check AI service status
    Write-Log "AI Service Status:"
    kubectl get pods -n toss-ai
    
    # Check monitoring status
    Write-Log "Monitoring Status:"
    kubectl get pods -n toss-monitoring
    
    # Check service mesh status
    Write-Log "Service Mesh Status:"
    kubectl get pods -n istio-system
    
    # Generate deployment summary
    $deploymentSummary = @"

================================================================================
TOSS Platform Phase 2 Deployment Summary
================================================================================

Environment: $Environment
Subscription: $SubscriptionId
Resource Group: $ResourceGroupName
Location: $Location

Deployed Components:
✓ Azure Kubernetes Service (AKS) with specialized node pools
✓ Azure AI Services (Cognitive Services, OpenAI, Form Recognizer, Text Analytics)
✓ PostgreSQL Flexible Server with high availability
✓ Redis Cache with Premium features
✓ Azure Key Vault with managed identities
✓ Application Gateway with WAF
✓ Container Registry with vulnerability scanning
✓ Multi-tenant AI Service Platform
✓ GPU-enabled inference workers
✓ Prometheus and Grafana monitoring
✓ Istio Service Mesh
✓ ArgoCD GitOps platform

Next Steps:
1. Configure DNS for external access
2. Set up SSL certificates
3. Configure external identity provider integration
4. Deploy application services
5. Configure backup and disaster recovery
6. Set up alerting and notification channels

Access Information:
- ArgoCD UI: kubectl port-forward svc/argocd-server -n argocd 8080:443
- ArgoCD Admin Password: $argoCDPassword
- Grafana UI: kubectl port-forward svc/grafana -n toss-monitoring 3000:3000
- Prometheus UI: kubectl port-forward svc/prometheus -n toss-monitoring 9090:9090

Documentation: ./docs/
Support: platform-team@toss.example.com

================================================================================
"@
    
    Write-Log $deploymentSummary "SUCCESS"
    
    # Save deployment summary to file
    $deploymentSummary | Out-File -FilePath "deployment-summary-$(Get-Date -Format 'yyyyMMdd-HHmmss').txt" -Encoding UTF8
    
} catch {
    Write-Log "Final validation encountered issues: $($_.Exception.Message)" "WARNING"
}

Write-Log "TOSS Platform Phase 2 deployment completed successfully!" "SUCCESS"
Write-Log "Total deployment time: $((Get-Date) - $script:startTime)" "INFO"
