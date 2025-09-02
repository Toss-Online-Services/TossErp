export interface SKUItem {
  id: string
  tenantId: string
  sku: string
  name: string
  description?: string
  category?: string
  unit?: string
  sellingPrice?: number
  costPrice?: number
  reorderLevel?: number
  reorderQty?: number
  isActive: boolean
  createdAt: string
  updatedAt: string
  quantityOnHand?: number
}

export interface InventoryListResponse {
  items: SKUItem[]
  totalCount: number
}

export interface CreateSKUInput {
  sku: string
  name: string
  description?: string
  category?: string
  unit?: string
  sellingPrice?: number
  costPrice?: number
  reorderLevel?: number
  reorderQty?: number
}

export interface StockMovementInput {
  itemId: string
  quantity: number // positive for increment, negative for decrement
  reason?: string
}
