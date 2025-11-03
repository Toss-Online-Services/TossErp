/**
 * Sales API Composable
 * Connects to TOSS backend /api/sales endpoints
 * 
 * Note: This handles completed sales transactions (POS).
 * For customer orders, use useCustomerOrdersAPI()
 * For products, use useProductsAPI()
 * For customers, use useCRMAPI()
 */
export const useSalesAPI = () => {
  const config = useRuntimeConfig()
  const devLocal = (typeof window !== 'undefined' && window.location.hostname === 'localhost')
    ? 'http://localhost:5000'
    : ''
  const apiBase = devLocal || config.public.apiBase || 'http://localhost:5000'
  const baseURL = apiBase + '/api'
  const productsAPI = useProductsAPI()
  const crmAPI = useCRMAPI()
  const ordersAPI = useCustomerOrdersAPI()

  /**
   * Create a new sale transaction (POS)
   */
  const createSale = async (saleData: {
    shopId: number
    customerId?: number
    items: Array<{
      productId: number
      quantity: number
      unitPrice: number
    }>
    paymentType: 'Cash' | 'Card' | 'MobileMoney' | 'BankTransfer' | 'PayLink'
    totalAmount: number
  }) => {
    return await $fetch<{ id: number }>(`${baseURL}/Sales`, {
      method: 'POST',
      body: {
        ...saleData
      }
    })
  }

  /**
   * Get sales with optional filters
   */
  const getSales = async (params?: {
    shopId?: number
    startDate?: string
    endDate?: string
    pageNumber?: number
    pageSize?: number
  }) => {
    return await $fetch<any>(`${baseURL}/Sales`, {
      method: 'GET',
      params: params || {}
    })
  }

  /**
   * Get daily sales summary for dashboard
   */
  const getDailySummary = async (shopId: number) => {
    return await $fetch<{
      date: string
      totalSales: number
      saleCount: number
      averageSaleValue: number
      cashSales: number
      cardSales: number
      mobileMoneySales: number
    }>(`${baseURL}/Sales/daily-summary`, {
      method: 'GET',
      params: { shopId }
    })
  }

  /**
   * Void/cancel a sale
   */
  const voidSale = async (saleId: number, reason?: string) => {
    return await $fetch<any>(`${baseURL}/Sales/${saleId}/void`, {
      method: 'POST',
      body: { reason }
    })
  }

  /**
   * Generate receipt for a sale
   */
  const generateReceipt = async (saleId: number) => {
    return await $fetch<{
      receiptNumber: string
      receiptUrl: string
    }>(`${baseURL}/Sales/${saleId}/receipt`, {
      method: 'POST'
    })
  }

  /**
   * Get sale by ID
   */
  const getSaleById = async (id: number) => {
    return await $fetch<any>(`${baseURL}/Sales/${id}`)
  }

  /**
   * Update sale status
   */
  const updateSaleStatus = async (saleId: number, newStatus: string, notes?: string) => {
    return await $fetch<boolean>(`${baseURL}/Sales/${saleId}/status`, {
      method: 'POST',
      body: { newStatus, notes }
    })
  }

  /**
   * Process refund for a sale
   */
  const processRefund = async (saleId: number, refundAmount: number, reason: string, restockItems: boolean = false) => {
    return await $fetch<{ refundId: number }>(`${baseURL}/Sales/${saleId}/refund`, {
      method: 'POST',
      body: { refundAmount, reason, restockItems }
    })
  }

  // === PROXY METHODS FOR CONVENIENCE ===
  // These delegate to the appropriate specialized composables

  /**
   * Get products (delegates to useProductsAPI)
   */
  const getProducts = async (shopId?: number) => {
    return await productsAPI.getProducts(shopId)
  }

  /**
   * Get categories (delegates to useProductsAPI)
   */
  const getCategories = async (shopId: number) => {
    return await productsAPI.getCategories(shopId)
  }

  /**
   * Get customers (delegates to useCRMAPI)
   */
  const getCustomers = async (shopId: number) => {
    return await crmAPI.getCustomers(shopId)
  }

  /**
   * Get customer orders (delegates to useCustomerOrdersAPI)
   */
  const getOrders = async (params?: {
    shopId?: number
    customerId?: number
    status?: string
  }) => {
    return await ordersAPI.getOrders(params)
  }

  /**
   * Create customer order (delegates to useCustomerOrdersAPI)
   */
  const createOrder = async (orderData: any) => {
    // Map the frontend order data to backend format
    return await ordersAPI.createOrder({
      customerId: orderData.customerId || 1, // TODO: Get from session
      shopId: orderData.shopId || 1, // TODO: Get from session
      items: orderData.orderItems?.map((item: any) => ({
        productId: item.id,
        quantity: item.quantity,
        unitPrice: item.price
      })) || [],
      notes: orderData.notes,
      paymentMethod: orderData.paymentMethod || 'Cash'
    })
  }

  /**
   * Update customer order status (delegates to useCustomerOrdersAPI)
   */
  const updateOrderStatus = async (orderId: number | string, newStatus: string) => {
    return await ordersAPI.updateOrderStatus(Number(orderId), newStatus)
  }

  /**
   * Complete a customer order (delegates to useCustomerOrdersAPI)
   */
  const completeOrder = async (orderId: number | string) => {
    return await ordersAPI.updateOrderStatus(Number(orderId), 'Complete')
  }

  /**
   * Cancel a customer order (delegates to useCustomerOrdersAPI)
   */
  const cancelOrder = async (orderId: number | string, reason?: string) => {
    return await ordersAPI.cancelOrder(Number(orderId), reason)
  }

  /**
   * Get invoices (sales can serve as invoices)
   */
  const getInvoices = async (shopId?: number) => {
    return await $fetch<any>(`${baseURL}/Sales/invoices`, {
      method: 'GET',
      params: { shopId: shopId || 1 }
    })
  }

  /**
   * Create invoice (creates a sale)
   */
  const createInvoice = async (invoiceData: {
    customer: string
    orderNumber?: string
    total: number
    status: string
    dueDate: Date
  }) => {
    // For now, treat invoices as sales
    // In production, you might have a separate Invoice entity
    return await createSale({
      shopId: 1, // TODO: Get from session
      customerId: 1, // TODO: Map customer name to ID
      items: [],
      paymentType: 'BankTransfer', // Account sales use bank transfer
      totalAmount: invoiceData.total
    })
  }

  /**
   * Update invoice status (updates sale status)
   */
  const updateInvoiceStatus = async (invoiceId: number | string, newStatus: string) => {
    return await updateSaleStatus(Number(invoiceId), newStatus)
  }

  /**
   * Hold a sale (save to database with OnHold status)
   */
  const holdSale = async (saleData: {
    shopId: number
    customerId?: number
    items: Array<{
      productId: number
      quantity: number
      unitPrice: number
    }>
    paymentMethod: 'Cash' | 'Card' | 'MobileMoney' | 'BankTransfer' | 'PayLink'
    totalAmount: number
    notes?: string
  }) => {
    return await $fetch<{ id: number }>(`${baseURL}/Sales/hold`, {
      method: 'POST',
      body: {
        ...saleData
      }
    })
  }

  /**
   * Get all held sales for a shop
   */
  const getHeldSales = async (shopId: number) => {
    return await $fetch<Array<{
      id: number
      saleNumber: string
      customerId?: number
      customerName: string
      heldAt: string
      total: number
      paymentMethod: string
      notes?: string
      items: Array<{
        productId: number
        productName: string
        quantity: number
        unitPrice: number
        total: number
      }>
    }>>(`${baseURL}/Sales/held`, {
      method: 'GET',
      params: { shopId }
    })
  }

  /**
   * Retrieve a held sale (change status from OnHold to Pending)
   */
  const retrieveHeldSale = async (saleId: number) => {
    return await $fetch<boolean>(`${baseURL}/Sales/${saleId}/retrieve`, {
      method: 'POST'
    })
  }

  /**
   * Delete a held sale
   */
  const deleteHeldSale = async (saleId: number) => {
    return await $fetch<boolean>(`${baseURL}/Sales/${saleId}/held`, {
      method: 'DELETE'
    })
  }

  /**
   * Get all voided sales for a shop
   */
  const getVoidedSales = async (shopId: number) => {
    return await $fetch<Array<{
      id: number
      saleNumber: string
      customerId?: number
      customerName: string
      voidedAt: string
      total: number
      paymentMethod: string
      voidReason?: string
      items: Array<{
        productId: number
        productName: string
        quantity: number
        unitPrice: number
        total: number
      }>
    }>>(`${baseURL}/Sales/voided`, {
      method: 'GET',
      params: { shopId }
    })
  }

  return {
    // Core sales methods
    createSale,
    getSales,
    getDailySummary,
    voidSale,
    generateReceipt,
    getSaleById,
    updateSaleStatus,
    processRefund,
    
    // Held sales methods
    holdSale,
    getHeldSales,
    retrieveHeldSale,
    deleteHeldSale,
    
    // Voided sales methods
    getVoidedSales,
    
    // Proxy methods for convenience
    getProducts,
    getCategories,
    getCustomers,
    getOrders,
    createOrder,
    updateOrderStatus,
    completeOrder,
    cancelOrder,
    getInvoices,
    createInvoice,
    updateInvoiceStatus
  }
}

