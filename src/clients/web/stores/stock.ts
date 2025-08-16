import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface StockItem {
  id: number
  name: string
  description: string
  sku: string
  category: string
  warehouse: string
  quantity: number
  unitCost: number
  reorderLevel: number
  isActive: boolean
  lastUpdated: string
}

export interface StockMovement {
  id: number
  itemId: number
  itemName: string
  type: 'in' | 'out' | 'adjustment' | 'transfer'
  quantity: number
  warehouse: string
  reference: string
  reason: string
  timestamp: string
  createdBy: string
}

export interface StockStats {
  totalItems: number
  totalValue: number
  lowStockCount: number
  outOfStockCount: number
  warehousesCount: number
}

export const useStockStore = defineStore('stock', () => {
  // State
  const items = ref<StockItem[]>([])
  const movements = ref<StockMovement[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const filters = ref({
    search: '',
    warehouse: '',
    category: '',
    stockLevel: '',
    page: 1,
    itemsPerPage: 10
  })

  // Getters
  const filteredItems = computed(() => {
    return items.value.filter(item => {
      const matchesSearch = !filters.value.search || 
        item.name.toLowerCase().includes(filters.value.search.toLowerCase()) ||
        item.sku.toLowerCase().includes(filters.value.search.toLowerCase()) ||
        item.category.toLowerCase().includes(filters.value.search.toLowerCase())
      
      const matchesWarehouse = !filters.value.warehouse || item.warehouse === filters.value.warehouse
      const matchesCategory = !filters.value.category || item.category.toLowerCase() === filters.value.category
      
      let matchesStockLevel = true
      if (filters.value.stockLevel === 'in-stock') {
        matchesStockLevel = item.quantity > item.reorderLevel
      } else if (filters.value.stockLevel === 'low-stock') {
        matchesStockLevel = item.quantity > 0 && item.quantity <= item.reorderLevel
      } else if (filters.value.stockLevel === 'out-of-stock') {
        matchesStockLevel = item.quantity === 0
      }
      
      return matchesSearch && matchesWarehouse && matchesCategory && matchesStockLevel
    })
  })

  const paginatedItems = computed(() => {
    const start = (filters.value.page - 1) * filters.value.itemsPerPage
    const end = start + filters.value.itemsPerPage
    return filteredItems.value.slice(start, end)
  })

  const totalPages = computed(() => Math.ceil(filteredItems.value.length / filters.value.itemsPerPage))

  const stats = computed<StockStats>(() => {
    const totalValue = items.value.reduce((sum, item) => sum + (item.quantity * item.unitCost), 0)
    const lowStockCount = items.value.filter(item => item.quantity > 0 && item.quantity <= item.reorderLevel).length
    const outOfStockCount = items.value.filter(item => item.quantity === 0).length
    const warehouses = new Set(items.value.map(item => item.warehouse))
    
    return {
      totalItems: items.value.length,
      totalValue,
      lowStockCount,
      outOfStockCount,
      warehousesCount: warehouses.size
    }
  })

  const categories = computed(() => {
    return Array.from(new Set(items.value.map(item => item.category)))
  })

  const warehouses = computed(() => {
    return Array.from(new Set(items.value.map(item => item.warehouse)))
  })

  // Actions
  const setLoading = (loading: boolean) => {
    isLoading.value = loading
  }

  const setError = (err: string | null) => {
    error.value = err
  }

  const setFilters = (newFilters: Partial<typeof filters.value>) => {
    filters.value = { ...filters.value, ...newFilters }
    if (newFilters.search !== undefined || newFilters.warehouse !== undefined || 
        newFilters.category !== undefined || newFilters.stockLevel !== undefined) {
      filters.value.page = 1 // Reset to first page when filters change
    }
  }

  const setPage = (page: number) => {
    filters.value.page = page
  }

  const addItem = (item: Omit<StockItem, 'id' | 'lastUpdated'>) => {
    const newItem: StockItem = {
      ...item,
      id: Date.now(),
      lastUpdated: new Date().toISOString()
    }
    items.value.push(newItem)
  }

  const updateItem = (id: number, updates: Partial<StockItem>) => {
    const index = items.value.findIndex(item => item.id === id)
    if (index !== -1) {
      items.value[index] = { ...items.value[index], ...updates, lastUpdated: new Date().toISOString() }
    }
  }

  const deleteItem = (id: number) => {
    const index = items.value.findIndex(item => item.id === id)
    if (index !== -1) {
      items.value.splice(index, 1)
    }
  }

  const addMovement = (movement: Omit<StockMovement, 'id' | 'timestamp'>) => {
    const newMovement: StockMovement = {
      ...movement,
      id: Date.now(),
      timestamp: new Date().toISOString()
    }
    movements.value.unshift(newMovement)
    
    // Update item quantity based on movement type
    const item = items.value.find(i => i.id === movement.itemId)
    if (item) {
      if (movement.type === 'in') {
        item.quantity += movement.quantity
      } else if (movement.type === 'out') {
        item.quantity = Math.max(0, item.quantity - movement.quantity)
      } else if (movement.type === 'adjustment') {
        item.quantity = movement.quantity
      }
      item.lastUpdated = new Date().toISOString()
    }
  }

  const loadMockData = () => {
    // Mock items data
    items.value = [
      {
        id: 1,
        name: 'Laptop Computer',
        description: 'High-performance laptop for business use',
        sku: 'LAP-001',
        category: 'Electronics',
        warehouse: 'Main Warehouse',
        quantity: 25,
        unitCost: 899.99,
        reorderLevel: 5,
        isActive: true,
        lastUpdated: new Date().toISOString()
      },
      {
        id: 2,
        name: 'Wireless Mouse',
        description: 'Ergonomic wireless mouse',
        sku: 'MOU-002',
        category: 'Electronics',
        warehouse: 'Main Warehouse',
        quantity: 150,
        unitCost: 29.99,
        reorderLevel: 20,
        isActive: true,
        lastUpdated: new Date().toISOString()
      },
      {
        id: 3,
        name: 'Office Chair',
        description: 'Comfortable office chair with lumbar support',
        sku: 'CHA-003',
        category: 'Furniture',
        warehouse: 'Secondary Warehouse',
        quantity: 8,
        unitCost: 199.99,
        reorderLevel: 10,
        isActive: true,
        lastUpdated: new Date().toISOString()
      },
      {
        id: 4,
        name: 'Coffee Mug',
        description: 'Ceramic coffee mug, 12oz',
        sku: 'MUG-004',
        category: 'Kitchen',
        warehouse: 'Main Warehouse',
        quantity: 0,
        unitCost: 12.99,
        reorderLevel: 50,
        isActive: true,
        lastUpdated: new Date().toISOString()
      },
      {
        id: 5,
        name: 'Notebook',
        description: 'Spiral-bound notebook, 100 pages',
        sku: 'NOT-005',
        category: 'Office Supplies',
        warehouse: 'Main Warehouse',
        quantity: 75,
        unitCost: 5.99,
        reorderLevel: 25,
        isActive: true,
        lastUpdated: new Date().toISOString()
      }
    ]

    // Mock movements data
    movements.value = [
      {
        id: 1,
        itemId: 1,
        itemName: 'Laptop Computer',
        type: 'in',
        quantity: 50,
        warehouse: 'Main Warehouse',
        reference: 'PO-2024-001',
        reason: 'New stock received',
        timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString(),
        createdBy: 'John Doe'
      },
      {
        id: 2,
        itemId: 2,
        itemName: 'Wireless Mouse',
        type: 'out',
        quantity: 25,
        warehouse: 'Main Warehouse',
        reference: 'SO-2024-001',
        reason: 'Sales order fulfilled',
        timestamp: new Date(Date.now() - 4 * 60 * 60 * 1000).toISOString(),
        createdBy: 'Jane Smith'
      },
      {
        id: 3,
        itemId: 3,
        itemName: 'Office Chair',
        type: 'adjustment',
        quantity: 8,
        warehouse: 'Secondary Warehouse',
        reference: 'ADJ-2024-001',
        reason: 'Physical count adjustment',
        timestamp: new Date(Date.now() - 6 * 60 * 60 * 1000).toISOString(),
        createdBy: 'Mike Johnson'
      }
    ]
  }

  return {
    // State
    items,
    movements,
    isLoading,
    error,
    filters,
    
    // Getters
    filteredItems,
    paginatedItems,
    totalPages,
    stats,
    categories,
    warehouses,
    
    // Actions
    setLoading,
    setError,
    setFilters,
    setPage,
    addItem,
    updateItem,
    deleteItem,
    addMovement,
    loadMockData
  }
})
