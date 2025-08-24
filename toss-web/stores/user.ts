export const useUserStore = defineStore('user', () => {
  // State
  const user = ref<User | null>(null)
  const isAuthenticated = ref(false)
  const loading = ref(false)
  const permissions = ref<string[]>([])

  // Getters
  const fullName = computed(() => {
    if (!user.value) return ''
    return `${user.value.firstName} ${user.value.lastName}`
  })

  const hasPermission = computed(() => (permission: string) => {
    return permissions.value.includes(permission) || permissions.value.includes('admin')
  })

  const userInitials = computed(() => {
    if (!user.value) return 'U'
    const firstName = user.value.firstName?.[0] || ''
    const lastName = user.value.lastName?.[0] || ''
    return (firstName + lastName).toUpperCase()
  })

  // Actions
  async function login(credentials: LoginCredentials) {
    loading.value = true
    try {
      const response = await $fetch('/api/auth/login', {
        method: 'POST',
        body: credentials
      })
      
      user.value = response.user
      isAuthenticated.value = true
      permissions.value = response.permissions || []
      
      // Store token if needed
      if (response.token) {
        const token = useCookie('auth-token', {
          default: () => null,
          secure: true,
          sameSite: 'strict',
          maxAge: 60 * 60 * 24 * 7 // 7 days
        })
        token.value = response.token
      }
      
      return response
    } catch (error) {
      throw error
    } finally {
      loading.value = false
    }
  }

  async function logout() {
    loading.value = true
    try {
      await $fetch('/api/auth/logout', {
        method: 'POST'
      })
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      user.value = null
      isAuthenticated.value = false
      permissions.value = []
      loading.value = false
      
      // Clear token
      const token = useCookie('auth-token')
      token.value = null
      
      await navigateTo('/login')
    }
  }

  async function checkAuth() {
    const token = useCookie('auth-token')
    if (!token.value) {
      return false
    }

    loading.value = true
    try {
      const response = await $fetch('/api/auth/me')
      user.value = response.user
      isAuthenticated.value = true
      permissions.value = response.permissions || []
      return true
    } catch (error) {
      // Token is invalid
      await logout()
      return false
    } finally {
      loading.value = false
    }
  }

  async function updateProfile(profileData: Partial<User>) {
    loading.value = true
    try {
      const response = await $fetch('/api/user/profile', {
        method: 'PATCH',
        body: profileData
      })
      
      user.value = { ...user.value, ...response.user }
      return response
    } catch (error) {
      throw error
    } finally {
      loading.value = false
    }
  }

  async function changePassword(passwords: ChangePasswordData) {
    loading.value = true
    try {
      const response = await $fetch('/api/user/change-password', {
        method: 'POST',
        body: passwords
      })
      return response
    } catch (error) {
      throw error
    } finally {
      loading.value = false
    }
  }

  return {
    // State
    user: readonly(user),
    isAuthenticated: readonly(isAuthenticated),
    loading: readonly(loading),
    permissions: readonly(permissions),
    
    // Getters
    fullName,
    hasPermission,
    userInitials,
    
    // Actions
    login,
    logout,
    checkAuth,
    updateProfile,
    changePassword
  }
})

// Types
export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  avatar?: string
  businessId?: string
  businessName?: string
  role: string
  status: 'active' | 'inactive' | 'pending'
  createdAt: string
  updatedAt: string
}

export interface LoginCredentials {
  email: string
  password: string
  rememberMe?: boolean
}

export interface ChangePasswordData {
  currentPassword: string
  newPassword: string
  confirmPassword: string
}
