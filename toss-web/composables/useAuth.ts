import { ref, computed } from 'vue'
import { jwtDecode } from 'jwt-decode'
import type { 
  AuthUser as User, 
  LoginCredentials, 
  AuthResponse, 
  TokenPayload, 
  RefreshTokenResponse 
} from '~/types/auth'

export const useAuth = () => {
  const user = useState<User | null>('auth-user', () => null)
  const token = useState<string | null>('auth-token', () => null)
  const refreshToken = useState<string | null>('auth-refresh-token', () => null)
  const tokenExpiry = useState<number | null>('auth-token-expiry', () => null)
  const isAuthenticated = computed(() => !!user.value && !!token.value && !isTokenExpired())
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const refreshTimer = ref<NodeJS.Timeout | null>(null)

  const apiBaseUrl = useRuntimeConfig().public.apiBase || 'http://localhost:5001'

  /**
   * Check if token is expired
   */
  const isTokenExpired = (): boolean => {
    if (!token.value) return true
    
    // In development with fake tokens, check localStorage expiry
    if (token.value.startsWith('dev-token-')) {
      if (!tokenExpiry.value) return false // No expiry set yet
      return Date.now() >= tokenExpiry.value
    }
    
    // For real JWT tokens
    if (!tokenExpiry.value) return true
    return Date.now() >= tokenExpiry.value
  }

  /**
   * Decode JWT token to get payload
   */
  const decodeToken = (token: string): TokenPayload | null => {
    try {
      return jwtDecode<TokenPayload>(token)
    } catch (error) {
      console.error('Failed to decode token:', error)
      return null
    }
  }

  /**
   * Set token with automatic expiry calculation
   */
  const setToken = (newToken: string, expiresIn?: number) => {
    token.value = newToken
    
    // Calculate expiry time
    if (expiresIn) {
      tokenExpiry.value = Date.now() + (expiresIn * 1000)
    } else if (newToken.startsWith('dev-token-')) {
      // For development tokens, set a default expiry of 4 hours
      tokenExpiry.value = Date.now() + (4 * 60 * 60 * 1000)
    } else {
      const payload = decodeToken(newToken)
      if (payload) {
        tokenExpiry.value = payload.exp * 1000
      }
    }

    // Store in localStorage
    if (process.client) {
      localStorage.setItem('auth-token', newToken)
      if (tokenExpiry.value) {
        localStorage.setItem('auth-token-expiry', tokenExpiry.value.toString())
      }
    }

    // Setup auto-refresh only for production tokens
    if (!newToken.startsWith('dev-token-')) {
      setupTokenRefresh()
    }
  }

  /**
   * Refresh access token using refresh token
   */
  const refreshAccessToken = async (): Promise<boolean> => {
    if (!refreshToken.value) {
      console.warn('No refresh token available')
      return false
    }

    try {
      const response = await $fetch<RefreshTokenResponse>(`${apiBaseUrl}/api/auth/refresh`, {
        method: 'POST',
        body: { refreshToken: refreshToken.value },
      })

      setToken(response.token, response.expiresIn)
      return true
    } catch (error) {
      console.error('Token refresh failed:', error)
      await logout()
      return false
    }
  }

  /**
   * Setup automatic token refresh
   */
  const setupTokenRefresh = () => {
    if (refreshTimer.value) {
      clearTimeout(refreshTimer.value)
    }

    if (!tokenExpiry.value) return

    // Refresh token 5 minutes before expiry
    const refreshTime = tokenExpiry.value - Date.now() - (5 * 60 * 1000)
    
    if (refreshTime > 0) {
      refreshTimer.value = setTimeout(async () => {
        const success = await refreshAccessToken()
        if (!success) {
          console.warn('Failed to refresh token, user will be logged out')
        }
      }, refreshTime)
    }
  }

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
      user.value = { ...response.user, lastLogin: new Date() }
      refreshToken.value = response.refreshToken
      setToken(response.token, response.expiresIn)

      // Store persistent data in localStorage
      if (process.client) {
        localStorage.setItem('auth-user', JSON.stringify(user.value))
        localStorage.setItem('auth-refresh-token', response.refreshToken)
        
        // Store remember me preference
        if (credentials.rememberMe) {
          localStorage.setItem('auth-remember-me', 'true')
        }
      }

      // Log successful login
      if (process.client) {
        const { logLogin } = useAudit()
        await logLogin(true)
      }

      return true
    } catch (e: any) {
      error.value = e.message || 'Login failed'
      
      // Log failed login
      if (process.client) {
        const { logLogin } = useAudit()
        await logLogin(false, error.value)
      }
      
      return false
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Logout user
   */
  const logout = async (reason?: string) => {
    // Log logout before clearing state
    if (process.client && user.value) {
      const { logLogout } = useAudit()
      await logLogout()
    }

    // Clear refresh timer
    if (refreshTimer.value) {
      clearTimeout(refreshTimer.value)
      refreshTimer.value = null
    }

    // Notify server about logout (optional)
    if (refreshToken.value) {
      try {
        await $fetch(`${apiBaseUrl}/api/auth/logout`, {
          method: 'POST',
          body: { refreshToken: refreshToken.value },
        })
      } catch (error) {
        // Ignore logout errors
        console.warn('Logout request failed:', error)
      }
    }

    // Clear all auth state
    user.value = null
    token.value = null
    refreshToken.value = null
    tokenExpiry.value = null

    // Clear localStorage
    if (process.client) {
      localStorage.removeItem('auth-token')
      localStorage.removeItem('auth-user')
      localStorage.removeItem('auth-refresh-token')
      localStorage.removeItem('auth-token-expiry')
      localStorage.removeItem('auth-remember-me')
    }

    // Show logout reason if provided
    if (reason) {
      console.info('Logout reason:', reason)
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
      const storedRefreshToken = localStorage.getItem('auth-refresh-token')
      const storedTokenExpiry = localStorage.getItem('auth-token-expiry')

      if (storedToken && storedUser && storedRefreshToken) {
        try {
          user.value = JSON.parse(storedUser)
          refreshToken.value = storedRefreshToken
        token.value = storedToken
          
          if (storedTokenExpiry) {
            tokenExpiry.value = parseInt(storedTokenExpiry)
          }

          // Check if token is expired
          if (isTokenExpired()) {
            // For development tokens, just clear auth
            if (storedToken.startsWith('dev-token-')) {
              logout('Session expired')
            } else {
              // Try to refresh production token
              refreshAccessToken()
            }
          } else {
            // Setup auto-refresh only for production tokens
            if (!storedToken.startsWith('dev-token-')) {
              setupTokenRefresh()
            }
          }
        } catch (error) {
          console.error('Failed to restore auth:', error)
          // Clear corrupted data
          logout('Corrupted authentication data')
        }
      }
    }
  }

  /**
   * Force token refresh
   */
  const forceRefresh = async (): Promise<boolean> => {
    return await refreshAccessToken()
  }

  /**
   * Check session validity
   */
  const checkSession = async (): Promise<boolean> => {
    if (!token.value) return false

    try {
      await $fetch(`${apiBaseUrl}/api/auth/verify`, {
        headers: getAuthHeader(),
      })
      return true
    } catch (error) {
      console.warn('Session verification failed:', error)
      await logout('Session expired')
      return false
    }
  }

  /**
   * Get time until token expires (in minutes)
   */
  const getTokenTimeRemaining = (): number => {
    if (!tokenExpiry.value) return 0
    const remaining = tokenExpiry.value - Date.now()
    return Math.max(0, Math.floor(remaining / (1000 * 60)))
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

  return {
    user: readonly(user),
    token: readonly(token),
    refreshToken: readonly(refreshToken),
    tokenExpiry: readonly(tokenExpiry),
    isAuthenticated,
    isLoading: readonly(isLoading),
    error: readonly(error),
    login,
    logout,
    restoreAuth,
    forceRefresh,
    checkSession,
    getTokenTimeRemaining,
    isTokenExpired,
    hasRole,
    hasPermission,
    hasAnyRole,
    getAuthHeader,
  }
}

