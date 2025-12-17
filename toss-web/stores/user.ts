import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', () => {
  const {
    user: authUser,
    isAuthenticated: authIsAuthenticated,
    isLoading,
    login: authLogin,
    logout: authLogout,
    restoreAuth,
    checkSession,
    hasRole,
    hasPermission: authHasPermission
  } = useAuth()

  // Use auth composable state
  const user = authUser
  const isAuthenticated = authIsAuthenticated
  const loading = isLoading

  // Computed
  const fullName = computed(() => {
    if (!user.value) return ''
    return user.value.name || `${user.value.id}`
  })

  const hasPermission = computed(() => (permission: string) => {
    return authHasPermission(permission)
  })

  const userInitials = computed(() => {
    if (!user.value || !user.value.name) return 'U'
    const nameParts = user.value.name.split(' ')
    const firstName = nameParts[0]?.[0] || ''
    const lastName = nameParts[1]?.[0] || ''
    return (firstName + lastName).toUpperCase()
  })

  const permissions = computed(() => user.value?.permissions || [])

  // Actions
  const login = async (credentials: any) => {
    return await authLogin(credentials)
  }

  const logout = async () => {
    await authLogout()
  }

  const checkAuth = async () => {
    // Restore from local storage first
    restoreAuth()
    
    // Then verify with backend
    if (user.value) {
      return await checkSession()
    }
    return false
  }

  const updateProfile = async (profileData: any) => {
    const { put } = useApi()
    try {
      const response = await put('/api/user/profile', profileData)
      // User will be updated on next auth check
      return response
    } catch (error) {
      throw error
    }
  }

  const changePassword = async (passwords: any) => {
    const { post } = useApi()
    try {
      const response = await post('/api/user/change-password', passwords)
      return response
    } catch (error) {
      throw error
    }
  }

  const checkRole = (role: string) => {
    return hasRole(role)
  }

  return {
    // State
    user,
    isAuthenticated,
    loading,
    permissions,
    
    // Computed
    fullName,
    hasPermission,
    userInitials,
    
    // Actions
    login,
    logout,
    checkAuth,
    updateProfile,
    changePassword,
    checkRole
  }
})
