# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name != "" ? var.resource_group_name : "${local.prefix}-rg"
  location = var.location
  tags     = local.common_tags
}

# Virtual Network
resource "azurerm_virtual_network" "main" {
  name                = "${local.prefix}-vnet"
  address_space       = [local.vnet_address_space]
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  tags                = local.common_tags
}

# Subnets
resource "azurerm_subnet" "subnets" {
  for_each = local.subnet_configs

  name                 = "${local.prefix}-${each.value.name}"
  resource_group_name  = azurerm_resource_group.main.name
  virtual_network_name = azurerm_virtual_network.main.name
  address_prefixes     = each.value.address_prefixes

  # Enable private endpoint network policies for specific subnets
  private_endpoint_network_policies_enabled = each.key == "private_endpoint_subnet" ? false : true
}

# Network Security Groups
resource "azurerm_network_security_group" "main" {
  name                = "${local.prefix}-nsg"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  tags                = local.common_tags

  dynamic "security_rule" {
    for_each = local.security_rules
    content {
      name                       = security_rule.value.name
      priority                   = security_rule.value.priority
      direction                  = security_rule.value.direction
      access                     = security_rule.value.access
      protocol                   = security_rule.value.protocol
      source_port_range          = security_rule.value.source_port_range
      destination_port_range     = security_rule.value.destination_port_range
      source_address_prefix      = security_rule.value.source_address_prefix
      destination_address_prefix = security_rule.value.destination_address_prefix
    }
  }
}

# Associate NSG with subnets
resource "azurerm_subnet_network_security_group_association" "main" {
  for_each = azurerm_subnet.subnets

  subnet_id                 = each.value.id
  network_security_group_id = azurerm_network_security_group.main.id
}

# Azure Container Registry
resource "azurerm_container_registry" "main" {
  name                = "${replace(local.prefix, "-", "")}acr"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = var.container_registry_sku
  admin_enabled       = false
  tags                = local.common_tags

  # Enable content trust and vulnerability scanning for Premium SKU
  dynamic "trust_policy" {
    for_each = var.container_registry_sku == "Premium" ? [1] : []
    content {
      enabled = true
    }
  }

  dynamic "retention_policy" {
    for_each = var.container_registry_sku == "Premium" ? [1] : []
    content {
      days    = 30
      enabled = true
    }
  }

  # Network access rules
  network_rule_set {
    default_action = "Deny"
    
    virtual_network {
      action    = "Allow"
      subnet_id = azurerm_subnet.subnets["aks_subnet"].id
    }
  }
}

# Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "main" {
  count = var.enable_monitoring ? 1 : 0

  name                = "${local.prefix}-logs"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = var.log_retention_days
  tags                = local.common_tags
}

# Application Insights
resource "azurerm_application_insights" "main" {
  count = var.enable_monitoring ? 1 : 0

  name                = "${local.prefix}-appinsights"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main[0].id
  application_type    = "web"
  sampling_percentage = var.app_insights_sampling_percentage
  tags                = local.common_tags
}

# Key Vault
resource "azurerm_key_vault" "main" {
  name                = "${replace(local.prefix, "-", "")}kv"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  tenant_id           = var.tenant_id
  sku_name            = "premium"
  
  # Security settings
  enabled_for_disk_encryption     = true
  enabled_for_template_deployment = true
  enabled_for_deployment          = true
  purge_protection_enabled        = true
  soft_delete_retention_days      = 7
  
  tags = local.common_tags

  # Network access rules
  network_acls {
    default_action = "Deny"
    bypass         = "AzureServices"
    
    virtual_network_subnet_ids = [
      azurerm_subnet.subnets["aks_subnet"].id,
      azurerm_subnet.subnets["private_endpoint_subnet"].id
    ]
  }
}

# Key Vault access policy for AKS
resource "azurerm_key_vault_access_policy" "aks" {
  key_vault_id = azurerm_key_vault.main.id
  tenant_id    = var.tenant_id
  object_id    = azurerm_kubernetes_cluster.main.kubelet_identity[0].object_id

  secret_permissions = [
    "Get",
    "List"
  ]

  certificate_permissions = [
    "Get",
    "List"
  ]
}

# Store database password in Key Vault
resource "azurerm_key_vault_secret" "db_password" {
  name         = "database-admin-password"
  value        = random_password.admin_password.result
  key_vault_id = azurerm_key_vault.main.id
  tags         = local.common_tags

  depends_on = [azurerm_key_vault_access_policy.aks]
}

