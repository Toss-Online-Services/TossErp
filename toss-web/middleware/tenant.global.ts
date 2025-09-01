export default defineNuxtRouteMiddleware((to) => {
  // Attach a default tenantId for dev to route meta if needed
  const tenantId = useCookie('tenant-id')
  if (!tenantId.value) tenantId.value = 'tenant1'
})

export default defineNuxtRouteMiddleware((to) => {
  // Global tenant middleware
  // This runs on every route change
  
  // For now, this is a placeholder for future tenant-specific logic
  // such as determining the current tenant based on subdomain, headers, etc.
  
  // Example future implementation:
  // const tenantStore = useTenantStore()
  // await tenantStore.initializeTenant(to)
})