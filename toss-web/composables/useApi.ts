import type { UseFetchOptions } from 'nuxt/app'

export const useApi = () => {
  const { getAuthHeader } = useAuth()
  const config = useRuntimeConfig()
  const apiBaseUrl = config.public.apiBaseUrl || 'http://localhost:5000'

  /**
   * Make an authenticated API request
   */
  const request = async <T>(
    endpoint: string,
    options?: UseFetchOptions<T>
  ) => {
    const headers = {
      ...getAuthHeader(),
      'Content-Type': 'application/json',
      ...options?.headers,
    }

    const url = endpoint.startsWith('http') ? endpoint : `${apiBaseUrl}${endpoint}`

    return await $fetch<T>(url, {
      ...options,
      headers,
    })
  }

  /**
   * GET request
   */
  const get = async <T>(endpoint: string, params?: Record<string, any>) => {
    const query = params ? new URLSearchParams(params).toString() : ''
    const url = query ? `${endpoint}?${query}` : endpoint
    
    return await request<T>(url, {
      method: 'GET',
    })
  }

  /**
   * POST request
   */
  const post = async <T>(endpoint: string, body?: any) => {
    return await request<T>(endpoint, {
      method: 'POST',
      body,
    })
  }

  /**
   * PUT request
   */
  const put = async <T>(endpoint: string, body?: any) => {
    return await request<T>(endpoint, {
      method: 'PUT',
      body,
    })
  }

  /**
   * DELETE request
   */
  const del = async <T>(endpoint: string) => {
    return await request<T>(endpoint, {
      method: 'DELETE',
    })
  }

  return {
    request,
    get,
    post,
    put,
    delete: del,
  }
}

