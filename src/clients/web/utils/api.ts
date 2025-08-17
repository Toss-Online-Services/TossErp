// API utilities for TOSS ERP
import type { FetchOptions } from 'ofetch'

// Types
export interface ApiResponse<T = any> {
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

// Configuration
const config = useRuntimeConfig()

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
const defaultOptions: FetchOptions = {
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  },
  timeout: config.public.apiTimeout || 30000,
  retry: 2,
  retryDelay: 1000
}

// Custom fetch wrapper with error handling
export const apiClient = $fetch.create({
  ...defaultOptions,
  onRequest({ request, options }) {
    // Add authentication token if available
    const token = useCookie('auth-token')
    if (token.value) {
      options.headers = {
        ...options.headers,
        Authorization: `Bearer ${token.value}`
      }
    }
    
    // Add correlation ID for request tracking
    const correlationId = generateCorrelationId()
    options.headers = {
      ...options.headers,
      'X-Correlation-ID': correlationId
    }
    
    console.log(`🚀 API Request: ${request}`, {
      method: options.method || 'GET',
      correlationId,
      body: options.body
    })
  },
  
  onResponse({ request, response }) {
    console.log(`✅ API Response: ${request}`, {
      status: response.status,
      statusText: response.statusText
    })
  },
  
  onResponseError({ request, response, error }) {
    console.error(`❌ API Error: ${request}`, {
      status: response?.status,
      statusText: response?.statusText,
      error: error.message
    })
    
    // Handle specific error cases
    if (response?.status === 401) {
      // Unauthorized - redirect to login or refresh token
      console.warn('Unauthorized access - clearing auth token')
      const token = useCookie('auth-token')
      token.value = null
      
      // In a real app, you might want to redirect to login
      // await navigateTo('/login')
    }
    
    if (response?.status === 403) {
      // Forbidden - user doesn't have permission
      console.warn('Access forbidden')
      // Handle forbidden access
    }
    
    if (response?.status >= 500) {
      // Server error - show user-friendly message
      console.error('Server error occurred')
      // You might want to show a toast notification here
    }
    
    throw error
  }
})

// Utility functions
export function generateCorrelationId(): string {
  return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
}

// Mock data flag
export const USE_MOCK_DATA = config.public.enableMockApi || process.env.NODE_ENV === 'development'

// Mock delay for development
export function mockDelay(ms: number = 500): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms))
}

// Generic API functions
export async function apiGet<T>(endpoint: string, options?: FetchOptions): Promise<T> {
  try {
    return await apiClient<T>(endpoint, {
      method: 'GET',
      ...options
    })
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

export async function apiDelete<T>(endpoint: string, options?: FetchOptions): Promise<T> {
  try {
    return await apiClient<T>(endpoint, {
      method: 'DELETE',
      ...options
    })
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
    const response = await apiGet(API_ENDPOINTS.health)
    return response?.status === 'healthy'
  } catch (error) {
    console.error('Health check failed:', error)
    return false
  }
}

// Version info
export async function getApiVersion(): Promise<string> {
  try {
    const response = await apiGet(API_ENDPOINTS.version)
    return response?.version || 'unknown'
  } catch (error) {
    console.error('Version check failed:', error)
    return 'unknown'
  }
}

// Export for global use
export default apiClient
