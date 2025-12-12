export default defineNuxtRouteMiddleware((to) => {
  const token = useCookie<string | null>('auth_token')

  const isLanding = to.path === '/landing' || to.path.startsWith('/landing/')
  const publicPaths = new Set([
    '/landing',
    '/help',
    '/copilot'
  ])

  const isPublic = isLanding || publicPaths.has(to.path)

  if (!token.value && !isPublic) {
    return navigateTo('/landing')
  }

  if (token.value && isLanding) {
    return navigateTo('/')
  }
})

