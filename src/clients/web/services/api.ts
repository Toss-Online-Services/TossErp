import type { StockItem, StockMovement } from '../stores/stock'

// API Configuration - Use Nuxt runtime config
const getApiConfig = () => {
  // Check if we're in browser
  if (typeof window !== 'undefined') {
    // Client-side: use window.__NUXT__ or composables
    return {
      baseUrl: 'http://localhost:8080/api', // Fallback
      timeout: 30000
    }
  }
  
  // Server-side: use environment variables
  return {
    baseUrl: process.env.NUXT_PUBLIC_API_URL || 'http://localhost:8080/api',
    timeout: parseInt(process.env.API_TIMEOUT || '30000')
  }
}

const config = getApiConfig()
const API_BASE_URL = config.baseUrl
const API_TIMEOUT = config.timeout

// API Response Types
interface ApiResponse<T> {
  data: T
  message?: string
  success: boolean
}

interface PaginatedResponse<T> {
  data: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

interface ApiError {
  message: string
  statusCode: number
  errors?: Record<string, string[]>
}

// HTTP Client with timeout and error handling
class ApiClient {
  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${API_BASE_URL}${endpoint}`
    const controller = new AbortController()
    const timeoutId = setTimeout(() => controller.abort(), API_TIMEOUT)

    try {
      const response = await fetch(url, {
        ...options,
        signal: controller.signal,
        headers: {
          'Content-Type': 'application/json',
          'User-Agent': 'TossErp-Web/1.0.0',
          'X-Client-Type': 'web',
          'X-Client-Version': '1.0.0',
          ...options.headers,
        },
      })

      clearTimeout(timeoutId)

      if (!response.ok) {
        const errorData: ApiError = await response.json().catch(() => ({
          message: `HTTP ${response.status}: ${response.statusText}`,
          statusCode: response.status,
        }))
        throw new Error(errorData.message)
      }

      return await response.json()
    } catch (error) {
      clearTimeout(timeoutId)
      if (error instanceof Error) {
        if (error.name === 'AbortError') {
          throw new Error('Request timeout')
        }
        throw error
      }
      throw new Error('Unknown error occurred')
    }
  }

  // GET request
  async get<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: 'GET' })
  }

  // POST request
  async post<T>(endpoint: string, data?: any): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    })
  }

  // PUT request
  async put<T>(endpoint: string, data?: any): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    })
  }

  // DELETE request
  async delete<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: 'DELETE' })
  }

  // PATCH request
  async patch<T>(endpoint: string, data?: any): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'PATCH',
      body: data ? JSON.stringify(data) : undefined,
    })
  }

  // Health check
  async healthCheck(): Promise<boolean> {
    try {
      const response = await fetch(`${API_BASE_URL.replace('/api', '')}/health`)
      return response.ok
    } catch {
      return false
    }
  }
}

// Stock API Service
export class StockApiService {
  private client = new ApiClient()

  // Items
  async getItems(params?: {
    page?: number
    pageSize?: number
    search?: string
    category?: string
    warehouse?: string
    stockLevel?: string
  }): Promise<PaginatedResponse<StockItem>> {
    const searchParams = new URLSearchParams()
    if (params?.page) searchParams.append('page', params.page.toString())
    if (params?.pageSize) searchParams.append('pageSize', params.pageSize.toString())
    if (params?.search) searchParams.append('search', params.search)
    if (params?.category) searchParams.append('category', params.category)
    if (params?.warehouse) searchParams.append('warehouse', params.warehouse)
    if (params?.stockLevel) searchParams.append('stockLevel', params.stockLevel)

    const queryString = searchParams.toString()
    const endpoint = `/stock/items${queryString ? `?${queryString}` : ''}`
    
    return this.client.get<PaginatedResponse<StockItem>>(endpoint)
  }

  async getItemById(id: number): Promise<StockItem> {
    return this.client.get<StockItem>(`/stock/items/${id}`)
  }

  async createItem(item: Omit<StockItem, 'id' | 'lastUpdated'>): Promise<StockItem> {
    return this.client.post<StockItem>('/stock/items', item)
  }

  async updateItem(id: number, item: Partial<StockItem>): Promise<StockItem> {
    return this.client.put<StockItem>(`/stock/items/${id}`, item)
  }

  async deleteItem(id: number): Promise<void> {
    return this.client.delete<void>(`/stock/items/${id}`)
  }

  // Stock Movements
  async getMovements(params?: {
    page?: number
    pageSize?: number
    itemId?: number
    warehouse?: string
    type?: string
    startDate?: string
    endDate?: string
  }): Promise<PaginatedResponse<StockMovement>> {
    const searchParams = new URLSearchParams()
    if (params?.page) searchParams.append('page', params.page.toString())
    if (params?.pageSize) searchParams.append('pageSize', params.pageSize.toString())
    if (params?.itemId) searchParams.append('itemId', params.itemId.toString())
    if (params?.warehouse) searchParams.append('warehouse', params.warehouse)
    if (params?.type) searchParams.append('type', params.type)
    if (params?.startDate) searchParams.append('startDate', params.startDate)
    if (params?.endDate) searchParams.append('endDate', params.endDate)

    const queryString = searchParams.toString()
    const endpoint = `/stock/movements${queryString ? `?${queryString}` : ''}`
    
    return this.client.get<PaginatedResponse<StockMovement>>(endpoint)
  }

  async createMovement(movement: Omit<StockMovement, 'id' | 'timestamp'>): Promise<StockMovement> {
    return this.client.post<StockMovement>('/stock/movements', movement)
  }

  // Stock Levels
  async getStockLevels(params?: {
    warehouse?: string
    lowStock?: boolean
  }): Promise<StockItem[]> {
    const searchParams = new URLSearchParams()
    if (params?.warehouse) searchParams.append('warehouse', params.warehouse)
    if (params?.lowStock !== undefined) searchParams.append('lowStock', params.lowStock.toString())

    const queryString = searchParams.toString()
    const endpoint = `/stock/levels${queryString ? `?${queryString}` : ''}`
    
    return this.client.get<StockItem[]>(endpoint)
  }

  // Bulk Operations
  async bulkImport(file: File): Promise<{ success: boolean; message: string; imported: number; errors: number }> {
    const formData = new FormData()
    formData.append('file', file)
    
    return this.client.post<{ success: boolean; message: string; imported: number; errors: number }>(
      '/stock/items/bulk-import',
      formData
    )
  }

  async bulkExport(params?: {
    format?: 'csv' | 'excel'
    category?: string
    warehouse?: string
  }): Promise<Blob> {
    const searchParams = new URLSearchParams()
    if (params?.format) searchParams.append('format', params.format)
    if (params?.category) searchParams.append('category', params.category)
    if (params?.warehouse) searchParams.append('warehouse', params.warehouse)

    const queryString = searchParams.toString()
    const endpoint = `/stock/items/bulk-export${queryString ? `?${queryString}` : ''}`
    
    const response = await fetch(`${API_BASE_URL}${endpoint}`)
    if (!response.ok) {
      throw new Error(`Export failed: ${response.statusText}`)
    }
    return response.blob()
  }

  // Reports
  async getStockValuationReport(params?: {
    warehouse?: string
    category?: string
    asOf?: string
  }): Promise<{
    totalValue: number
    itemCount: number
    categoryBreakdown: Array<{ category: string; value: number; count: number }>
  }> {
    const searchParams = new URLSearchParams()
    if (params?.warehouse) searchParams.append('warehouse', params.warehouse)
    if (params?.category) searchParams.append('category', params.category)
    if (params?.asOf) searchParams.append('asOf', params.asOf)

    const queryString = searchParams.toString()
    const endpoint = `/stock/reports/valuation${queryString ? `?${queryString}` : ''}`
    
    return this.client.get(endpoint)
  }

  async getMovementSummary(params?: {
    startDate: string
    endDate: string
    warehouse?: string
    type?: string
  }): Promise<{
    totalIn: number
    totalOut: number
    netChange: number
    movements: StockMovement[]
  }> {
    const searchParams = new URLSearchParams()
    if (params) {
      searchParams.append('startDate', params.startDate)
      searchParams.append('endDate', params.endDate)
      if (params.warehouse) searchParams.append('warehouse', params.warehouse)
      if (params.type) searchParams.append('type', params.type)
    }

    const queryString = searchParams.toString()
    const endpoint = `/stock/reports/movements${queryString ? `?${queryString}` : ''}`
    
    return this.client.get(endpoint)
  }

  // Health check
  async healthCheck(): Promise<boolean> {
    return this.client.healthCheck()
  }
}

// Export singleton instance
export const stockApi = new StockApiService()

// Mock API Service for development (when backend is not available)
export class MockStockApiService {
  private delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms))
  }

  async getItems(params?: any): Promise<PaginatedResponse<StockItem>> {
    await this.delay(500) // Simulate network delay
    
    // Return mock data structure
    return {
      data: [],
      total: 0,
      page: params?.page || 1,
      pageSize: params?.pageSize || 10,
      totalPages: 0
    }
  }

  async getItemById(id: number): Promise<StockItem> {
    await this.delay(300)
    throw new Error('Mock API: Backend not available')
  }

  async createItem(item: any): Promise<StockItem> {
    await this.delay(500)
    throw new Error('Mock API: Backend not available')
  }

  async updateItem(id: number, item: any): Promise<StockItem> {
    await this.delay(500)
    throw new Error('Mock API: Backend not available')
  }

  async deleteItem(id: number): Promise<void> {
    await this.delay(300)
    throw new Error('Mock API: Backend not available')
  }

  async getMovements(params?: any): Promise<PaginatedResponse<StockMovement>> {
    await this.delay(500)
    return {
      data: [],
      total: 0,
      page: params?.page || 1,
      pageSize: params?.pageSize || 10,
      totalPages: 0
    }
  }

  async createMovement(movement: any): Promise<StockMovement> {
    await this.delay(500)
    throw new Error('Mock API: Backend not available')
  }

  async getStockLevels(params?: any): Promise<StockItem[]> {
    await this.delay(300)
    return []
  }

  async bulkImport(file: File): Promise<any> {
    await this.delay(1000)
    throw new Error('Mock API: Backend not available')
  }

  async bulkExport(params?: any): Promise<Blob> {
    await this.delay(1000)
    throw new Error('Mock API: Backend not available')
  }

  async getStockValuationReport(params?: any): Promise<any> {
    await this.delay(500)
    throw new Error('Mock API: Backend not available')
  }

  async getMovementSummary(params: any): Promise<any> {
    await this.delay(500)
    throw new Error('Mock API: Backend not available')
  }

  async healthCheck(): Promise<boolean> {
    return false // Mock service is never healthy
  }
}

// Export mock service for development
export const mockStockApi = new MockStockApiService()

// Helper function to determine which API to use
export function getStockApi() {
  // In development, you can switch between real and mock API
  const useMock = process.env.NODE_ENV === 'development' && process.env.NUXT_USE_MOCK_API === 'true'
  return useMock ? mockStockApi : stockApi
}
