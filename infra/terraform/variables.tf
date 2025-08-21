# Variables for Terraform configuration
variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"
  
  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be one of: dev, staging, prod."
  }
}

variable "location" {
  description = "Azure region for resource deployment"
  type        = string
  default     = "East US"
  
  validation {
    condition = contains([
      "East US", "East US 2", "West US", "West US 2", "West US 3",
      "Central US", "North Central US", "South Central US",
      "West Central US", "Canada Central", "Canada East",
      "North Europe", "West Europe", "UK South", "UK West",
      "France Central", "Germany West Central", "Switzerland North",
      "Norway East", "Sweden Central", "Australia East",
      "Australia Southeast", "Japan East", "Japan West",
      "Korea Central", "Southeast Asia", "East Asia",
      "Central India", "South India", "West India"
    ], var.location)
    error_message = "Location must be a valid Azure region."
  }
}

variable "resource_group_name" {
  description = "Name of the Azure Resource Group"
  type        = string
  default     = ""
}

variable "subscription_id" {
  description = "Azure Subscription ID"
  type        = string
  sensitive   = true
}

variable "tenant_id" {
  description = "Azure Tenant ID"
  type        = string
  sensitive   = true
}

# Kubernetes configuration
variable "kubernetes_version" {
  description = "Kubernetes version for AKS cluster"
  type        = string
  default     = "1.27.3"
}

variable "node_count" {
  description = "Initial number of nodes in the AKS cluster"
  type        = number
  default     = 3
  
  validation {
    condition     = var.node_count >= 1 && var.node_count <= 100
    error_message = "Node count must be between 1 and 100."
  }
}

variable "node_vm_size" {
  description = "VM size for AKS cluster nodes"
  type        = string
  default     = "Standard_D4s_v3"
}

variable "max_node_count" {
  description = "Maximum number of nodes for autoscaling"
  type        = number
  default     = 10
  
  validation {
    condition     = var.max_node_count >= var.node_count
    error_message = "Max node count must be greater than or equal to initial node count."
  }
}

variable "min_node_count" {
  description = "Minimum number of nodes for autoscaling"
  type        = number
  default     = 1
  
  validation {
    condition     = var.min_node_count <= var.node_count
    error_message = "Min node count must be less than or equal to initial node count."
  }
}

# Database configuration
variable "db_admin_username" {
  description = "Administrator username for the PostgreSQL server"
  type        = string
  default     = "tossadmin"
  
  validation {
    condition     = length(var.db_admin_username) >= 1 && length(var.db_admin_username) <= 63
    error_message = "Database admin username must be between 1 and 63 characters."
  }
}

variable "db_name" {
  description = "Name of the main database"
  type        = string
  default     = "tosserpdb"
  
  validation {
    condition     = can(regex("^[a-zA-Z][a-zA-Z0-9_]*$", var.db_name))
    error_message = "Database name must start with a letter and contain only letters, numbers, and underscores."
  }
}

variable "enable_database_replica" {
  description = "Enable read replica for the database"
  type        = bool
  default     = false
}

# Redis configuration
variable "redis_capacity" {
  description = "Redis cache capacity"
  type        = number
  default     = 1
  
  validation {
    condition     = contains([1, 2, 3, 4, 5, 6], var.redis_capacity)
    error_message = "Redis capacity must be between 1 and 6."
  }
}

variable "redis_sku_name" {
  description = "Redis SKU name"
  type        = string
  default     = "Standard"
  
  validation {
    condition     = contains(["Basic", "Standard", "Premium"], var.redis_sku_name)
    error_message = "Redis SKU must be Basic, Standard, or Premium."
  }
}

# Monitoring and logging
variable "enable_monitoring" {
  description = "Enable Azure Monitor and Application Insights"
  type        = bool
  default     = true
}

variable "log_retention_days" {
  description = "Number of days to retain logs"
  type        = number
  default     = 30
  
  validation {
    condition     = var.log_retention_days >= 7 && var.log_retention_days <= 730
    error_message = "Log retention days must be between 7 and 730."
  }
}

# Security configuration
variable "enable_private_cluster" {
  description = "Enable private AKS cluster"
  type        = bool
  default     = true
}

variable "authorized_ip_ranges" {
  description = "Authorized IP ranges for AKS API server access"
  type        = list(string)
  default     = []
}

variable "enable_pod_security_policy" {
  description = "Enable Pod Security Policy for AKS"
  type        = bool
  default     = true
}

variable "enable_network_policy" {
  description = "Enable network policy for AKS (Calico or Azure)"
  type        = string
  default     = "azure"
  
  validation {
    condition     = contains(["azure", "calico"], var.enable_network_policy)
    error_message = "Network policy must be 'azure' or 'calico'."
  }
}

# AI Services configuration
variable "enable_ai_services" {
  description = "Enable Azure AI Services deployment"
  type        = bool
  default     = true
}

variable "ai_services_sku" {
  description = "SKU for Azure AI Services"
  type        = string
  default     = "S0"
  
  validation {
    condition     = contains(["F0", "S0", "S1", "S2", "S3"], var.ai_services_sku)
    error_message = "AI Services SKU must be one of: F0, S0, S1, S2, S3."
  }
}

