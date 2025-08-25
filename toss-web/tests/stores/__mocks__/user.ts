import { ref, computed } from 'vue'

export const useUserStore = () => {
  const user = ref(null)
  const permissions = ref([])
  const authToken = ref(null)

  const isAuthenticated = computed(() => !!user.value)
  const userInitials = computed(() => {
    if (!user.value) return ''
    return `${user.value.firstName?.[0] || ''}${user.value.lastName?.[0] || ''}`.toUpperCase()
  })

  function hasPermission(permission: string) {
    if (!user.value || !permissions.value) return false
    if (permissions.value.includes('admin')) return true
    return permissions.value.includes(permission)
  }

  async function login(credentials: { email: string; password: string }) {
    // Mock login logic
    const response = await global.$fetch('/api/auth/login', {
      method: 'POST',
      body: credentials
    })
    
    user.value = response.user
    authToken.value = response.token
    permissions.value = response.permissions
    
    return response
  }

  function logout() {
    user.value = null
    authToken.value = null
    permissions.value = []
  }

  async function register(data: any) {
    const response = await global.$fetch('/api/auth/register', {
      method: 'POST',
      body: data
    })
    
    user.value = response.user
    authToken.value = response.token
    permissions.value = response.permissions
    
    return response
  }

  async function checkAuth() {
    if (!authToken.value) return false
    
    try {
      const response = await global.$fetch('/api/auth/me')
      user.value = response.user
      permissions.value = response.permissions
      return true
    } catch {
      logout()
      return false
    }
  }

  return {
    user,
    permissions,
    authToken,
    isAuthenticated,
    userInitials,
    hasPermission,
    login,
    logout,
    register,
    checkAuth
  }
}
