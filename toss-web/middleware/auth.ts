export default defineNuxtRouteMiddleware(async (to, from) => {
  // Skip middleware for login, register, and public pages
  const publicPages = ['/login', '/register', '/', '/unauthorized']
  if (publicPages.includes(to.path)) {
    return
  }

  // Check authentication
  const { isAuthenticated, user, checkSession } = useAuth()

  if (!isAuthenticated.value) {
    // Try to restore auth from localStorage
    const { restoreAuth } = useAuth()
    restoreAuth()
    
    // Check again after restore
    if (!isAuthenticated.value) {
      return navigateTo('/login')
    }
  }

  // Verify session is still valid
  const sessionValid = await checkSession()
  if (!sessionValid) {
    return navigateTo('/login')
  }

  // Check role-based access
  const { hasAnyRole, hasRole } = useAuth()
  const requiredRoles = to.meta.roles as string[] | undefined
  const requiredRole = to.meta.role as string | undefined

  if (requiredRole) {
    if (!hasRole(requiredRole)) {
      return navigateTo('/unauthorized')
    }
  } else if (requiredRoles && requiredRoles.length > 0) {
    if (!hasAnyRole(requiredRoles)) {
      return navigateTo('/unauthorized')
    }
  }

  // Check onboarding status for role-specific routes
  const onboardingRoutes = ['/retailer', '/supplier', '/driver']
  const needsOnboarding = onboardingRoutes.some(route => to.path.startsWith(route))
  
  if (needsOnboarding && user.value && !to.path.includes('/onboarding')) {
    const userRole = user.value.roles?.[0] // Get primary role
    if (userRole) {
      const { get } = useApi()
      try {
        const onboardingStatus = await get<{ isCompleted: boolean }>(
          `/api/onboarding/${user.value.id}?role=${userRole}`
        )
        
        // If onboarding not completed and not already on onboarding page
        if (!onboardingStatus?.isCompleted) {
          return navigateTo(`/${userRole.toLowerCase()}/onboarding`)
        }
      } catch (error) {
        // If error (e.g., 404 means no onboarding status), allow through
        // User will be redirected to onboarding if needed
        console.warn('Onboarding check failed:', error)
      }
    }
  }
})
