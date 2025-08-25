// Tenant authentication composable for Service as Software platform
export const useTenant = () => {
  const setDemoTenant = (tenantId: string = 'demo-salon') => {
    // Set tenant context in cookie for demo purposes
    const tenantCookie = useCookie('tenant-id', {
      default: () => tenantId,
      maxAge: 60 * 60 * 24 * 7, // 7 days
      sameSite: 'lax'
    })
    
    // Also set an auth token for demo
    const authCookie = useCookie('auth-token', {
      default: () => `demo.token.${tenantId}`,
      maxAge: 60 * 60 * 24 * 7, // 7 days  
      sameSite: 'lax'
    })
    
    tenantCookie.value = tenantId
    authCookie.value = `demo.token.${tenantId}`
    
    return {
      tenantId: tenantCookie.value,
      authToken: authCookie.value
    }
  }
  
  const getCurrentTenant = () => {
    const tenantCookie = useCookie('tenant-id')
    const authCookie = useCookie('auth-token')
    
    return {
      tenantId: tenantCookie.value,
      authToken: authCookie.value
    }
  }
  
  const clearTenant = () => {
    const tenantCookie = useCookie('tenant-id')
    const authCookie = useCookie('auth-token')
    
    tenantCookie.value = null
    authCookie.value = null
  }
  
  return {
    setDemoTenant,
    getCurrentTenant,
    clearTenant
  }
}
