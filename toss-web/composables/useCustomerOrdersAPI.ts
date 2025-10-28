/**
 * Customer Orders API Composable
 * Connects to TOSS backend /api/customer-orders endpoints
 */
export const useCustomerOrdersAPI = () => {
  const config = useRuntimeConfig()
  const baseURL = (config.public.apiBase || 'https://localhost:5001') + '/api'

  return {
    /**
     * Create a new customer order
     */
    async createOrder(data: {
      customerId: number
      shopId: number
      items: Array<{
        productId: number
        quantity: number
        unitPrice: number
      }>
      shippingMethod?: string
      shippingAddressId?: number
      billingAddressId?: number
      notes?: string
      paymentMethod?: string
    }) {
      return await $fetch<{ id: number }>(`${baseURL}/CustomerOrders`, {
        method: 'POST',
        body: data
      })
    },

    /**
     * Get customer orders with optional filters
     */
    async getOrders(params?: {
      shopId?: number
      customerId?: number
      status?: string
      pageNumber?: number
      pageSize?: number
    }) {
      return await $fetch<Array<{
        id: number
        orderGuid: string
        orderNumber: string
        customerId: number
        customerName: string
        orderDate: string
        orderStatus: string
        shippingStatus: string
        paymentStatus: string
        orderTotal: number
        itemCount: number
      }>>(`${baseURL}/CustomerOrders`, {
        method: 'GET',
        params: params || {}
      })
    },

    /**
     * Update order status
     */
    async updateOrderStatus(orderId: number, newStatus: string, notes?: string) {
      return await $fetch<boolean>(`${baseURL}/CustomerOrders/${orderId}/status`, {
        method: 'POST',
        body: { newStatus, notes }
      })
    },

    /**
     * Cancel an order
     */
    async cancelOrder(orderId: number, reason?: string) {
      return await $fetch<boolean>(`${baseURL}/CustomerOrders/${orderId}/cancel`, {
        method: 'POST',
        body: { reason }
      })
    }
  }
}

