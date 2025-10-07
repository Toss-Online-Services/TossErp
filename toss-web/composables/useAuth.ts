import { ref, computed } from 'vue'

export interface User {
  id: number
  name: string
  email: string
  roles: string[]
  permissions: string[]
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface AuthResponse {
  token: string
  refreshToken: string
  user: User
  expiresIn: number
}

export const useAuth = () => {
  const user = useState<User | null>('auth-user', () => null)
  const token = useState<string | null>('auth-token', () => null)
  const isAuthenticated = computed(() => !!user.value && !!token.value)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const apiBaseUrl = useRuntimeConfig().public.apiBaseUrl || 'http://localhost:5000'

  /**
   * Login user with email and password
   */
  const login = async (credentials: LoginCredentials): Promise<boolean> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<AuthResponse>(`${apiBaseUrl}/api/auth/login`, {
        method: 'POST',
        body: credentials,
      })

      // Store authentication data
      user.value = response.user
      token.value = response.token

      // Store token in localStorage for persistence
      if (process.client) {
        localStorage.setItem('auth-token', response.token)
        localStorage.setItem('auth-user', JSON.stringify(response.user))
      }

      return true
    } catch (e: any) {
      error.value = e.message || 'Login failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Logout user
   */
  const logout = async () => {
    user.value = null
    token.value = null

    if (process.client) {
      localStorage.removeItem('auth-token')
      localStorage.removeItem('auth-user')
    }

    await navigateTo('/login')
  }

  /**
   * Restore authentication from localStorage
   */
  const restoreAuth = () => {
    if (process.client) {
      const storedToken = localStorage.getItem('auth-token')
      const storedUser = localStorage.getItem('auth-user')

      if (storedToken && storedUser) {
        token.value = storedToken
        user.value = JSON.parse(storedUser)
      }
    }
  }

  /**
   * Check if user has a specific role
   */
  const hasRole = (role: string): boolean => {
    return user.value?.roles.includes(role) ?? false
  }

  /**
   * Check if user has a specific permission
   */
  const hasPermission = (permission: string): boolean => {
    return user.value?.permissions.includes(permission) ?? false
  }

  /**
   * Check if user has any of the specified roles
   */
  const hasAnyRole = (roles: string[]): boolean => {
    return roles.some(role => hasRole(role))
  }

  /**
   * Get authorization header for API calls
   */
  const getAuthHeader = () => {
    return token.value ? { Authorization: `Bearer ${token.value}` } : {}
  }

  // Restore auth on composable creation
  if (process.client) {
    restoreAuth()
  }

  return {
    user: readonly(user),
    token: readonly(token),
    isAuthenticated,
    isLoading: readonly(isLoading),
    error: readonly(error),
    login,
    logout,
    restoreAuth,
    hasRole,
    hasPermission,
    hasAnyRole,
    getAuthHeader,
  }
}

