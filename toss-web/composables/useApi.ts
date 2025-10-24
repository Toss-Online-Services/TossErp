import type { UseFetchOptions } from 'nuxt/app'

/**
 * Base API composable for making authenticated requests to TOSS backend
 * 
 * Usage:
 * const { get, post, put, delete: del } = useApi()
 * const data = await get('/api/sales')
 */
export const useApi = () => {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase || 'http://localhost:5001'
  const token = useCookie('auth-token')

  /**
   * Create $fetch instance with default configuration
   */
  const createFetchInstance = () => {
    return $fetch.create({
      baseURL: apiBase,
      headers: {
        'Content-Type': 'application/json',
        ...(token.value ? { Authorization: `Bearer ${token.value}` } : {})
      },
      onRequest({ options }) {
        // Add auth token if available
        if (token.value && options.headers) {
          (options.headers as Record<string, string>).Authorization = `Bearer ${token.value}`
        }
      },
      onResponseError({ response }) {
        // Handle common error scenarios
        if (response.status === 401) {
          // Unauthorized - redirect to login
          if (process.client) {
            navigateTo('/login')
          }
        }
        console.error('API Error:', response._data)
      }
    })
  }

  const api = createFetchInstance()

  /**
   * GET request
   */
  const get = async <T>(endpoint: string, params?: Record<string, any>): Promise<T> => {
    return await api<T>(endpoint, {
      method: 'GET',
      query: params
    })
  }

  /**
   * POST request
   */
  const post = async <T>(endpoint: string, body?: any): Promise<T> => {
    return await api<T>(endpoint, {
      method: 'POST',
      body
    })
  }

  /**
   * PUT request
   */
  const put = async <T>(endpoint: string, body?: any): Promise<T> => {
    return await api<T>(endpoint, {
      method: 'PUT',
      body
    })
  }

  /**
   * DELETE request
   */
  const del = async <T>(endpoint: string): Promise<T> => {
    return await api<T>(endpoint, {
      method: 'DELETE'
    })
  }

  return {
    api,
    get,
    post,
    put,
    delete: del,
    apiBase
  }
}
