/**
 * Buying/Procurement API Composable
 * Connects to TOSS backend /api/buying endpoints
 */
export const useBuyingAPI = () => {
  const { get, post } = useApi()

  /**
   * Create a new purchase order
   */
  const createPurchaseOrder = async (poData: {
    supplierId: number
    shopId: number
    orderDate: string
    requiredDate?: string
    items: Array<{
      productId: number
      quantity: number
      unitPrice: number
    }>
    totalAmount: number
    notes?: string
  }) => {
    return await post<{ id: number }>('/api/buying/purchase-orders', poData)
  }

  /**
   * Get purchase order by ID
   */
  const getPurchaseOrderById = async (id: number) => {
    return await get<any>(`/api/buying/purchase-orders/${id}`)
  }

  /**
   * Approve a purchase order
   */
  const approvePurchaseOrder = async (id: number) => {
    return await post<any>(`/api/buying/purchase-orders/${id}/approve`, {})
  }

  /**
   * Get all purchase orders with filters
   */
  const getPurchaseOrders = async (params?: {
    shopId?: number
    supplierId?: number
    status?: string
    startDate?: string
    endDate?: string
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/buying/purchase-orders', params)
  }

  return {
    createPurchaseOrder,
    getPurchaseOrderById,
    approvePurchaseOrder,
    getPurchaseOrders
  }
}
