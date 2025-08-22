// Sales Management Composable
import { ref, computed, readonly } from 'vue'
import type { 
  Sale, 
  Customer, 
  SalesOverview, 
  TopProduct,
  CreateSaleRequest,
  CreateCustomerRequest 
} from '~/services/salesService'
import { SalesService } from '~/services/salesService'

export const useSalesManagement = () => {
  // Reactive state
  const sales = ref<Sale[]>([])
  const customers = ref<Customer[]>([])
  const salesOverview = ref<SalesOverview | null>(null)
  const topProducts = ref<TopProduct[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Computed properties
  const todaysSales = computed(() => salesOverview.value?.todaysSales || 0)
  const todaysOrders = computed(() => salesOverview.value?.todaysOrders || 0)
  const averageOrderValue = computed(() => salesOverview.value?.averageOrderValue || 0)
  const totalCustomers = computed(() => salesOverview.value?.totalCustomers || 0)
  const recentSales = computed(() => salesOverview.value?.recentSales || [])

  // Load sales overview
  const loadSalesOverview = async (period: 'today' | 'week' | 'month' = 'today') => {
    try {
      loading.value = true
      error.value = null
      salesOverview.value = await SalesService.getOverview(period)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load sales overview'
      console.error('Error loading sales overview:', err)
    } finally {
      loading.value = false
    }
  }

  // Load all sales
  const loadSales = async (params?: {
    customerId?: string
    status?: string
    startDate?: string
    endDate?: string
    page?: number
    limit?: number
  }) => {
    try {
      loading.value = true
      error.value = null
      sales.value = await SalesService.getSales(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load sales'
      console.error('Error loading sales:', err)
    } finally {
      loading.value = false
    }
  }

  // Get single sale
  const getSale = async (id: string): Promise<Sale | null> => {
    try {
      loading.value = true
      error.value = null
      return await SalesService.getSale(id)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load sale'
      console.error('Error loading sale:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Create new sale
  const createSale = async (data: CreateSaleRequest): Promise<Sale | null> => {
    try {
      loading.value = true
      error.value = null
      const newSale = await SalesService.createSale(data)
      
      // Add to local sales array if loaded
      if (sales.value.length > 0) {
        sales.value.unshift(newSale)
      }
      
      // Refresh overview to get updated stats
      await loadSalesOverview()
      
      return newSale
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create sale'
      console.error('Error creating sale:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Update sale status
  const updateSaleStatus = async (id: string, status: Sale['status']): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      const updatedSale = await SalesService.updateSaleStatus(id, status)
      
      // Update local sales array
      const index = sales.value.findIndex((sale: Sale) => sale.id === id)
      if (index !== -1) {
        sales.value[index] = updatedSale
      }
      
      // Update recent sales in overview if available
      if (salesOverview.value?.recentSales) {
        const recentIndex = salesOverview.value.recentSales.findIndex((sale: Sale) => sale.id === id)
        if (recentIndex !== -1) {
          salesOverview.value.recentSales[recentIndex] = updatedSale
        }
      }
      
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update sale status'
      console.error('Error updating sale status:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Cancel sale
  const cancelSale = async (id: string, reason?: string): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      await SalesService.cancelSale(id, reason)
      
      // Update local sales
      const index = sales.value.findIndex((sale: Sale) => sale.id === id)
      if (index !== -1) {
        sales.value[index].status = 'cancelled'
      }
      
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to cancel sale'
      console.error('Error cancelling sale:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Process refund
  const refundSale = async (id: string, amount?: number, reason?: string): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      await SalesService.refundSale(id, amount, reason)
      
      // Update local sales
      const index = sales.value.findIndex((sale: Sale) => sale.id === id)
      if (index !== -1) {
        sales.value[index].status = 'refunded'
      }
      
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to process refund'
      console.error('Error processing refund:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Load customers
  const loadCustomers = async (params?: {
    search?: string
    page?: number
    limit?: number
  }) => {
    try {
      loading.value = true
      error.value = null
      customers.value = await SalesService.getCustomers(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load customers'
      console.error('Error loading customers:', err)
    } finally {
      loading.value = false
    }
  }

  // Get single customer
  const getCustomer = async (id: string): Promise<Customer | null> => {
    try {
      loading.value = true
      error.value = null
      return await SalesService.getCustomer(id)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load customer'
      console.error('Error loading customer:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Create customer
  const createCustomer = async (data: CreateCustomerRequest): Promise<Customer | null> => {
    try {
      loading.value = true
      error.value = null
      const newCustomer = await SalesService.createCustomer(data)
      
      // Add to local customers array if loaded
      if (customers.value.length > 0) {
        customers.value.push(newCustomer)
      }
      
      return newCustomer
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create customer'
      console.error('Error creating customer:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Update customer
  const updateCustomer = async (id: string, data: Partial<CreateCustomerRequest>): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      const updatedCustomer = await SalesService.updateCustomer(id, data)
      
      // Update local customers array
      const index = customers.value.findIndex((customer: Customer) => customer.id === id)
      if (index !== -1) {
        customers.value[index] = updatedCustomer
      }
      
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update customer'
      console.error('Error updating customer:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Delete customer
  const deleteCustomer = async (id: string): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      await SalesService.deleteCustomer(id)
      
      // Remove from local customers array
      const index = customers.value.findIndex((customer: Customer) => customer.id === id)
      if (index !== -1) {
        customers.value.splice(index, 1)
      }
      
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to delete customer'
      console.error('Error deleting customer:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Load top products
  const loadTopProducts = async (params?: {
    period?: 'today' | 'week' | 'month'
    limit?: number
  }) => {
    try {
      loading.value = true
      error.value = null
      topProducts.value = await SalesService.getTopProducts(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load top products'
      console.error('Error loading top products:', err)
    } finally {
      loading.value = false
    }
  }

  // Generate sales report
  const generateReport = async (params: {
    startDate: string
    endDate: string
    format?: 'json' | 'csv' | 'pdf'
  }) => {
    try {
      loading.value = true
      error.value = null
      return await SalesService.generateReport(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to generate report'
      console.error('Error generating report:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Utility functions
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(amount)
  }

  const formatTime = (dateString: string): string => {
    const date = new Date(dateString)
    const now = new Date()
    const diffInMinutes = Math.floor((now.getTime() - date.getTime()) / (1000 * 60))
    
    if (diffInMinutes < 1) return 'Just now'
    if (diffInMinutes < 60) return `${diffInMinutes} minutes ago`
    
    const diffInHours = Math.floor(diffInMinutes / 60)
    if (diffInHours < 24) return `${diffInHours} hour${diffInHours > 1 ? 's' : ''} ago`
    
    const diffInDays = Math.floor(diffInHours / 24)
    return `${diffInDays} day${diffInDays > 1 ? 's' : ''} ago`
  }

  const getStatusBadgeClass = (status: Sale['status']): string => {
    switch (status) {
      case 'completed':
        return 'bg-green-100 text-green-600 dark:bg-green-900 dark:text-green-400'
      case 'pending':
        return 'bg-yellow-100 text-yellow-600 dark:bg-yellow-900 dark:text-yellow-400'
      case 'cancelled':
      case 'refunded':
        return 'bg-red-100 text-red-600 dark:bg-red-900 dark:text-red-400'
      default:
        return 'bg-gray-100 text-gray-600 dark:bg-gray-900 dark:text-gray-400'
    }
  }

  const getStatusText = (status: Sale['status']): string => {
    switch (status) {
      case 'completed':
        return 'Completed'
      case 'pending':
        return 'Pending'
      case 'cancelled':
        return 'Cancelled'
      case 'refunded':
        return 'Refunded'
      default:
        return 'Unknown'
    }
  }

  return {
    // Reactive state
    sales: readonly(sales),
    customers: readonly(customers),
    salesOverview: readonly(salesOverview),
    topProducts: readonly(topProducts),
    loading: readonly(loading),
    error: readonly(error),
    
    // Computed properties
    todaysSales,
    todaysOrders,
    averageOrderValue,
    totalCustomers,
    recentSales,
    
    // Sales methods
    loadSalesOverview,
    loadSales,
    getSale,
    createSale,
    updateSaleStatus,
    cancelSale,
    refundSale,
    
    // Customer methods
    loadCustomers,
    getCustomer,
    createCustomer,
    updateCustomer,
    deleteCustomer,
    
    // Product methods
    loadTopProducts,
    
    // Reporting
    generateReport,
    
    // Utility functions
    formatCurrency,
    formatTime,
    getStatusBadgeClass,
    getStatusText
  }
}

