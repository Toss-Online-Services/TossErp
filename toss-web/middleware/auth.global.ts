// Authentication disabled for development
export default defineNuxtRouteMiddleware((to) => {
  // Allow all routes without authentication check
  // const token = useCookie<string | null>('auth_token')

  // const isLanding = to.path === '/landing' || to.path.startsWith('/landing/')
  // const publicPaths = new Set([
  //   '/',
  //   '/landing',
  //   '/help',
  //   '/copilot',
  //   '/signin',
  //   '/signup',
  //   '/reset',
  //   '/verification',
  //   '/lock',
  //   '/error'
  // ])

  // const isPublic = isLanding || publicPaths.has(to.path)

  // if (!token.value && !isPublic) {
  //   return navigateTo('/landing')
  // }

  // if (token.value && isLanding) {
  //   return navigateTo('/')
  // }
})

