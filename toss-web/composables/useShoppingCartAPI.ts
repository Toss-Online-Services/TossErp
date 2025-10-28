export const useShoppingCartAPI = () => {
  const config = useRuntimeConfig()
  const baseURL = (config.public.apiBase || 'https://localhost:5001') + '/api'

  return {
    /**
     * Add a product to the shopping cart
     */
    async addToCart(shopId: number, productId: number, quantity: number = 1, sessionId: string, customerId?: number, attributes?: string) {
      return await $fetch<{
        cartItemId: number
        totalItems: number
        cartTotal: number
        message: string
      }>(`${baseURL}/ShoppingCart/add`, {
        method: 'POST',
        body: {
          shopId,
          productId,
          quantity,
          sessionId,
          customerId,
          attributes
        }
      })
    },

    /**
     * Update cart item quantity
     */
    async updateCartItem(cartItemId: number, quantity: number, sessionId: string) {
      return await $fetch<{
        success: boolean
        totalItems: number
        cartTotal: number
        message: string
      }>(`${baseURL}/ShoppingCart/update`, {
        method: 'PUT',
        body: {
          cartItemId,
          quantity,
          sessionId
        }
      })
    },

    /**
     * Get shopping cart contents
     */
    async getCart(sessionId: string, shopId: number) {
      return await $fetch<{
        items: Array<{
          id: number
          productId: number
          productName: string
          productSku: string
          productImage: string | null
          quantity: number
          unitPrice: number
          taxRate: number
          discountAmount: number
          subtotal: number
          tax: number
          total: number
          attributes: string | null
        }>
        totalItems: number
        subtotal: number
        totalTax: number
        totalDiscount: number
        grandTotal: number
      }>(`${baseURL}/ShoppingCart/get`, {
        method: 'POST',
        body: { sessionId, shopId }
      })
    },

    /**
     * Checkout (complete sale from cart)
     */
    async checkout(
      sessionId: string,
      shopId: number,
      paymentMethod: string,
      amountPaid: number,
      customerId?: number,
      notes?: string
    ) {
      return await $fetch<{
        saleId: number
        saleNumber: string
        total: number
        amountPaid: number
        change: number
        completedAt: string
      }>(`${baseURL}/ShoppingCart/checkout`, {
        method: 'POST',
        body: {
          sessionId,
          shopId,
          paymentMethod,
          amountPaid,
          customerId,
          notes
        }
      })
    },

    /**
     * Clear shopping cart
     */
    async clearCart(sessionId: string, shopId: number) {
      return await $fetch<boolean>(`${baseURL}/ShoppingCart/clear`, {
        method: 'DELETE',
        params: { sessionId, shopId }
      })
    }
  }
}

