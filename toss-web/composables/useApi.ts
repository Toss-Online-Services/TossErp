import type { UseFetchOptions } from 'nuxt/app'
import { 
  MockStockService, 
  MockLogisticsService, 
  MockPurchasingService, 
  MockSalesService,
  MockAutomationService,
  MockDashboardService 
} from '~/services/mock'

export const useApi = () => {
  const { getAuthHeader } = useAuth()
  const config = useRuntimeConfig()
  const apiBaseUrl = config.public.apiBaseUrl || 'http://localhost:5000'

  /**
   * Check if we should use mock data
   */
  const useMockData = () => {
    if (process.client) {
      const isOffline = !navigator.onLine
      const isDevelopment = process.env.NODE_ENV === 'development'
      return isOffline || isDevelopment
    }
    return true // Default to mock on server
  }

  /**
   * Route to appropriate mock service
   */
  const getMockResponse = <T>(endpoint: string, params?: Record<string, any>): T | null => {
    // Stock/Inventory endpoints
    if (endpoint.includes('/api/inventory/stock-levels')) {
      return MockStockService.getStockLevels() as T
    }
    if (endpoint.includes('/api/stock/items')) {
      return MockStockService.getAllItems() as T
    }
    if (endpoint.includes('/api/stock/movements')) {
      return MockStockService.getMovements() as T
    }
    if (endpoint.includes('/api/stock/warehouses')) {
      return MockStockService.getWarehouses() as T
    }

    // Logistics endpoints
    if (endpoint.includes('/api/logistics/drivers')) {
      return MockLogisticsService.getDrivers() as T
    }
    if (endpoint.includes('/api/logistics/jobs')) {
      const status = params?.status
      if (status === 'open') {
        return MockLogisticsService.getAvailableJobs() as T
      }
      if (status === 'assigned') {
        return MockLogisticsService.getAssignedJobs() as T
      }
      return [] as T
    }

    // Purchasing endpoints
    if (endpoint.includes('/api/purchasing/suppliers')) {
      return MockPurchasingService.getSuppliers() as T
    }
    if (endpoint.includes('/api/purchasing/orders')) {
      return MockPurchasingService.getPurchaseOrders() as T
    }
    if (endpoint.includes('/api/purchasing/invoices')) {
      return MockPurchasingService.getPurchaseInvoices() as T
    }
    if (endpoint.includes('/api/purchasing/group-buy')) {
      return MockPurchasingService.getGroupBuyOpportunities() as T
    }

    // Sales endpoints
    if (endpoint.includes('/api/sales/orders')) {
      return MockSalesService.getOrders() as T
    }
    if (endpoint.includes('/api/sales/quotations')) {
      return MockSalesService.getQuotations() as T
    }
    if (endpoint.includes('/api/sales/invoices')) {
      return MockSalesService.getInvoices() as T
    }
    if (endpoint.includes('/api/sales/pos/products')) {
      return MockSalesService.getPOSProducts() as T
    }
    if (endpoint.includes('/api/sales/pos/transactions')) {
      return MockSalesService.getPOSTransactions() as T
    }

    // Automation endpoints
    if (endpoint.includes('/api/automation/workflows')) {
      return MockAutomationService.getWorkflows() as T
    }
    if (endpoint.includes('/api/automation/triggers')) {
      return MockAutomationService.getTriggers() as T
    }
    if (endpoint.includes('/api/automation/executions')) {
      return MockAutomationService.getExecutions() as T
    }

    // Dashboard endpoints
    if (endpoint.includes('/api/dashboard/metrics')) {
      return MockDashboardService.getMetrics() as T
    }
    if (endpoint.includes('/api/dashboard/top-products')) {
      return MockDashboardService.getTopProducts() as T
    }
    if (endpoint.includes('/api/dashboard/activities')) {
      return MockDashboardService.getRecentActivities() as T
    }

    return null
  }

  /**
   * Make an authenticated API request (with mock fallback)
   */
  const request = async <T>(
    endpoint: string,
    options?: UseFetchOptions<T>
  ) => {
    // Return mock data if in mock mode
    if (useMockData()) {
      const mockData = getMockResponse<T>(endpoint, options?.params as Record<string, any>)
      if (mockData !== null) {
        // Simulate network delay
        await new Promise(resolve => setTimeout(resolve, 100 + Math.random() * 200))
        return mockData
      }
    }

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
      params
    })
  }

  /**
   * POST request
   */
  const post = async <T>(endpoint: string, body?: any) => {
    // Mock mode: simulate success
    if (useMockData()) {
      await new Promise(resolve => setTimeout(resolve, 300))
      return { success: true, data: body } as T
    }

    return await request<T>(endpoint, {
      method: 'POST',
      body,
    })
  }

  /**
   * PUT request
   */
  const put = async <T>(endpoint: string, body?: any) => {
    // Mock mode: simulate success
    if (useMockData()) {
      await new Promise(resolve => setTimeout(resolve, 300))
      return { success: true, data: body } as T
    }

    return await request<T>(endpoint, {
      method: 'PUT',
      body,
    })
  }

  /**
   * DELETE request
   */
  const del = async <T>(endpoint: string) => {
    // Mock mode: simulate success
    if (useMockData()) {
      await new Promise(resolve => setTimeout(resolve, 200))
      return { success: true } as T
    }

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
    useMockData,
  }
}


