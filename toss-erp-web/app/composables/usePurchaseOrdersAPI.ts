export const usePurchaseOrdersAPI = () => {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase
  const baseURL = apiBase + '/api'

  return {
    /**
     * Get purchase orders with optional filters
     */
    async getPurchaseOrders(params?: {
      shopId?: number
      status?: string
      skip?: number
      take?: number
    }) {
      return await $fetch<any[]>(`${baseURL}/Buying/purchase-orders`, {
        method: 'GET',
        params: params || {}
      })
    },

    /**
     * Get purchase order by ID
     */
    async getPurchaseOrderById(id: number) {
      return await $fetch<any>(`${baseURL}/Buying/purchase-orders/${id}`, {
        method: 'GET'
      })
    },

    /**
     * Create a new purchase order
     */
    async createPurchaseOrder(orderData: {
      shopId: number
      supplierId: number
      items: Array<{
        productId: number
        quantity: number
        unitPrice: number
        notes?: string
      }>
      notes?: string
    }) {
      return await $fetch<{ id: number }>(`${baseURL}/Buying/purchase-orders`, {
        method: 'POST',
        body: orderData
      })
    },

    /**
     * Approve a purchase order (supplier action)
     */
    async approvePurchaseOrder(id: number, notes?: string) {
      return await $fetch<{ success: boolean }>(`${baseURL}/Buying/purchase-orders/${id}/approve`, {
        method: 'POST',
        body: { notes }
      })
    },

    /**
     * Update purchase order status
     */
    async updatePurchaseOrderStatus(id: number, status: string, notes?: string) {
      return await $fetch<{ success: boolean }>(`${baseURL}/Buying/purchase-orders/${id}/status`, {
        method: 'POST',
        body: { status, notes }
      })
    },

    /**
     * Receive goods for a purchase order
     */
    async receiveGoods(id: number, items: Array<{
      productId: number
      quantityReceived: number
      notes?: string
    }>) {
      return await $fetch<{ success: boolean }>(`${baseURL}/Buying/purchase-orders/${id}/receive`, {
        method: 'POST',
        body: { items }
      })
    }
  }
}

