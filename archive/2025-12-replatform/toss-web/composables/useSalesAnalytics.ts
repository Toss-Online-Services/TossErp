/**
 * Sales Analytics Composable
 * Provides comprehensive sales reporting, trends analysis, and forecasting
 */

interface SalesMetrics {
  totalRevenue: number
  totalOrders: number
  averageOrderValue: number
  totalCustomers: number
  repeatCustomerRate: number
  returnRate: number
  grossProfit: number
  grossProfitMargin: number
}

interface SalesTrend {
  date: string
  revenue: number
  orders: number
  customers: number
  averageValue: number
}

interface ProductPerformance {
  productId: string
  productName: string
  quantitySold: number
  revenue: number
  profit: number
  profitMargin: number
  returnRate: number
  ranking: number
}

interface CustomerAnalytics {
  customerId: string
  customerName: string
  totalPurchases: number
  totalSpent: number
  averageOrderValue: number
  lastPurchaseDate: string
  lifetimeValue: number
  segment: 'new' | 'active' | 'at_risk' | 'churned' | 'vip'
}

interface SalesAnalyticsFilters {
  dateFrom: string
  dateTo: string
  customerId?: string
  productId?: string
  categoryId?: string
  territoryId?: string
  salesPersonId?: string
}

export const useSalesAnalytics = () => {
  const { api } = useApi()
  const { t } = useI18n()
  
  // State
  const metrics = ref<SalesMetrics | null>(null)
  const trends = ref<SalesTrend[]>([])
  const topProducts = ref<ProductPerformance[]>([])
  const customerAnalytics = ref<CustomerAnalytics[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Fetch sales metrics
  const fetchMetrics = async (filters: SalesAnalyticsFilters) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/metrics', {
        params: filters
      })
      metrics.value = response.data
      return metrics.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch sales trends
  const fetchTrends = async (filters: SalesAnalyticsFilters, groupBy: 'day' | 'week' | 'month' = 'day') => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/trends', {
        params: { ...filters, groupBy }
      })
      trends.value = response.data
      return trends.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch product performance
  const fetchProductPerformance = async (filters: SalesAnalyticsFilters, limit: number = 10) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/products', {
        params: { ...filters, limit }
      })
      topProducts.value = response.data
      return topProducts.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch customer analytics
  const fetchCustomerAnalytics = async (filters: SalesAnalyticsFilters) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/customers', {
        params: filters
      })
      customerAnalytics.value = response.data
      return customerAnalytics.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch sales by category
  const fetchSalesByCategory = async (filters: SalesAnalyticsFilters) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/by-category', {
        params: filters
      })
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch sales by territory
  const fetchSalesByTerritory = async (filters: SalesAnalyticsFilters) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/by-territory', {
        params: filters
      })
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch sales by payment method
  const fetchSalesByPaymentMethod = async (filters: SalesAnalyticsFilters) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/by-payment-method', {
        params: filters
      })
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Generate sales forecast
  const generateForecast = async (filters: SalesAnalyticsFilters, periods: number = 30) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/api/sales/analytics/forecast', {
        ...filters,
        periods
      })
      return response.data
    } catch (err: any) {
      error.value = err.message || t('analytics.errors.forecastFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get cohort analysis
  const getCohortAnalysis = async (filters: Omit<SalesAnalyticsFilters, 'dateFrom' | 'dateTo'> & {
    cohortType: 'month' | 'quarter' | 'year'
    startDate: string
    endDate: string
  }) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/cohort', {
        params: filters
      })
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get customer lifetime value
  const getCustomerLifetimeValue = async (customerId: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get(`/api/sales/analytics/customers/${customerId}/lifetime-value`)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get sales funnel metrics
  const getSalesFunnel = async (filters: SalesAnalyticsFilters) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/funnel', {
        params: filters
      })
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Export analytics data
  const exportAnalytics = async (
    filters: SalesAnalyticsFilters,
    format: 'csv' | 'excel' | 'pdf',
    reportType: 'summary' | 'detailed' | 'trends' | 'products' | 'customers'
  ) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/export', {
        params: { ...filters, format, reportType },
        responseType: 'blob'
      })
      
      // Download file
      const blob = new Blob([response], {
        type: format === 'pdf' ? 'application/pdf' : 
              format === 'excel' ? 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' :
              'text/csv'
      })
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `sales-analytics-${reportType}-${new Date().toISOString().split('T')[0]}.${format}`
      link.click()
      window.URL.revokeObjectURL(url)
      
      return response
    } catch (err: any) {
      error.value = err.message || t('errors.exportFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Compare periods
  const comparePeriods = async (
    currentPeriod: { dateFrom: string, dateTo: string },
    previousPeriod: { dateFrom: string, dateTo: string }
  ) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/api/sales/analytics/compare', {
        currentPeriod,
        previousPeriod
      })
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get real-time dashboard data
  const getRealTimeDashboard = async () => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/analytics/realtime')
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Calculate growth rate
  const calculateGrowthRate = (current: number, previous: number): number => {
    if (previous === 0) return current > 0 ? 100 : 0
    return ((current - previous) / previous) * 100
  }

  // Format currency
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR'
    }).format(amount)
  }

  // Format percentage
  const formatPercentage = (value: number, decimals: number = 1): string => {
    return `${value.toFixed(decimals)}%`
  }

  return {
    // State
    metrics,
    trends,
    topProducts,
    customerAnalytics,
    loading,
    error,
    
    // Methods
    fetchMetrics,
    fetchTrends,
    fetchProductPerformance,
    fetchCustomerAnalytics,
    fetchSalesByCategory,
    fetchSalesByTerritory,
    fetchSalesByPaymentMethod,
    generateForecast,
    getCohortAnalysis,
    getCustomerLifetimeValue,
    getSalesFunnel,
    exportAnalytics,
    comparePeriods,
    getRealTimeDashboard,
    
    // Utilities
    calculateGrowthRate,
    formatCurrency,
    formatPercentage
  }
}
