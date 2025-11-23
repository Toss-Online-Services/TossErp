import type { UseFetchOptions } from 'nuxt/app'

/**
 * Base API composable for making authenticated requests to TOSS backend
 * Includes caching and request deduplication for better performance
 * 
 * Usage:
 * const { get, post, put, delete: del } = useApi()
 * const data = await get('/api/sales')
 */
export const useApi = () => {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase
  const token = useCookie('auth-token')
  const cache = useApiCache()

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
          if (options.headers instanceof Headers) {
            options.headers.set('Authorization', `Bearer ${token.value}`)
          } else {
            (options.headers as Record<string, string>)['Authorization'] = `Bearer ${token.value}`
          }
        }
      },
      onResponseError({ response }) {
        // Handle common error scenarios
        if (response.status === 401) {
          // Unauthorized - redirect to login
          if (process.client) {
            navigateTo('/auth/login')
          }
        }
        console.error('API Error:', response._data)
      }
    })
  }

  const api = createFetchInstance()

  /**
   * GET request with caching and deduplication
   */
  const get = async <T>(
    endpoint: string, 
    params?: Record<string, any>,
    options?: { 
      cache?: boolean
      ttl?: number // Time to live in milliseconds
      skipCache?: boolean
    }
  ): Promise<T> => {
    const useCache = options?.cache !== false
    const skipCache = options?.skipCache === true

    // Check for pending request (deduplication)
    if (useCache && !skipCache) {
      const pending = cache.getPendingRequest<T>(endpoint, params)
      if (pending) {
        return pending
      }
    }

    // Check cache first
    if (useCache && !skipCache) {
      const cached = cache.get<T>(endpoint, params)
      if (cached !== null) {
        return cached
      }
    }

    // Make the request
    const requestPromise = api<T>(endpoint, {
      method: 'GET',
      query: params
    }) as Promise<T>

    // Register pending request for deduplication
    if (useCache && !skipCache) {
      cache.setPendingRequest(endpoint, params, requestPromise)
    }

    try {
      const data = await requestPromise
      
      // Cache successful responses
      if (useCache && !skipCache) {
        cache.set(endpoint, data, params, options?.ttl)
      }
      
      return data
    } catch (error) {
      // Don't cache errors
      throw error
    }
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

  /**
   * Invalidate cache for specific endpoint
   */
  const invalidateCache = (endpoint?: string, params?: Record<string, any>) => {
    cache.invalidate(endpoint, params)
  }

  /**
   * Clear all cache
   */
  const clearCache = () => {
    cache.clear()
  }

  return {
    api,
    get,
    post,
    put,
    delete: del,
    apiBase,
    invalidateCache,
    clearCache
  }
}
