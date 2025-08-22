// Enhanced Gateway API Service for TOSS ERP III
// Provides comprehensive integration with backend services

interface ApiResponse<T = any> {
  success: boolean
  data?: T
  error?: string
  message?: string
}

interface DashboardData {
  todayMoney: number
  todayMoneyChange: number
  todayUsers: number
  todayUsersChange: number
  newClients: number
  newClientsChange: number
  sales: number
  salesChange: number
  globalSales: number
  reachedUsers: number
  activeUsers: number
  activeUsersChange: number
  users: number
  clicks: number
  salesAmount: number
  items: number
  salesOverviewChange: number
  salesByCountry: Array<{
    name: string
    sales: number
    value: number
    bounce: number
    color: string
  }>
}

interface AnalyticsData {
  totalRevenue: number
  revenueChange: number
  totalOrders: number
  ordersChange: number
  averageOrderValue: number
  aovChange: number
  conversionRate: number
  conversionChange: number
  newCustomers: number
  newCustomersChange: number
  repeatCustomers: number
  repeatCustomersChange: number
  customerLifetimeValue: number
  clvChange: number
  topProducts: Array<{
    id: string
    name: string
    category: string
    revenue: number
    orders: number
  }>
  topCategories: Array<{
    id: string
    name: string
    products: number
    revenue: number
    orders: number
  }>
}

interface ChatbotMessage {
  id: string
  text: string
  sender: 'user' | 'bot'
  timestamp: Date
  actions?: Array<{
    type: string
    label: string
    data?: any
  }>
}

interface ChatbotResponse {
  reply: string
  actions?: Array<{
    type: string
    label: string
    data?: any
  }>
}

interface StockItem {
  id: string
  name: string
  category: string
  quantity: number
  price: number
  supplier: string
  lastUpdated: Date
  status: 'in-stock' | 'low-stock' | 'out-of-stock'
}

interface SalesOrder {
  id: string
  customerName: string
  items: Array<{
    productId: string
    name: string
    quantity: number
    price: number
  }>
  total: number
  status: 'pending' | 'processing' | 'shipped' | 'delivered' | 'cancelled'
  orderDate: Date
}

interface FinancialRecord {
  id: string
  type: 'income' | 'expense'
  category: string
  amount: number
  description: string
  date: Date
}

class GatewayApiService {
  private baseUrl: string
  private timeout: number

