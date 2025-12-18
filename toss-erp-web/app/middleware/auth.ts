export default defineNuxtRouteMiddleware((to, from) => {
  const userStore = useUserStore()
  
  // Check if user is authenticated
  if (!userStore.isAuthenticated) {
    // Redirect to login page
    return navigateTo('/auth/login', { redirectCode: 301 })
  }
})
