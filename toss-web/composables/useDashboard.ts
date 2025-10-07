import { ref, computed } from 'vue'

export interface DashboardMetrics {
  totalRevenue: number
  totalExpenses: number
  netProfit: number
  totalOrders: number
  totalCustomers: number
  lowStockItems: number
  pendingOrders: number
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
  const metrics = ref<DashboardMetrics | null>(null)
  const salesData = ref<SalesData[]>([])
  const topProducts = ref<TopProduct[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const { getAuthHeader } = useAuth()
  const apiBaseUrl = useRuntimeConfig().public.apiBaseUrl || 'http://localhost:5000'

  /**
   * Fetch dashboard metrics
   */
  const fetchMetrics = async (startDate?: Date, endDate?: Date) => {
    isLoading.value = true
    error.value = null

    try {
      const params = new URLSearchParams()
      if (startDate) params.append('startDate', startDate.toISOString())
      if (endDate) params.append('endDate', endDate.toISOString())

      metrics.value = await $fetch<DashboardMetrics>(
        `${apiBaseUrl}/api/dashboard/metrics?${params}`,
        {
          headers: getAuthHeader(),
        }
      )
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch metrics'
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Fetch sales trend data
   */
  const fetchSalesTrend = async (days: number = 30) => {
    try {
      salesData.value = await $fetch<SalesData[]>(
        `${apiBaseUrl}/api/dashboard/sales-trend?days=${days}`,
        {
          headers: getAuthHeader(),
        }
      )
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch sales trend'
    }
  }

  /**
   * Fetch top selling products
   */
  const fetchTopProducts = async (limit: number = 10) => {
    try {
      topProducts.value = await $fetch<TopProduct[]>(
        `${apiBaseUrl}/api/dashboard/top-products?limit=${limit}`,
        {
          headers: getAuthHeader(),
        }
      )
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch top products'
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

