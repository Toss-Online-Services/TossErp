export default defineNuxtRouteMiddleware((to) => {
  const userStore = useUserStore()
  
  // If user is not authenticated, redirect to login
  if (!userStore.isAuthenticated) {
    return navigateTo('/login')
  }
  
  // Check for specific permissions if required
  if (to.meta.requiresPermission) {
    const requiredPermission = to.meta.requiresPermission as string
    if (!userStore.hasPermission.value(requiredPermission)) {
      throw createError({
        statusCode: 403,
        statusMessage: 'Access denied. Insufficient permissions.'
      })
    }
  }
})
