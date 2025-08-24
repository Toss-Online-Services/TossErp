// API utilities for TOSS ERP
import type { FetchOptions } from 'ofetch'

// Types
interface ApiResponse<T = any> {
  data?: T
  success: boolean
  message?: string
  errors?: string[]
}

export interface ApiError {
  message: string
  status?: number
  statusText?: string
  data?: any
}

// Configuration helper
const getConfig = () => {
  try {
    // Check if we're in browser environment  
    if (typeof window !== 'undefined') {
      return {
        public: {
          gatewayUrl: 'http://localhost:8080',
          apiTimeout: 30000
        }
      }
    }
  } catch {
    // Fallback for non-Nuxt contexts
  }
  return {
    public: {
      gatewayUrl: 'http://localhost:8080',
      apiTimeout: 30000
    }
  }
}

const config = getConfig()

// Base API URL from runtime config
export const API_BASE_URL = config.public.gatewayUrl || 'http://localhost:8080'
export const API_ENDPOINTS = {
  // Dashboard
  dashboard: '/api/dashboard',
  
  // Stock Management
  stock: '/api/stock',
  stockItems: '/api/stock/items',
  stockCategories: '/api/stock/categories',
  stockAdjustments: '/api/stock/adjustments',
  
  // Sales & POS
  sales: '/api/sales',
  orders: '/api/sales/orders',
  customers: '/api/sales/customers',
  pos: '/api/sales/pos',
  
  // Financial Management
  finance: '/api/finance',
  transactions: '/api/finance/transactions',
  reports: '/api/finance/reports',
  
  // Collaboration
  collaboration: '/api/collaboration',
  groups: '/api/collaboration/groups',
  opportunities: '/api/collaboration/opportunities',
  
  // System
  health: '/health',
  version: '/api/version'
} as const

// Default fetch options
const defaultOptions = {
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  },
  timeout: (config.public as any)?.apiTimeout || 30000
}

// Helper to get auth token safely
const getAuthToken = () => {
  try {
    if (typeof window !== 'undefined' && (window as any).__NUXT__) {
      // Try to get token from Nuxt context or localStorage
      return localStorage.getItem('auth-token')
    }
  } catch {
    // Fallback
  }
  return null
}

// Generate correlation ID for request tracking
const generateCorrelationId = () => {
  return `req-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
}

// Custom fetch wrapper with error handling
export const apiClient = async <T = any>(
  url: string, 
  options: RequestInit & { timeout?: number } = {}
): Promise<T> => {
  const token = getAuthToken()
  const correlationId = generateCorrelationId()
  
  const headers = {
    ...defaultOptions.headers,
    ...options.headers,
    'X-Correlation-ID': correlationId,
    ...(token && { Authorization: `Bearer ${token}` })
  }

  const controller = new AbortController()
  const timeoutId = setTimeout(() => controller.abort(), options.timeout || defaultOptions.timeout)

  try {
    const fullUrl = url.startsWith('http') ? url : `${API_BASE_URL}${url}`
    
    console.log(`🚀 API Request: ${fullUrl}`, {
      method: options.method || 'GET',
      correlationId
    })

    const response = await fetch(fullUrl, {
      ...options,
      headers,
      signal: controller.signal
    })

    clearTimeout(timeoutId)

    console.log(`✅ API Response: ${fullUrl}`, {
      status: response.status,
      statusText: response.statusText
    })

    if (!response.ok) {
      // Handle specific error cases
      if (response.status === 401) {
        console.warn('Unauthorized access - clearing auth token')
        if (typeof localStorage !== 'undefined') {
          localStorage.removeItem('auth-token')
        }
      }

      if (response.status === 403) {
        console.warn('Access forbidden')
      }

      if (response.status >= 500) {
        console.error('Server error occurred')
      }

      throw new Error(`HTTP ${response.status}: ${response.statusText}`)
    }

    const contentType = response.headers.get('Content-Type')
    if (contentType?.includes('application/json')) {
      return await response.json()
    }
    
    return await response.text() as T
  } catch (error) {
    clearTimeout(timeoutId)
    
    console.error(`❌ API Error: ${url}`, {
      error: error instanceof Error ? error.message : 'Unknown error'
    })

    if (error instanceof Error && error.name === 'AbortError') {
      throw new Error('Request timeout')
    }
    throw error
  }
}

// Mock data flag
export const USE_MOCK_DATA = (config.public as any)?.enableMockApi || process.env.NODE_ENV === 'development'

// Mock delay for development
export function mockDelay(ms: number = 500): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms))
}

// Generic API functions
export async function apiGet<T>(endpoint: string, options?: RequestInit): Promise<T> {
  try {
    const config = getConfig()
    const response = await fetch(`${config.public.gatewayUrl}${endpoint}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers
      },
      ...options
    })
    
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`)
    }
    
    return await response.json()
  } catch (error) {
    throw handleApiError(error)
  }
}

export async function apiPost<T>(endpoint: string, data?: any, options?: FetchOptions): Promise<T> {
  try {
    return await apiClient<T>(endpoint, {
      method: 'POST',
      body: data,
      ...options
    })
  } catch (error) {
    throw handleApiError(error)
  }
}

export async function apiPut<T>(endpoint: string, data?: any, options?: FetchOptions): Promise<T> {
  try {
    return await apiClient<T>(endpoint, {
      method: 'PUT',
      body: data,
      ...options
    })
  } catch (error) {
    throw handleApiError(error)
  }
}

export async function apiPatch<T>(endpoint: string, data?: any, options?: FetchOptions): Promise<T> {
  try {
    return await apiClient<T>(endpoint, {
      method: 'PATCH',
      body: data,
      ...options
    })
  } catch (error) {
    throw handleApiError(error)
  }
}

export async function apiDelete<T>(endpoint: string, options?: RequestInit): Promise<T> {
  try {
    const config = getConfig()
    const response = await fetch(`${config.public.gatewayUrl}${endpoint}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers
      },
      ...options
    })
    
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`)
    }
    
    return await response.json()
  } catch (error) {
    throw handleApiError(error)
  }
}

// Error handling
function handleApiError(error: any): ApiError {
  const apiError: ApiError = {
    message: 'An unexpected error occurred',
    status: error.response?.status,
    statusText: error.response?.statusText,
    data: error.response?._data
  }
  
  if (error.response) {
    // Server responded with error status
    apiError.message = error.response._data?.message || 
                     error.response.statusText || 
                     `HTTP ${error.response.status} Error`
  } else if (error.request) {
    // Request was made but no response received
    apiError.message = 'Network error - please check your connection'
  } else {
    // Something else happened
    apiError.message = error.message || 'An unexpected error occurred'
  }
  
  return apiError
}

// Health check
export async function checkApiHealth(): Promise<boolean> {
  try {
    const response = await apiGet<{ status: string }>(API_ENDPOINTS.health)
    return response?.status === 'healthy'
  } catch (error) {
    console.error('Health check failed:', error)
    return false
  }
}

// Version info
export async function getApiVersion(): Promise<string> {
  try {
    const response = await apiGet<{ version: string }>(API_ENDPOINTS.version)
    return response?.version || 'unknown'
  } catch (error) {
    console.error('Version check failed:', error)
    return 'unknown'
  }
}

// Export for global use
export default apiClient

