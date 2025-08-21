# AKS Cluster
resource "azurerm_kubernetes_cluster" "main" {
  name                = "${local.prefix}-aks"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  dns_prefix          = "${local.prefix}-aks"
  kubernetes_version  = var.kubernetes_version
  
  # Private cluster configuration
  private_cluster_enabled             = var.enable_private_cluster
  private_cluster_public_fqdn_enabled = false
  api_server_authorized_ip_ranges     = var.authorized_ip_ranges

  # Default node pool
  default_node_pool {
    name                = "system"
    node_count          = var.node_count
    vm_size             = var.node_vm_size
    type                = "VirtualMachineScaleSets"
    vnet_subnet_id      = azurerm_subnet.subnets["aks_subnet"].id
    enable_auto_scaling = var.auto_scaling_enabled
    min_count          = var.auto_scaling_enabled ? var.min_node_count : null
    max_count          = var.auto_scaling_enabled ? var.max_node_count : null
    os_disk_size_gb    = 100
    os_disk_type       = "Managed"
    
    # Security settings
    only_critical_addons_enabled = true
    
    # Node labels
    node_labels = {
      "node-type" = "system"
    }
    
    # Node taints for system nodes
    node_taints = [
      "CriticalAddonsOnly=true:NoSchedule"
    ]

    upgrade_settings {
      max_surge = "33%"
    }
  }

  # Identity configuration
  identity {
    type = "SystemAssigned"
  }

  # Network configuration
  network_profile {
    network_plugin     = "azure"
    network_policy     = var.enable_network_policy
    dns_service_ip     = "10.254.0.10"
    service_cidr       = "10.254.0.0/16"
    load_balancer_sku  = "standard"
    outbound_type      = "loadBalancer"
  }

  # Azure AD integration
  azure_active_directory_role_based_access_control {
    managed                = true
    admin_group_object_ids = []
    azure_rbac_enabled     = true
  }

  # Add-ons
  azure_policy_enabled = var.enable_policy_enforcement

  # OMS Agent (Azure Monitor)
  dynamic "oms_agent" {
    for_each = var.enable_monitoring ? [1] : []
    content {
      log_analytics_workspace_id = azurerm_log_analytics_workspace.main[0].id
    }
  }

  # Key Vault Secrets Provider
  key_vault_secrets_provider {
    secret_rotation_enabled  = true
    secret_rotation_interval = "2m"
  }

  # Workload Identity
  workload_identity_enabled = true
  oidc_issuer_enabled      = true

  # Maintenance window
  maintenance_window {
    allowed {
      day   = "Sunday"
      hours = [2, 3, 4]
    }
  }

  # Auto-upgrade configuration
  automatic_channel_upgrade = "patch"

  tags = local.common_tags

  depends_on = [
    azurerm_subnet_network_security_group_association.main
  ]
}

# Additional node pool for applications
resource "azurerm_kubernetes_cluster_node_pool" "apps" {
  name                  = "apps"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.main.id
  vm_size               = "Standard_D8s_v3"
  node_count            = 2
  enable_auto_scaling   = true
  min_count            = 1
  max_count            = 20
  vnet_subnet_id       = azurerm_subnet.subnets["aks_subnet"].id
  os_disk_size_gb      = 100
  os_disk_type         = "Managed"
  
  # Enable spot instances for cost optimization
  priority        = var.enable_spot_instances ? "Spot" : "Regular"
  eviction_policy = var.enable_spot_instances ? "Delete" : null
  spot_max_price  = var.enable_spot_instances ? 0.5 : null

  node_labels = {
    "node-type" = "application"
  }

  upgrade_settings {
    max_surge = "33%"
  }

  tags = local.common_tags
}

# Additional node pool for AI workloads
resource "azurerm_kubernetes_cluster_node_pool" "ai" {
  count = var.enable_ai_services ? 1 : 0

  name                  = "ai"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.main.id
  vm_size               = "Standard_NC6s_v3"  # GPU-enabled VMs
  node_count            = 0
  enable_auto_scaling   = true
  min_count            = 0
  max_count            = 5
  vnet_subnet_id       = azurerm_subnet.subnets["ai_services_subnet"].id
  os_disk_size_gb      = 200
  os_disk_type         = "Premium_LRS"

  node_labels = {
    "node-type" = "ai-workload"
    "gpu"       = "nvidia-tesla-k80"
  }

  node_taints = [
    "gpu=true:NoSchedule"
  ]

  upgrade_settings {
    max_surge = "25%"
  }

  tags = local.common_tags
}