# PostgreSQL Server
resource "azurerm_postgresql_flexible_server" "main" {
  name                = "${local.prefix}-postgres"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  
  administrator_login    = var.db_admin_username
  administrator_password = random_password.admin_password.result
  
  sku_name                     = local.database_config.primary.sku_name
  storage_mb                   = local.database_config.primary.storage_mb
  backup_retention_days        = local.database_config.primary.backup_retention_days
  geo_redundant_backup_enabled = local.database_config.primary.geo_redundant_backup_enabled
  auto_grow_enabled           = local.database_config.primary.auto_grow_enabled
  
  version = "14"
  zone    = "1"

  # Network configuration
  delegated_subnet_id = azurerm_subnet.subnets["private_endpoint_subnet"].id
  private_dns_zone_id = azurerm_private_dns_zone.postgres.id

  # High availability configuration
  high_availability {
    mode                      = "ZoneRedundant"
    standby_availability_zone = "2"
  }

  # Maintenance window
  maintenance_window {
    day_of_week  = 0    # Sunday
    start_hour   = 2    # 2 AM
    start_minute = 0
  }

  tags = local.common_tags

  depends_on = [azurerm_private_dns_zone_virtual_network_link.postgres]
}

# PostgreSQL Database
resource "azurerm_postgresql_flexible_server_database" "main" {
  name      = var.db_name
  server_id = azurerm_postgresql_flexible_server.main.id
  collation = "en_US.utf8"
  charset   = "utf8"
}

# PostgreSQL Read Replica (optional)
resource "azurerm_postgresql_flexible_server" "replica" {
  count = var.enable_database_replica ? 1 : 0

  name                = "${local.prefix}-postgres-replica"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  
  source_server_id = azurerm_postgresql_flexible_server.main.id
  
  sku_name                     = local.database_config.replica.sku_name
  storage_mb                   = local.database_config.replica.storage_mb
  backup_retention_days        = local.database_config.replica.backup_retention_days
  geo_redundant_backup_enabled = local.database_config.replica.geo_redundant_backup_enabled
  auto_grow_enabled           = local.database_config.replica.auto_grow_enabled
  
  zone = "2"
  
  tags = local.common_tags
}

# Private DNS Zone for PostgreSQL
resource "azurerm_private_dns_zone" "postgres" {
  name                = "${local.prefix}-postgres.postgres.database.azure.com"
  resource_group_name = azurerm_resource_group.main.name
  tags                = local.common_tags
}

# Link private DNS zone to VNet
resource "azurerm_private_dns_zone_virtual_network_link" "postgres" {
  name                  = "${local.prefix}-postgres-vnet-link"
  private_dns_zone_name = azurerm_private_dns_zone.postgres.name
  virtual_network_id    = azurerm_virtual_network.main.id
  resource_group_name   = azurerm_resource_group.main.name
  tags                  = local.common_tags
}

# Redis Cache
resource "azurerm_redis_cache" "main" {
  name                = "${replace(local.prefix, "-", "")}redis"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  capacity            = var.redis_capacity
  family              = var.redis_sku_name == "Premium" ? "P" : "C"
  sku_name            = var.redis_sku_name
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"
  
  # Advanced configuration
  redis_configuration {
    enable_authentication           = true
    maxmemory_reserved             = var.redis_sku_name == "Premium" ? 125 : 50
    maxfragmentationmemory_reserved = var.redis_sku_name == "Premium" ? 125 : 50
    maxmemory_delta                = var.redis_sku_name == "Premium" ? 125 : 50
    maxmemory_policy               = "allkeys-lru"
  }

  # Premium features
  dynamic "patch_schedule" {
    for_each = var.redis_sku_name == "Premium" ? [1] : []
    content {
      day_of_week    = "Sunday"
      start_hour_utc = 2
    }
  }

  tags = local.common_tags
}

# Store Redis connection string in Key Vault
resource "azurerm_key_vault_secret" "redis_connection" {
  name         = "redis-connection-string"
  value        = azurerm_redis_cache.main.primary_connection_string
  key_vault_id = azurerm_key_vault.main.id
  tags         = local.common_tags

  depends_on = [azurerm_key_vault_access_policy.aks]
}
