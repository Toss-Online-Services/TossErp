// Simple test endpoint for debugging tenant context
export default defineEventHandler(async (event) => {
  try {
    // Try to get tenant context
    const tenantId = getHeader(event, 'x-tenant-id') || 
                     getCookie(event, 'tenant-id') ||
                     'demo-salon'
    
    return {
      success: true,
      debug: {
        headers: {
          'x-tenant-id': getHeader(event, 'x-tenant-id'),
          'host': getHeader(event, 'host'),
          'authorization': getHeader(event, 'authorization')
        },
        cookies: {
          'tenant-id': getCookie(event, 'tenant-id'),
          'auth-token': getCookie(event, 'auth-token')
        },
        resolvedTenant: tenantId,
        timestamp: new Date().toISOString()
      },
      data: {
        message: 'Tenant debug test successful',
        tenantId: tenantId
      }
    }
  } catch (error) {
    return {
      success: false,
      error: error.message,
      stack: error.stack
    }
  }
})