variable "enable_openai_service" {
  description = "Enable Azure OpenAI Service"
  type        = bool
  default     = true
}

# Backup and disaster recovery
variable "enable_backup" {
  description = "Enable automated backup for critical resources"
  type        = bool
  default     = true
}

variable "backup_retention_days" {
  description = "Number of days to retain backups"
  type        = number
  default     = 35
  
  validation {
    condition     = var.backup_retention_days >= 7 && var.backup_retention_days <= 35
    error_message = "Backup retention days must be between 7 and 35."
  }
}

variable "enable_geo_redundancy" {
  description = "Enable geo-redundant storage for backups"
  type        = bool
  default     = true
}

# Cost optimization
variable "enable_spot_instances" {
  description = "Enable spot instances for non-critical workloads"
  type        = bool
  default     = false
}

variable "auto_scaling_enabled" {
  description = "Enable auto-scaling for AKS cluster"
  type        = bool
  default     = true
}

# Compliance and governance
variable "compliance_tags" {
  description = "Additional compliance tags for resources"
  type        = map(string)
  default = {
    DataClassification = "Confidential"
    ComplianceFramework = "SOC2-GDPR-HIPAA"
    DataResidency = "Regional"
    EncryptionRequired = "true"
  }
}

variable "enable_policy_enforcement" {
  description = "Enable Azure Policy enforcement"
  type        = bool
  default     = true
}

# Application configuration
variable "app_insights_sampling_percentage" {
  description = "Application Insights sampling percentage"
  type        = number
  default     = 100
  
  validation {
    condition     = var.app_insights_sampling_percentage >= 0 && var.app_insights_sampling_percentage <= 100
    error_message = "Sampling percentage must be between 0 and 100."
  }
}

variable "container_registry_sku" {
  description = "SKU for Azure Container Registry"
  type        = string
  default     = "Premium"
  
  validation {
    condition     = contains(["Basic", "Standard", "Premium"], var.container_registry_sku)
    error_message = "Container Registry SKU must be Basic, Standard, or Premium."
  }
}

# Multi-tenancy configuration
variable "enable_tenant_isolation" {
  description = "Enable tenant isolation features"
  type        = bool
  default     = true
}

variable "max_tenants" {
  description = "Maximum number of tenants supported"
  type        = number
  default     = 100
  
  validation {
    condition     = var.max_tenants >= 1 && var.max_tenants <= 1000
    error_message = "Max tenants must be between 1 and 1000."
  }
}

# Service mesh configuration
variable "enable_service_mesh" {
  description = "Enable Istio service mesh"
  type        = bool
  default     = true
}

variable "service_mesh_type" {
  description = "Type of service mesh to deploy"
  type        = string
  default     = "istio"
  
  validation {
    condition     = contains(["istio", "linkerd", "consul"], var.service_mesh_type)
    error_message = "Service mesh type must be istio, linkerd, or consul."
  }
}

# GitOps configuration
variable "enable_gitops" {
  description = "Enable GitOps with ArgoCD or Flux"
  type        = bool
  default     = true
}

variable "gitops_tool" {
  description = "GitOps tool to use"
  type        = string
  default     = "argocd"
  
  validation {
    condition     = contains(["argocd", "flux"], var.gitops_tool)
    error_message = "GitOps tool must be argocd or flux."
  }
}

variable "git_repository_url" {
  description = "Git repository URL for GitOps"
  type        = string
  default     = ""
}

variable "git_branch" {
  description = "Git branch for GitOps"
  type        = string
  default     = "main"
}

# Secrets management
variable "enable_external_secrets" {
  description = "Enable External Secrets Operator"
  type        = bool
  default     = true
}

variable "secrets_provider" {
  description = "Secrets provider (azure-keyvault, aws-secretsmanager, etc.)"
  type        = string
  default     = "azure-keyvault"
  
  validation {
    condition     = contains(["azure-keyvault", "aws-secretsmanager", "gcp-secretmanager", "vault"], var.secrets_provider)
    error_message = "Secrets provider must be one of: azure-keyvault, aws-secretsmanager, gcp-secretmanager, vault."
  }
}

# Performance and scaling
variable "enable_hpa" {
  description = "Enable Horizontal Pod Autoscaler"
  type        = bool
  default     = true
}

variable "enable_vpa" {
  description = "Enable Vertical Pod Autoscaler"
  type        = bool
  default     = true
}

variable "enable_cluster_autoscaler" {
  description = "Enable cluster autoscaler"
  type        = bool
  default     = true
}

# DevOps tools
variable "enable_prometheus" {
  description = "Enable Prometheus monitoring"
  type        = bool
  default     = true
}

variable "enable_grafana" {
  description = "Enable Grafana dashboards"
  type        = bool
  default     = true
}

variable "enable_jaeger" {
  description = "Enable Jaeger distributed tracing"
  type        = bool
  default     = true
}

variable "enable_elastic_stack" {
  description = "Enable Elastic Stack (ELK) for logging"
  type        = bool
  default     = true
}
