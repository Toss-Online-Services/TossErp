// Tenant middleware for Service as Software platform
export default defineNuxtRouteMiddleware((to) => {
  // Auto-set demo tenant for demonstration purposes
  if (process.client) {
    // Set tenant cookie on client side for demo
    const tenantCookie = useCookie('tenant-id', {
      default: () => 'demo-salon',
      maxAge: 60 * 60 * 24 * 7 // 7 days
    })
    
    if (!tenantCookie.value) {
      tenantCookie.value = 'demo-salon'
    }
  }
})
