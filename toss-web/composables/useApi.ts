import { ref } from 'vue'

interface ApiOptions {
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  body?: any
  headers?: Record<string, string>
}

interface ApiResponse<T> {
  data: T | null
  error: Error | null
  loading: boolean
  execute: () => Promise<void>
}

export function useApi<T>(url: string, options: ApiOptions = {}): ApiResponse<T> {
  const data = ref<T | null>(null)
  const error = ref<Error | null>(null)
  const loading = ref(false)

  const execute = async () => {
    loading.value = true
    error.value = null

    try {
      const config = useRuntimeConfig()
      const baseURL = config.public.apiBase || 'http://localhost:5000/api'

      const response = await $fetch<T>(`${baseURL}${url}`, {
        method: options.method || 'GET',
        body: options.body,
        headers: {
          'Content-Type': 'application/json',
          ...options.headers
        }
      })

      data.value = response
    } catch (err) {
      error.value = err as Error
      console.error('API Error:', err)
    } finally {
      loading.value = false
    }
  }

  return {
    data: data as any,
    error: error as any,
    loading: loading.value,
    execute
  }
}

export function useAuthApi() {
  const token = useCookie('auth_token')

  function getHeaders(): Record<string, string> {
    const headers: Record<string, string> = {
      'Content-Type': 'application/json'
    }

    if (token.value) {
      headers['Authorization'] = `Bearer ${token.value}`
    }

    return headers
  }

  async function login(email: string, password: string) {
    const { data, error, execute } = useApi('/auth/login', {
      method: 'POST',
      body: { email, password }
    })

    await execute()

    if (data.value && 'token' in data.value) {
      token.value = (data.value as any).token
    }

    return { data, error }
  }

  async function logout() {
    token.value = null
    await navigateTo('/login')
  }

  function isAuthenticated(): boolean {
    return !!token.value
  }

  return {
    login,
    logout,
    isAuthenticated,
    getHeaders,
    token
  }
}

