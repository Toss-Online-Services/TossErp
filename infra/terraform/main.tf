terraform {
  required_version = ">= 1.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.71"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.23"
    }
    helm = {
      source  = "hashicorp/helm"
      version = "~> 2.11"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.5"
    }
  }

  backend "azurerm" {
    resource_group_name  = "toss-terraform-state"
    storage_account_name = "tosstfstate"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

provider "kubernetes" {
  host                   = azurerm_kubernetes_cluster.main.kube_config.0.host
  client_certificate     = base64decode(azurerm_kubernetes_cluster.main.kube_config.0.client_certificate)
  client_key             = base64decode(azurerm_kubernetes_cluster.main.kube_config.0.client_key)
  cluster_ca_certificate = base64decode(azurerm_kubernetes_cluster.main.kube_config.0.cluster_ca_certificate)
}

provider "helm" {
  kubernetes {
    host                   = azurerm_kubernetes_cluster.main.kube_config.0.host
    client_certificate     = base64decode(azurerm_kubernetes_cluster.main.kube_config.0.client_certificate)
    client_key             = base64decode(azurerm_kubernetes_cluster.main.kube_config.0.client_key)
    cluster_ca_certificate = base64decode(azurerm_kubernetes_cluster.main.kube_config.0.cluster_ca_certificate)
  }
}

# Local variables for consistent naming and configuration
locals {
  environment = var.environment
  location    = var.location
  prefix      = "toss-${local.environment}"
  
  # Common tags applied to all resources
  common_tags = {
    Environment   = local.environment
    Project       = "TOSS-ERP"
    ManagedBy     = "Terraform"
    Owner         = "DevOps-Team"
    CostCenter    = "Engineering"
    Compliance    = "SOC2-GDPR-HIPAA"
    CreatedDate   = formatdate("YYYY-MM-DD", timestamp())
  }

  # Network configuration
  vnet_address_space = "10.0.0.0/16"
  subnet_configs = {
    aks_subnet = {
      name             = "aks-subnet"
      address_prefixes = ["10.0.1.0/24"]
    }
    app_gateway_subnet = {
      name             = "appgw-subnet"
      address_prefixes = ["10.0.2.0/24"]
    }
    private_endpoint_subnet = {
      name             = "pe-subnet"
      address_prefixes = ["10.0.3.0/24"]
    }
    ai_services_subnet = {
      name             = "ai-subnet"
      address_prefixes = ["10.0.4.0/24"]
    }
  }

  # Security configuration
  security_rules = [
    {
      name                       = "AllowHTTPS"
      priority                   = 1001
      direction                  = "Inbound"
      access                     = "Allow"
      protocol                   = "Tcp"
      source_port_range          = "*"
      destination_port_range     = "443"
      source_address_prefix      = "*"
      destination_address_prefix = "*"
    },
    {
      name                       = "AllowHTTP"
      priority                   = 1002
      direction                  = "Inbound"
      access                     = "Allow"
      protocol                   = "Tcp"
      source_port_range          = "*"
      destination_port_range     = "80"
      source_address_prefix      = "*"
      destination_address_prefix = "*"
    },
    {
      name                       = "DenyAllInbound"
      priority                   = 4096
      direction                  = "Inbound"
      access                     = "Deny"
      protocol                   = "*"
      source_port_range          = "*"
      destination_port_range     = "*"
      source_address_prefix      = "*"
      destination_address_prefix = "*"
    }
  ]

  # AI Services configuration
  ai_services = {
    cognitive_services = {
      kind = "CognitiveServices"
      sku  = "S0"
    }
    openai = {
      kind = "OpenAI"
      sku  = "S0"
    }
    form_recognizer = {
      kind = "FormRecognizer"
      sku  = "S0"
    }
    text_analytics = {
      kind = "TextAnalytics"
      sku  = "S0"
    }
  }

  # Database configuration
  database_config = {
    primary = {
      sku_name                     = "GP_Gen5_4"
      storage_mb                   = 102400
      backup_retention_days        = 35
      geo_redundant_backup_enabled = true
      auto_grow_enabled           = true
      ssl_enforcement_enabled     = true
      ssl_minimal_tls_version_enforced = "TLS1_2"
    }
    replica = {
      sku_name                     = "GP_Gen5_2"
      storage_mb                   = 51200
      backup_retention_days        = 7
      geo_redundant_backup_enabled = false
      auto_grow_enabled           = true
      ssl_enforcement_enabled     = true
      ssl_minimal_tls_version_enforced = "TLS1_2"
    }
  }
}

# Random password generation for secure resources
resource "random_password" "admin_password" {
  length  = 32
  special = true
  upper   = true
  lower   = true
  numeric = true
}
