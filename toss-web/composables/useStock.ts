// Stock management composable for TOSS ERP III
import { ref, computed, readonly } from 'vue'

// Stock API base URL
const STOCK_API_BASE = 'http://localhost:5001/api'

// Types based on the backend DTOs
export interface ItemDto {
  id: string
  tenantId: string
  sku: string
  barcode?: string
  name: string
  description?: string
  category: string
  unit: string
  sellingPrice: number
  costPrice?: number
  reorderLevel: number
  reorderQty: number
  isActive: boolean
  createdAt: string
  updatedAt: string
  quantityOnHand?: number
  price?: number
}

export interface CreateItemRequest {
  sku: string
  barcode?: string
  name: string
  description?: string
  category: string
  unit: string
  sellingPrice: number
  costPrice?: number
  reorderLevel: number
  reorderQty: number
  isActive: boolean
}

export interface UpdateItemRequest extends CreateItemRequest {
  id: string
}

export interface WarehouseDto {
  id: string
  tenantId: string
  code: string
  name: string
  description?: string
  parentWarehouseId?: string
  isGroup: boolean
  address?: string
  isActive: boolean
  type?: string
  itemCount?: number
  stockValue?: number
  createdAt: string
  updatedAt: string
}

export interface StockMovementDto {
  id: string
  tenantId: string
  itemId: string
  itemName: string
  itemSku: string
  warehouseId: string
  warehouseName: string
  movementType: 'IN' | 'OUT' | 'TRANSFER'
  quantity: number
  balanceQty: number
  rate?: number
  amount?: number
  voucherType: string
  voucherNo: string
  transactionDate: string
  createdAt: string
}

export interface StockLevelDto {
  itemId: string
  itemName: string
  itemSku: string
  warehouseId: string
  warehouseName: string
  quantityOnHand: number
  reservedQty: number
  availableQty: number
  reorderLevel: number
  costPrice?: number
  totalValue: number
}

export interface StockOverviewDto {
  totalItems: number
  lowStockItems: number
  outOfStockItems: number
  totalValue: number
  totalCategories: number
  categorySummary: CategorySummaryDto[]
}

export interface CategorySummaryDto {
  category: string
  itemCount: number
  totalValue: number
}

export interface StockEntryRequest {
  itemId: string
  warehouseId: string
  movementType: 'IN' | 'OUT' | 'TRANSFER'
  quantity: number
  rate?: number
  targetWarehouseId?: string
  reference?: string
  remarks?: string
}

export interface PaginationParams {
  page?: number
  pageSize?: number
  searchTerm?: string
  category?: string
  sortBy?: string
  sortOrder?: 'asc' | 'desc'
}

