/**
 * Sales API Composable
 * Connects to TOSS backend /api/sales endpoints
 */
export const useSalesAPI = () => {
  const { get, post } = useApi()

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
    paymentType: string
    totalAmount: number
  }) => {
    return await post<{ id: number }>('/api/sales', saleData)
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
    return await get<any>('/api/sales', params)
  }

  /**
   * Get daily sales summary for dashboard
   */
  const getDailySummary = async (shopId: number) => {
    return await get<{
      totalSales: number
      totalRevenue: number
      totalCustomers: number
      avgTransactionValue: number
    }>('/api/sales/daily-summary', { shopId })
  }

  /**
   * Void/cancel a sale
   */
  const voidSale = async (saleId: number) => {
    return await post<any>(`/api/sales/${saleId}/void`, {})
  }

  /**
   * Generate receipt for a sale
   */
  const generateReceipt = async (saleId: number) => {
    return await post<{
      receiptNumber: string
      receiptUrl: string
    }>(`/api/sales/${saleId}/receipt`, {})
  }

  /**
   * Get sale by ID
   */
  const getSaleById = async (id: number) => {
    return await get<any>(`/api/sales/${id}`)
  }

  return {
    createSale,
    getSales,
    getDailySummary,
    voidSale,
    generateReceipt,
    getSaleById
  }
}
