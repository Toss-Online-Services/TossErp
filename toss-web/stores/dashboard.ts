import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface DashboardStats {
  todaySales: number
  cashIn: number
  cashOut: number
  lowStockItems: number
  pendingOrders: number
  overdueInvoices: number
  activeCustomers: number
}

export interface SalesTrend {
  day: string
  amount: number
  date: Date
}

export interface TopProduct {
  id: string
  name: string
  sales: number
  revenue: number
  trend: 'up' | 'down' | 'stable'
}

export interface QuickAction {
  id: string
  title: string
  icon: string
  iconBg: string
  route: string
  count?: number
}

export const useDashboardStore = defineStore('dashboard', () => {
  // State
  const stats = ref<DashboardStats>({
    todaySales: 0,
    cashIn: 0,
    cashOut: 0,
    lowStockItems: 0,
    pendingOrders: 0,
    overdueInvoices: 0,
    activeCustomers: 0
  })

  const salesTrend = ref<SalesTrend[]>([])
  const topProducts = ref<TopProduct[]>([])
  const loading = ref(false)
  const lastSync = ref<Date | null>(null)

  // Computed
  const netCash = computed(() => stats.value.cashIn - stats.value.cashOut)
  const salesGrowth = computed(() => {
    if (salesTrend.value.length < 2) return 0
    const today = salesTrend.value[salesTrend.value.length - 1].amount
    const yesterday = salesTrend.value[salesTrend.value.length - 2].amount
    return yesterday > 0 ? ((today - yesterday) / yesterday) * 100 : 0
  })

  // Actions
  async function fetchDashboardData() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 1000))
      
      // Mock data for now
      stats.value = {
        todaySales: 15420,
        cashIn: 12300,
        cashOut: 4500,
        lowStockItems: 8,
        pendingOrders: 3,
        overdueInvoices: 2,
        activeCustomers: 45
      }

      salesTrend.value = [
        { day: 'Mon', amount: 12000, date: new Date(2025, 11, 2) },
        { day: 'Tue', amount: 15000, date: new Date(2025, 11, 3) },
        { day: 'Wed', amount: 13500, date: new Date(2025, 11, 4) },
        { day: 'Thu', amount: 16000, date: new Date(2025, 11, 5) },
        { day: 'Fri', amount: 18000, date: new Date(2025, 11, 6) },
        { day: 'Sat', amount: 22000, date: new Date(2025, 11, 7) },
        { day: 'Sun', amount: 15420, date: new Date(2025, 11, 8) }
      ]

      topProducts.value = [
        { id: '1', name: 'Cement 50kg', sales: 120, revenue: 12000, trend: 'up' },
        { id: '2', name: 'Sugar 2.5kg', sales: 80, revenue: 8000, trend: 'up' },
        { id: '3', name: 'Cooking Oil 750ml', sales: 50, revenue: 5000, trend: 'stable' }
      ]

      lastSync.value = new Date()
    } catch (error) {
      console.error('Failed to fetch dashboard data:', error)
    } finally {
      loading.value = false
    }
  }

  function refreshStats() {
    return fetchDashboardData()
  }

  return {
    // State
    stats,
    salesTrend,
    topProducts,
    loading,
    lastSync,
    // Computed
    netCash,
    salesGrowth,
    // Actions
    fetchDashboardData,
    refreshStats
  }
})