// Stock operations composable
export const useStock = () => {
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Mock data for development
  const getMockItems = (): ItemDto[] => [
    {
      id: '1',
      tenantId: 'tenant1',
      sku: 'BREAD-001',
      name: 'White Bread Loaf',
      description: 'Fresh white bread loaf',
      category: 'Bakery',
      unit: 'loaf',
      sellingPrice: 12.00,
      costPrice: 8.50,
      reorderLevel: 20,
      reorderQty: 50,
      isActive: true,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-15T10:30:00Z',
      quantityOnHand: 45
    },
    {
      id: '2',
      tenantId: 'tenant1',
      sku: 'MILK-001',
      name: 'Fresh Milk 1L',
      description: 'Fresh full cream milk',
      category: 'Dairy',
      unit: 'liter',
      sellingPrice: 25.00,
      costPrice: 18.00,
      reorderLevel: 25,
      reorderQty: 30,
      isActive: true,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-15T08:15:00Z',
      quantityOnHand: 15
    },
    {
      id: '3',
      tenantId: 'tenant1',
      sku: 'RICE-001',
      name: 'Basmati Rice 2kg',
      description: 'Premium basmati rice',
      category: 'Grains',
      unit: 'kg',
      sellingPrice: 48.00,
      costPrice: 35.00,
      reorderLevel: 10,
      reorderQty: 20,
      isActive: true,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-14T16:45:00Z',
      quantityOnHand: 8
    },
    {
      id: '4',
      tenantId: 'tenant1',
      sku: 'SOAP-001',
      name: 'Washing Powder 1kg',
      description: 'All-purpose washing powder',
      category: 'Cleaning',
      unit: 'kg',
      sellingPrice: 32.00,
      costPrice: 22.00,
      reorderLevel: 15,
      reorderQty: 25,
      isActive: true,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-13T11:20:00Z',
      quantityOnHand: 12
    }
  ]

  const getMockWarehouses = (): WarehouseDto[] => [
    {
      id: '1',
      tenantId: 'tenant1',
      code: 'MAIN',
      name: 'Main Store',
      description: 'Primary retail location',
      isGroup: false,
      address: 'Township Center, Main Road',
      isActive: true,
      type: 'RETAIL',
      itemCount: 45,
      stockValue: 2500.00,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-15T00:00:00Z'
    },
    {
      id: '2',
      tenantId: 'tenant1',
      code: 'COLD',
      name: 'Cold Storage Facility',
      description: 'Refrigerated storage for perishables',
      isGroup: false,
      address: 'Industrial Area, Cold Chain Hub',
      isActive: true,
      type: 'COLD_STORAGE',
      itemCount: 23,
      stockValue: 1850.00,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-15T00:00:00Z'
    },
    {
      id: '3',
      tenantId: 'tenant1',
      code: 'SHARED',
      name: 'Township Central Warehouse',
      description: 'Shared community warehouse facility',
      isGroup: true,
      address: 'Community Hub, Shared Facilities',
      isActive: true,
      type: 'SHARED',
      itemCount: 67,
      stockValue: 4200.00,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-15T00:00:00Z'
    }
  ]

  const getMockMovements = (): StockMovementDto[] => [
    {
      id: '1',
      tenantId: 'tenant1',
      itemId: '1',
      itemName: 'White Bread Loaf',
      itemSku: 'BREAD-001',
      warehouseId: '1',
      warehouseName: 'Main Store',
      movementType: 'IN',
      quantity: 50,
      balanceQty: 45,
      rate: 8.50,
      amount: 425.00,
      voucherType: 'Purchase',
      voucherNo: 'PO-2024-001',
      transactionDate: '2024-01-15T10:30:00Z',
      createdAt: '2024-01-15T10:30:00Z'
    },
    {
      id: '2',
      tenantId: 'tenant1',
      itemId: '2',
      itemName: 'Fresh Milk 1L',
      itemSku: 'MILK-001',
      warehouseId: '2',
      warehouseName: 'Cold Storage Facility',
      movementType: 'OUT',
      quantity: 10,
      balanceQty: 15,
      rate: 18.00,
      amount: 180.00,
      voucherType: 'Sale',
      voucherNo: 'SALE-2024-045',
      transactionDate: '2024-01-14T14:20:00Z',
      createdAt: '2024-01-14T14:20:00Z'
    },
    {
      id: '3',
      tenantId: 'tenant1',
      itemId: '4',
      itemName: 'Washing Powder 1kg',
      itemSku: 'SOAP-001',
      warehouseId: '3',
      warehouseName: 'Township Central Warehouse',
      movementType: 'TRANSFER',
      quantity: 5,
      balanceQty: 12,
      rate: 22.00,
      amount: 110.00,
      voucherType: 'Transfer',
      voucherNo: 'TRF-2024-012',
      transactionDate: '2024-01-13T09:15:00Z',
      createdAt: '2024-01-13T09:15:00Z'
    }
  ]

  // Helper function for API calls with fallback to mock data
  const apiCall = async <T>(endpoint: string, options: RequestInit = {}): Promise<T> => {
    loading.value = true
    error.value = null
    
    try {
      // For development, return mock data instead of making API calls
      if (endpoint === '/items') {
        return { items: getMockItems(), totalCount: getMockItems().length } as unknown as T
      }
      if (endpoint === '/warehouses') {
        return { warehouses: getMockWarehouses(), totalCount: getMockWarehouses().length } as unknown as T
      }
      if (endpoint === '/stock-movements') {
        return { movements: getMockMovements(), totalCount: getMockMovements().length } as unknown as T
      }

      const response = await fetch(`${STOCK_API_BASE}${endpoint}`, {
        headers: {
          'Content-Type': 'application/json',
          ...options.headers,
        },
        ...options,
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const data = await response.json()
      return data
    } catch (err) {
      // Fallback to mock data on error for development
      if (endpoint.includes('/items')) {
        return { items: getMockItems(), totalCount: getMockItems().length } as unknown as T
      }
      if (endpoint.includes('/warehouses')) {
        return { warehouses: getMockWarehouses(), totalCount: getMockWarehouses().length } as unknown as T
      }
      if (endpoint.includes('/stock-movements')) {
        return { movements: getMockMovements(), totalCount: getMockMovements().length } as unknown as T
      }
      
      error.value = err instanceof Error ? err.message : 'An unknown error occurred'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Items API
  const getItems = async (params: PaginationParams = {}) => {
    const searchParams = new URLSearchParams()
    if (params.page) searchParams.append('page', params.page.toString())
    if (params.pageSize) searchParams.append('pageSize', params.pageSize.toString())
    if (params.searchTerm) searchParams.append('searchTerm', params.searchTerm)
    if (params.category) searchParams.append('category', params.category)
    if (params.sortBy) searchParams.append('sortBy', params.sortBy)
    if (params.sortOrder) searchParams.append('sortOrder', params.sortOrder)
    
    const queryString = searchParams.toString()
    return apiCall<{ items: ItemDto[], totalCount: number }>(`/items${queryString ? '?' + queryString : ''}`)
  }

  const getItem = async (id: string) => {
    return apiCall<ItemDto>(`/items/${id}`)
  }

  const createItem = async (item: CreateItemRequest) => {
    return apiCall<ItemDto>('/items', {
      method: 'POST',
      body: JSON.stringify(item),
    })
  }

  const updateItem = async (item: UpdateItemRequest) => {
    return apiCall<void>(`/items/${item.id}`, {
      method: 'PUT',
      body: JSON.stringify(item),
    })
  }

  const deleteItem = async (id: string) => {
    return apiCall<void>(`/items/${id}`, {
      method: 'DELETE',
    })
  }

  const getLowStockItems = async (params: PaginationParams = {}) => {
    const searchParams = new URLSearchParams()
    if (params.page) searchParams.append('page', params.page.toString())
    if (params.pageSize) searchParams.append('pageSize', params.pageSize.toString())
    
    const queryString = searchParams.toString()
    return apiCall<{ items: ItemDto[], totalCount: number }>(`/items/low-stock${queryString ? '?' + queryString : ''}`)
  }

  const getOutOfStockItems = async (params: PaginationParams = {}) => {
    const searchParams = new URLSearchParams()
    if (params.page) searchParams.append('page', params.page.toString())
    if (params.pageSize) searchParams.append('pageSize', params.pageSize.toString())
    
    const queryString = searchParams.toString()
    return apiCall<{ items: ItemDto[], totalCount: number }>(`/items/out-of-stock${queryString ? '?' + queryString : ''}`)
  }

  const getStockOverview = async () => {
    return apiCall<StockOverviewDto>('/items/overview')
  }

  const getCategories = async () => {
    return apiCall<string[]>('/items/categories')
  }

  const getItemByBarcode = async (barcode: string) => {
    return apiCall<ItemDto>(`/items/barcode/${barcode}`)
  }

  const getItemBySku = async (sku: string) => {
    return apiCall<ItemDto>(`/items/sku/${sku}`)
  }

  // Warehouses API
  const getWarehouses = async (params: PaginationParams = {}) => {
    const searchParams = new URLSearchParams()
    if (params.page) searchParams.append('page', params.page.toString())
    if (params.pageSize) searchParams.append('pageSize', params.pageSize.toString())
    if (params.searchTerm) searchParams.append('searchTerm', params.searchTerm)
    
    const queryString = searchParams.toString()
    return apiCall<{ warehouses: WarehouseDto[], totalCount: number }>(`/warehouses${queryString ? '?' + queryString : ''}`)
  }

  const getWarehouse = async (id: string) => {
    return apiCall<WarehouseDto>(`/warehouses/${id}`)
  }

  // Stock Movements API
  const getStockMovements = async (params: PaginationParams & { itemId?: string, warehouseId?: string } = {}) => {
    const searchParams = new URLSearchParams()
    if (params.page) searchParams.append('page', params.page.toString())
    if (params.pageSize) searchParams.append('pageSize', params.pageSize.toString())
    if (params.itemId) searchParams.append('itemId', params.itemId)
    if (params.warehouseId) searchParams.append('warehouseId', params.warehouseId)
    if (params.searchTerm) searchParams.append('searchTerm', params.searchTerm)
    
    const queryString = searchParams.toString()
    return apiCall<{ movements: StockMovementDto[], totalCount: number }>(`/stock-movements${queryString ? '?' + queryString : ''}`)
  }

  const getItemStockHistory = async (itemId: string) => {
    return apiCall<StockMovementDto[]>(`/items/${itemId}/stock-history`)
  }

  // Stock Operations API
  const createStockEntry = async (entry: StockEntryRequest) => {
    return apiCall<void>('/stock-operations/entry', {
      method: 'POST',
      body: JSON.stringify(entry),
    })
  }

  const issueStock = async (entry: Omit<StockEntryRequest, 'movementType'>) => {
    return apiCall<void>('/stock-operations/issue', {
      method: 'POST',
      body: JSON.stringify(entry),
    })
  }

  const receiveStock = async (entry: Omit<StockEntryRequest, 'movementType'>) => {
    return apiCall<void>('/stock-operations/receive', {
      method: 'POST',
      body: JSON.stringify(entry),
    })
  }

  const transferStock = async (entry: Omit<StockEntryRequest, 'movementType'>) => {
    return apiCall<void>('/stock-operations/transfer', {
      method: 'POST',
      body: JSON.stringify(entry),
    })
  }

  // Stock Levels API
  const getStockLevels = async (params: PaginationParams & { warehouseId?: string } = {}) => {
    const searchParams = new URLSearchParams()
    if (params.page) searchParams.append('page', params.page.toString())
    if (params.pageSize) searchParams.append('pageSize', params.pageSize.toString())
    if (params.warehouseId) searchParams.append('warehouseId', params.warehouseId)
    if (params.searchTerm) searchParams.append('searchTerm', params.searchTerm)
    
    const queryString = searchParams.toString()
    return apiCall<{ stockLevels: StockLevelDto[], totalCount: number }>(`/stock-levels${queryString ? '?' + queryString : ''}`)
  }

  const getItemStockLevel = async (itemId: string, warehouseId?: string) => {
    const endpoint = warehouseId 
      ? `/stock-levels/${itemId}?warehouseId=${warehouseId}`
      : `/stock-levels/${itemId}`
    return apiCall<StockLevelDto[]>(endpoint)
  }

  return {
    loading: readonly(loading),
    error: readonly(error),
    
    // Items
    getItems,
    getItem,
    createItem,
    updateItem,
    deleteItem,
    getLowStockItems,
    getOutOfStockItems,
    getStockOverview,
    getCategories,
    getItemByBarcode,
    getItemBySku,
    
    // Warehouses
    getWarehouses,
    getWarehouse,
    
    // Stock Movements
    getStockMovements,
    getItemStockHistory,
    
    // Stock Operations
    createStockEntry,
    issueStock,
    receiveStock,
    transferStock,
    
    // Stock Levels
    getStockLevels,
    getItemStockLevel,
  }
}

// Helper composable for stock state management
export const useStockState = () => {
  const items = ref<ItemDto[]>([])
  const warehouses = ref<WarehouseDto[]>([])
  const stockMovements = ref<StockMovementDto[]>([])
  const stockLevels = ref<StockLevelDto[]>([])
  const categories = ref<string[]>([])
  const overview = ref<StockOverviewDto | null>(null)

  const lowStockItems = computed(() => 
    items.value.filter(item => 
      item.quantityOnHand !== undefined && 
      item.quantityOnHand <= item.reorderLevel
    )
  )

  const outOfStockItems = computed(() => 
    items.value.filter(item => 
      item.quantityOnHand !== undefined && 
      item.quantityOnHand <= 0
    )
  )

  const totalStockValue = computed(() => 
    items.value.reduce((total, item) => 
      total + ((item.quantityOnHand || 0) * (item.costPrice || item.sellingPrice)), 0
    )
  )

  const activeItems = computed(() => 
    items.value.filter(item => item.isActive)
  )

  const itemsByCategory = computed(() => {
    const grouped = items.value.reduce((acc, item) => {
      if (!acc[item.category]) {
        acc[item.category] = []
      }
      acc[item.category].push(item)
      return acc
    }, {} as Record<string, ItemDto[]>)
    
    return grouped
  })

  return {
    items,
    warehouses,
    stockMovements,
    stockLevels,
    categories,
    overview,
    
    // Computed properties
    lowStockItems,
    outOfStockItems,
    totalStockValue,
    activeItems,
    itemsByCategory,
  }
}
