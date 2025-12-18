import type { FetchOptions } from 'ofetch'

export const useApiInterceptor = () => {
  const { token, refreshToken, isTokenExpired, forceRefresh, logout, getAuthHeader } = useAuth()

  /**
   * Create an authenticated fetch instance with automatic token refresh
   */
  const createAuthenticatedFetch = () => {
    return $fetch.create({
      // Request interceptor
      onRequest: async ({ options }) => {
        // Add auth header if token exists and is not expired
        if (token.value && !isTokenExpired()) {
          const authHeaders = getAuthHeader()
          if (options.headers) {
            if (options.headers instanceof Headers) {
              Object.entries(authHeaders).forEach(([key, value]) => {
                options.headers.set(key, value)
              })
            } else {
              options.headers = {
                ...options.headers,
                ...authHeaders
              }
            }
          } else {
            options.headers = authHeaders
          }
        } else if (token.value && isTokenExpired() && refreshToken.value) {
          // Try to refresh token before request
          const refreshSuccess = await forceRefresh()
          if (refreshSuccess) {
            const authHeaders = getAuthHeader()
            if (options.headers) {
              if (options.headers instanceof Headers) {
                Object.entries(authHeaders).forEach(([key, value]) => {
                  options.headers.set(key, value)
                })
              } else {
                options.headers = {
                  ...options.headers,
                  ...authHeaders
                }
              }
            } else {
              options.headers = authHeaders
            }
          } else {
            // Refresh failed, redirect to login
            await logout('Token refresh failed')
            throw new Error('Authentication required')
          }
        }
      },

      // Response interceptor
      onResponseError: async ({ response, options }) => {
        // Handle 401 Unauthorized responses
        if (response.status === 401) {
          // Try to refresh token once
          if (refreshToken.value && !options.retry) {
            const refreshSuccess = await forceRefresh()
            if (refreshSuccess) {
              // Retry the original request with new token
              return $fetch(response.url, {
                ...options,
                headers: {
                  ...options.headers,
                  ...getAuthHeader()
                },
                retry: true // Mark as retry to prevent infinite loops
              })
            }
          }
          
          // Refresh failed or no refresh token, logout user
          await logout('Session expired')
          throw new Error('Authentication required')
        }

        // Handle other errors normally
        throw response._data || new Error(`HTTP ${response.status}: ${response.statusText}`)
      }
    })
  }

  /**
   * Enhanced API fetch with automatic authentication and retry
   */
  const authenticatedFetch = async <T = any>(
    url: string, 
    options: FetchOptions = {}
  ): Promise<T> => {
    const authFetch = createAuthenticatedFetch()
    return authFetch<T>(url, options)
  }

  /**
   * API methods with authentication
   */
  const api = {
    get: <T = any>(url: string, options?: FetchOptions) => 
      authenticatedFetch<T>(url, { ...options, method: 'GET' }),
    
    post: <T = any>(url: string, body?: any, options?: FetchOptions) => 
      authenticatedFetch<T>(url, { ...options, method: 'POST', body }),
    
    put: <T = any>(url: string, body?: any, options?: FetchOptions) => 
      authenticatedFetch<T>(url, { ...options, method: 'PUT', body }),
    
    patch: <T = any>(url: string, body?: any, options?: FetchOptions) => 
      authenticatedFetch<T>(url, { ...options, method: 'PATCH', body }),
    
    delete: <T = any>(url: string, options?: FetchOptions) => 
      authenticatedFetch<T>(url, { ...options, method: 'DELETE' }),
  }

  return {
    authenticatedFetch,
    api
  }
}
