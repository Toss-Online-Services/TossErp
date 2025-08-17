// Stock Management Composable
import { StockService, type StockItem, type StockOverview, type CreateStockItemRequest } from '~/services/stockService'

export const useStockManagement = () => {
  // Reactive state
  const stockItems = ref<StockItem[]>([])
  const stockOverview = ref<StockOverview | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  
  // Load stock overview
  const loadOverview = async () => {
    try {
      loading.value = true
      error.value = null
      stockOverview.value = await StockService.getOverview()
    } catch (err: any) {
      error.value = err.message || 'Failed to load stock overview'
      console.error('Failed to load stock overview:', err)
    } finally {
      loading.value = false
    }
  }
  
  // Load stock items
  const loadStockItems = async (filters?: {
    category?: string
    status?: string
    search?: string
  }) => {
    try {
      loading.value = true
      error.value = null
      stockItems.value = await StockService.getStockItems(filters)
    } catch (err: any) {
      error.value = err.message || 'Failed to load stock items'
      console.error('Failed to load stock items:', err)
    } finally {
      loading.value = false
    }
  }
  
  // Add new stock item
  const addStockItem = async (itemData: CreateStockItemRequest) => {
    try {
      loading.value = true
      error.value = null
      const newItem = await StockService.createStockItem(itemData)
      stockItems.value.unshift(newItem)
      
      // Refresh overview to get updated totals
      await loadOverview()
      
      return newItem
    } catch (err: any) {
      error.value = err.message || 'Failed to add stock item'
      console.error('Failed to add stock item:', err)
      throw err
    } finally {
      loading.value = false
    }
  }
  
  // Update stock item
  const updateStockItem = async (id: string, itemData: Partial<CreateStockItemRequest>) => {
    try {
      loading.value = true
      error.value = null
      const updatedItem = await StockService.updateStockItem({ id, ...itemData })
      
      // Update in local state
      const index = stockItems.value.findIndex(item => item.id === id)
      if (index !== -1) {
        stockItems.value[index] = updatedItem
      }
      
      // Refresh overview to get updated totals
      await loadOverview()
      
      return updatedItem
    } catch (err: any) {
      error.value = err.message || 'Failed to update stock item'
      console.error('Failed to update stock item:', err)
      throw err
    } finally {
      loading.value = false
    }
  }
  
  // Delete stock item
  const deleteStockItem = async (id: string) => {
    try {
      loading.value = true
      error.value = null
      await StockService.deleteStockItem(id)
      
      // Remove from local state
      stockItems.value = stockItems.value.filter(item => item.id !== id)
      
      // Refresh overview to get updated totals
      await loadOverview()
    } catch (err: any) {
      error.value = err.message || 'Failed to delete stock item'
      console.error('Failed to delete stock item:', err)
      throw err
    } finally {
      loading.value = false
    }
  }
  
  // Adjust stock quantity
  const adjustStock = async (stockItemId: string, adjustment: {
    type: 'increase' | 'decrease' | 'correction'
    quantity: number
    reason: string
  }) => {
    try {
      loading.value = true
      error.value = null
      await StockService.adjustStock({
        stockItemId,
        ...adjustment
      })
      
      // Refresh the affected item
      const updatedItem = await StockService.getStockItem(stockItemId)
      const index = stockItems.value.findIndex(item => item.id === stockItemId)
      if (index !== -1) {
        stockItems.value[index] = updatedItem
      }
      
      // Refresh overview to get updated totals
      await loadOverview()
    } catch (err: any) {
      error.value = err.message || 'Failed to adjust stock'
      console.error('Failed to adjust stock:', err)
      throw err
    } finally {
      loading.value = false
    }
  }
  
  // Computed properties
  const totalStockValue = computed(() => stockOverview.value?.totalStockValue || 0)
  const totalItems = computed(() => stockOverview.value?.totalItems || 0)
  const lowStockItems = computed(() => stockOverview.value?.lowStockItems || 0)
  const totalCategories = computed(() => stockOverview.value?.totalCategories || 0)
  
  // Filtered stock items
  const inStockItems = computed(() => stockItems.value.filter(item => item.status === 'in-stock'))
  const lowStockItemsList = computed(() => stockItems.value.filter(item => item.status === 'low-stock'))
  const outOfStockItems = computed(() => stockItems.value.filter(item => item.status === 'out-of-stock'))
  
  // Utility functions
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR',
      minimumFractionDigits: 2
    }).format(amount)
  }
  
  const getStatusBadgeClass = (status: string): string => {
    switch (status) {
      case 'in-stock':
        return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
      case 'low-stock':
        return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
      case 'out-of-stock':
        return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
      default:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300'
    }
  }
  
  const getStatusText = (status: string): string => {
    switch (status) {
      case 'in-stock':
        return 'In Stock'
      case 'low-stock':
        return 'Low Stock'
      case 'out-of-stock':
        return 'Out of Stock'
      default:
        return 'Unknown'
    }
  }
  
  // Auto-load overview when composable is used
  onMounted(() => {
    loadOverview()
  })
  
  return {
    // State
    stockItems: readonly(stockItems),
    stockOverview: readonly(stockOverview),
    loading: readonly(loading),
    error: readonly(error),
    
    // Actions
    loadOverview,
    loadStockItems,
    addStockItem,
    updateStockItem,
    deleteStockItem,
    adjustStock,
    
    // Computed
    totalStockValue,
    totalItems,
    lowStockItems,
    totalCategories,
    inStockItems,
    lowStockItemsList,
    outOfStockItems,
    
    // Utils
    formatCurrency,
    getStatusBadgeClass,
    getStatusText
  }
}