  constructor() {
    // Mock runtime config
    const config = {
      public: {
        gatewayUrl: 'http://localhost:8080',
        apiTimeout: 30000
      }
    }
    this.baseUrl = config.public.gatewayUrl || 'http://localhost:8080'
    this.timeout = config.public.apiTimeout || 30000
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<ApiResponse<T>> {
    try {
      const url = `${this.baseUrl}${endpoint}`
      const defaultOptions: RequestInit = {
        headers: {
          'Content-Type': 'application/json',
          ...options.headers,
        },
      }

      const response = await fetch(url, {
        ...defaultOptions,
        ...options,
      })

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`)
      }

      const data = await response.json() as T
      return {
        success: true,
        data
      } as ApiResponse<T>
    } catch (error: any) {
      console.error(`API Error (${endpoint}):`, error)
      return {
        success: false,
        error: error.message || 'Network error occurred',
      }
    }
  }

  // Dashboard API Methods
  async getDashboardData(): Promise<ApiResponse<DashboardData>> {
    return this.request<DashboardData>('/api/dashboard/overview')
  }

  async getDashboardStats(period: string = '30d'): Promise<ApiResponse<DashboardData>> {
    return this.request<DashboardData>(`/api/dashboard/stats?period=${period}`)
  }

  // Analytics API Methods
  async getAnalyticsData(period: string = '30d'): Promise<ApiResponse<AnalyticsData>> {
    return this.request<AnalyticsData>(`/api/analytics/overview?period=${period}`)
  }

  async getRevenueTrend(period: string = '30d'): Promise<ApiResponse<any>> {
    return this.request(`/api/analytics/revenue-trend?period=${period}`)
  }

  async getOrdersTrend(period: string = '30d'): Promise<ApiResponse<any>> {
    return this.request(`/api/analytics/orders-trend?period=${period}`)
  }

  async getTopProducts(limit: number = 10): Promise<ApiResponse<any>> {
    return this.request(`/api/analytics/top-products?limit=${limit}`)
  }

  async getTopCategories(limit: number = 10): Promise<ApiResponse<any>> {
    return this.request(`/api/analytics/top-categories?limit=${limit}`)
  }

  async getCustomerInsights(): Promise<ApiResponse<any>> {
    return this.request('/api/analytics/customer-insights')
  }

  // Chatbot API Methods
  async sendChatbotMessage(message: string, context?: any): Promise<ApiResponse<ChatbotResponse>> {
    return this.request<ChatbotResponse>('/api/chatbot/message', {
      method: 'POST',
      body: JSON.stringify({ message, context }),
    })
  }

  async getChatbotHistory(): Promise<ApiResponse<ChatbotMessage[]>> {
    return this.request<ChatbotMessage[]>('/api/chatbot/history')
  }

  async clearChatbotHistory(): Promise<ApiResponse<void>> {
    return this.request<void>('/api/chatbot/history', {
      method: 'DELETE',
    })
  }

  async uploadFileToChatbot(file: File): Promise<ApiResponse<any>> {
    const formData = new FormData()
    formData.append('file', file)
    
    return this.request('/api/chatbot/upload', {
      method: 'POST',
      body: formData,
      headers: {
        // Don't set Content-Type for FormData
      },
    })
  }

  // Stock Management API Methods
  async getStockItems(filters?: any): Promise<ApiResponse<StockItem[]>> {
    const queryParams = filters ? new URLSearchParams(filters).toString() : ''
    return this.request<StockItem[]>(`/api/stock/items${queryParams ? `?${queryParams}` : ''}`)
  }

  async getStockItem(id: string): Promise<ApiResponse<StockItem>> {
    return this.request<StockItem>(`/api/stock/items/${id}`)
  }

  async createStockItem(item: Omit<StockItem, 'id' | 'lastUpdated'>): Promise<ApiResponse<StockItem>> {
    return this.request<StockItem>('/api/stock/items', {
      method: 'POST',
      body: JSON.stringify(item),
    })
  }

  async updateStockItem(id: string, updates: Partial<StockItem>): Promise<ApiResponse<StockItem>> {
    return this.request<StockItem>(`/api/stock/items/${id}`, {
      method: 'PUT',
      body: JSON.stringify(updates),
    })
  }

  async deleteStockItem(id: string): Promise<ApiResponse<void>> {
    return this.request<void>(`/api/stock/items/${id}`, {
      method: 'DELETE',
    })
  }

  async getLowStockItems(): Promise<ApiResponse<StockItem[]>> {
    return this.request<StockItem[]>('/api/stock/low-stock')
  }

  async getStockAlerts(): Promise<ApiResponse<any[]>> {
    return this.request('/api/stock/alerts')
  }

  // Sales API Methods
  async getSalesOrders(filters?: any): Promise<ApiResponse<SalesOrder[]>> {
    const queryParams = filters ? new URLSearchParams(filters).toString() : ''
    return this.request<SalesOrder[]>(`/api/sales/orders${queryParams ? `?${queryParams}` : ''}`)
  }

  async getSalesOrder(id: string): Promise<ApiResponse<SalesOrder>> {
    return this.request<SalesOrder>(`/api/sales/orders/${id}`)
  }

  async createSalesOrder(order: Omit<SalesOrder, 'id' | 'orderDate'>): Promise<ApiResponse<SalesOrder>> {
    return this.request<SalesOrder>('/api/sales/orders', {
      method: 'POST',
      body: JSON.stringify(order),
    })
  }

  async updateSalesOrder(id: string, updates: Partial<SalesOrder>): Promise<ApiResponse<SalesOrder>> {
    return this.request<SalesOrder>(`/api/sales/orders/${id}`, {
      method: 'PUT',
      body: JSON.stringify(updates),
    })
  }

  async getSalesReport(period: string = '30d'): Promise<ApiResponse<any>> {
    return this.request(`/api/sales/report?period=${period}`)
  }

  // Finance API Methods
  async getFinancialRecords(filters?: any): Promise<ApiResponse<FinancialRecord[]>> {
    const queryParams = filters ? new URLSearchParams(filters).toString() : ''
    return this.request<FinancialRecord[]>(`/api/finance/records${queryParams ? `?${queryParams}` : ''}`)
  }

  async createFinancialRecord(record: Omit<FinancialRecord, 'id'>): Promise<ApiResponse<FinancialRecord>> {
    return this.request<FinancialRecord>('/api/finance/records', {
      method: 'POST',
      body: JSON.stringify(record),
    })
  }

  async getFinancialSummary(period: string = '30d'): Promise<ApiResponse<any>> {
    return this.request(`/api/finance/summary?period=${period}`)
  }

  async getProfitLossReport(period: string = '30d'): Promise<ApiResponse<any>> {
    return this.request(`/api/finance/profit-loss?period=${period}`)
  }

  // Export API Methods
  async exportData(type: string, format: 'json' | 'csv' | 'xlsx' = 'xlsx', filters?: any): Promise<ApiResponse<any>> {
    const queryParams = new URLSearchParams({
      type,
      format,
      ...filters,
    }).toString()
    
    return this.request(`/api/export/data?${queryParams}`)
  }

  async exportReport(reportType: string, format: 'pdf' | 'xlsx' = 'pdf', filters?: any): Promise<ApiResponse<any>> {
    const queryParams = new URLSearchParams({
      reportType,
      format,
      ...filters,
    }).toString()
    
    return this.request(`/api/export/report?${queryParams}`)
  }

  // System API Methods
  async getSystemHealth(): Promise<ApiResponse<any>> {
    return this.request('/api/system/health')
  }

  async getSystemMetrics(): Promise<ApiResponse<any>> {
    return this.request('/api/system/metrics')
  }

  async getSystemLogs(level?: string, limit?: number): Promise<ApiResponse<any[]>> {
    const queryParams = new URLSearchParams()
    if (level) queryParams.append('level', level)
    if (limit) queryParams.append('limit', limit.toString())
    
    return this.request(`/api/system/logs${queryParams.toString() ? `?${queryParams.toString()}` : ''}`)
  }

  // User Management API Methods
  async getCurrentUser(): Promise<ApiResponse<any>> {
    return this.request('/api/user/profile')
  }

  async updateUserProfile(updates: any): Promise<ApiResponse<any>> {
    return this.request('/api/user/profile', {
      method: 'PUT',
      body: JSON.stringify(updates),
    })
  }

  async changePassword(currentPassword: string, newPassword: string): Promise<ApiResponse<void>> {
    return this.request<void>('/api/user/change-password', {
      method: 'POST',
      body: JSON.stringify({ currentPassword, newPassword }),
    })
  }

  // Notification API Methods
  async getNotifications(): Promise<ApiResponse<any[]>> {
    return this.request('/api/notifications')
  }

  async markNotificationAsRead(id: string): Promise<ApiResponse<void>> {
    return this.request<void>(`/api/notifications/${id}/read`, {
      method: 'PUT',
    })
  }

  async markAllNotificationsAsRead(): Promise<ApiResponse<void>> {
    return this.request<void>('/api/notifications/read-all', {
      method: 'PUT',
    })
  }

  // Settings API Methods
  async getUserSettings(): Promise<ApiResponse<any>> {
    return this.request('/api/settings/user')
  }

  async updateUserSettings(settings: any): Promise<ApiResponse<any>> {
    return this.request('/api/settings/user', {
      method: 'PUT',
      body: JSON.stringify(settings),
    })
  }

  async getSystemSettings(): Promise<ApiResponse<any>> {
    return this.request('/api/settings/system')
  }

  // Utility Methods
  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(amount)
  }

  formatDate(date: Date): string {
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    }).format(new Date(date))
  }

  formatDateTime(date: Date): string {
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(new Date(date))
  }

  // Error Handling
  handleApiError(error: any): string {
    if (error.response?.status === 401) {
      return 'Authentication required. Please log in again.'
    } else if (error.response?.status === 403) {
      return 'Access denied. You do not have permission to perform this action.'
    } else if (error.response?.status === 404) {
      return 'Resource not found.'
    } else if (error.response?.status === 500) {
      return 'Server error. Please try again later.'
    } else if (error.code === 'NETWORK_ERROR') {
      return 'Network error. Please check your connection and try again.'
    } else {
      return error.message || 'An unexpected error occurred.'
    }
  }

  // Retry Logic
  async retryRequest<T>(
    requestFn: () => Promise<ApiResponse<T>>,
    maxRetries: number = 3,
    delay: number = 1000
  ): Promise<ApiResponse<T>> {
    let lastError: any

    for (let attempt = 1; attempt <= maxRetries; attempt++) {
      try {
        const result = await requestFn()
        if (result.success) {
          return result
        }
        lastError = result.error
      } catch (error: any) {
        lastError = error
        if (attempt === maxRetries) {
          break
        }
        await new Promise(resolve => setTimeout(resolve, delay * attempt))
      }
    }

    return {
      success: false,
      error: this.handleApiError(lastError),
    }
  }
}

// Create and export singleton instance
export const gatewayApi = new GatewayApiService()

// Export types for use in components
export type {
  ApiResponse,
  DashboardData,
  AnalyticsData,
  ChatbotMessage,
  ChatbotResponse,
  StockItem,
  SalesOrder,
  FinancialRecord,
}
