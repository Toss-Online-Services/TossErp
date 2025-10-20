export default defineNuxtRouteMiddleware((to, from) => {
  // Bypass auth in development mode
  if (process.dev) {
    return
  }

  // Skip middleware for login and public pages
  const publicPages = ['/login', '/register', '/']
  if (publicPages.includes(to.path)) {
    return
  }

  // Check authentication
  const { isAuthenticated } = useAuth()

  if (!isAuthenticated.value) {
    return navigateTo('/login')
  }

  // Optional: Check role-based access
  const { hasAnyRole } = useAuth()
  const requiredRoles = to.meta.roles as string[] | undefined

  if (requiredRoles && requiredRoles.length > 0) {
    if (!hasAnyRole(requiredRoles)) {
      return navigateTo('/unauthorized')
    }
  }
})