# Role assignment for AKS to pull from ACR
resource "azurerm_role_assignment" "aks_acr" {
  principal_id                     = azurerm_kubernetes_cluster.main.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                           = azurerm_container_registry.main.id
  skip_service_principal_aad_check = true
}

# Role assignment for AKS to manage network
resource "azurerm_role_assignment" "aks_network" {
  principal_id         = azurerm_kubernetes_cluster.main.identity[0].principal_id
  role_definition_name = "Network Contributor"
  scope               = azurerm_virtual_network.main.id
}

# Role assignment for AKS to access Key Vault
resource "azurerm_role_assignment" "aks_keyvault" {
  principal_id         = azurerm_kubernetes_cluster.main.identity[0].principal_id
  role_definition_name = "Key Vault Secrets User"
  scope               = azurerm_key_vault.main.id
}

# Azure AI Services
resource "azurerm_cognitive_account" "ai_services" {
  for_each = var.enable_ai_services ? local.ai_services : {}

  name                = "${local.prefix}-${lower(each.key)}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  kind                = each.value.kind
  sku_name            = each.value.sku

  # Network restrictions
  network_acls {
    default_action = "Deny"
    
    virtual_network_rules {
      subnet_id = azurerm_subnet.subnets["ai_services_subnet"].id
    }
  }

  # Identity for managed identity authentication
  identity {
    type = "SystemAssigned"
  }

  tags = local.common_tags
}

# Store AI Services keys in Key Vault
resource "azurerm_key_vault_secret" "ai_services_keys" {
  for_each = var.enable_ai_services ? azurerm_cognitive_account.ai_services : {}

  name         = "ai-${lower(each.key)}-key"
  value        = each.value.primary_access_key
  key_vault_id = azurerm_key_vault.main.id
  tags         = local.common_tags

  depends_on = [azurerm_key_vault_access_policy.aks]
}

# Store AI Services endpoints in Key Vault
resource "azurerm_key_vault_secret" "ai_services_endpoints" {
  for_each = var.enable_ai_services ? azurerm_cognitive_account.ai_services : {}

  name         = "ai-${lower(each.key)}-endpoint"
  value        = each.value.endpoint
  key_vault_id = azurerm_key_vault.main.id
  tags         = local.common_tags

  depends_on = [azurerm_key_vault_access_policy.aks]
}

# Application Gateway for ingress
resource "azurerm_public_ip" "app_gateway" {
  name                = "${local.prefix}-appgw-pip"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  allocation_method   = "Static"
  sku                 = "Standard"
  zones               = ["1", "2", "3"]
  tags                = local.common_tags
}

resource "azurerm_application_gateway" "main" {
  name                = "${local.prefix}-appgw"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  enable_http2        = true
  zones               = ["1", "2", "3"]

  sku {
    name     = "WAF_v2"
    tier     = "WAF_v2"
    capacity = 2
  }

  # Auto-scaling configuration
  autoscale_configuration {
    min_capacity = 1
    max_capacity = 10
  }

  gateway_ip_configuration {
    name      = "gateway-ip-config"
    subnet_id = azurerm_subnet.subnets["app_gateway_subnet"].id
  }

  frontend_port {
    name = "http-port"
    port = 80
  }

  frontend_port {
    name = "https-port"
    port = 443
  }

  frontend_ip_configuration {
    name                 = "frontend-ip-config"
    public_ip_address_id = azurerm_public_ip.app_gateway.id
  }

  backend_address_pool {
    name = "aks-backend-pool"
  }

  backend_http_settings {
    name                  = "http-settings"
    cookie_based_affinity = "Disabled"
    path                  = "/"
    port                  = 80
    protocol              = "Http"
    request_timeout       = 60
  }

  http_listener {
    name                           = "http-listener"
    frontend_ip_configuration_name = "frontend-ip-config"
    frontend_port_name             = "http-port"
    protocol                       = "Http"
  }

  request_routing_rule {
    name                       = "http-routing-rule"
    rule_type                  = "Basic"
    http_listener_name         = "http-listener"
    backend_address_pool_name  = "aks-backend-pool"
    backend_http_settings_name = "http-settings"
    priority                   = 10
  }

  # WAF configuration
  waf_configuration {
    enabled          = true
    firewall_mode    = "Prevention"
    rule_set_type    = "OWASP"
    rule_set_version = "3.2"
    
    disabled_rule_group {
      rule_group_name = "REQUEST-920-PROTOCOL-ENFORCEMENT"
      rules          = [920300, 920440]
    }
  }

  # Identity for managed identity authentication
  identity {
    type = "SystemAssigned"
  }

  tags = local.common_tags
}
