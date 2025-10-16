import { ref } from 'vue'
import { MockDashboardService } from '~/services/mock'

export interface DashboardMetrics {
  totalRevenue: number
  totalExpenses?: number
  netProfit?: number
  totalOrders: number
  totalCustomers: number
  lowStockItems: number
  pendingOrders?: number
}

export interface SalesData {
  date: string
  revenue: number
  orders: number
}

export interface TopProduct {
  productId: number
  productName: string
  quantitySold: number
  revenue: number
}

export const useDashboard = () => {
  const { get, useMockData } = useApi()
  const metrics = ref<DashboardMetrics | null>(null)
  const salesData = ref<SalesData[]>([])
  const topProducts = ref<TopProduct[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  /**
   * Fetch dashboard metrics
   */
  const fetchMetrics = async (startDate?: Date, endDate?: Date) => {
    isLoading.value = true
    error.value = null

    try {
      if (useMockData()) {
        // Use mock data
        await new Promise(resolve => setTimeout(resolve, 300))
        metrics.value = MockDashboardService.getMetrics()
      } else {
        const params: Record<string, string> = {}
        if (startDate) params.startDate = startDate.toISOString()
        if (endDate) params.endDate = endDate.toISOString()

        metrics.value = await get<DashboardMetrics>('/api/dashboard/metrics', params)
      }
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch metrics'
      // Fallback to mock on error
      metrics.value = MockDashboardService.getMetrics()
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Fetch sales trend data
   */
  const fetchSalesTrend = async (days: number = 30) => {
    try {
      if (useMockData()) {
        // Generate mock sales trend
        await new Promise(resolve => setTimeout(resolve, 200))
        const mockData = []
        for (let i = days - 1; i >= 0; i--) {
          const date = new Date()
          date.setDate(date.getDate() - i)
          mockData.push({
            date: date.toISOString().split('T')[0],
            revenue: Math.floor(Math.random() * 50000) + 20000,
            orders: Math.floor(Math.random() * 50) + 10
          })
        }
        salesData.value = mockData
      } else {
        salesData.value = await get<SalesData[]>('/api/dashboard/sales-trend', { days })
      }
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch sales trend'
    }
  }

  /**
   * Fetch top selling products
   */
  const fetchTopProducts = async (limit: number = 10) => {
    try {
      if (useMockData()) {
        await new Promise(resolve => setTimeout(resolve, 200))
        topProducts.value = MockDashboardService.getTopProducts().slice(0, limit)
      } else {
        topProducts.value = await get<TopProduct[]>('/api/dashboard/top-products', { limit })
      }
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch top products'
      // Fallback to mock
      topProducts.value = MockDashboardService.getTopProducts().slice(0, limit)
    }
  }

  /**
   * Refresh all dashboard data
   */
  const refreshDashboard = async () => {
    await Promise.all([
      fetchMetrics(),
      fetchSalesTrend(),
      fetchTopProducts(),
    ])
  }

  return {
    metrics: readonly(metrics),
    salesData: readonly(salesData),
    topProducts: readonly(topProducts),
    isLoading: readonly(isLoading),
    error: readonly(error),
    fetchMetrics,
    fetchSalesTrend,
    fetchTopProducts,
    refreshDashboard,
  }
}


