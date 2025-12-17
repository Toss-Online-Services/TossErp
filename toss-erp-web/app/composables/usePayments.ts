/**
 * Payments API Composable
 * Connects to TOSS backend /api/payments endpoints
 */
export const usePayments = () => {
  const { get, post } = useApi()

  /**
   * Generate a payment link
   */
  const generatePayLink = async (payLinkData: {
    shopId: number
    amount: number
    reference?: string
    description?: string
    expiresAt?: string
  }) => {
    return await post<{
      id: number
      linkUrl: string
      expiresAt: string
    }>('/api/payments/pay-links', payLinkData)
  }

  /**
   * Record a payment transaction
   */
  const recordPayment = async (paymentData: {
    shopId: number
    saleId?: number
    amount: number
    paymentType: string
    paymentMethod?: string
    reference?: string
    paidAt: string
  }) => {
    return await post<{ id: number }>('/api/payments/record', paymentData)
  }

  /**
   * Get payments with filters
   */
  const getPayments = async (params?: {
    shopId?: number
    saleId?: number
    paymentType?: string
    startDate?: string
    endDate?: string
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/payments', params)
  }

  /**
   * Get payment by ID
   */
  const getPaymentById = async (id: number) => {
    return await get<any>(`/api/payments/${id}`)
  }

  return {
    generatePayLink,
    recordPayment,
    getPayments,
    getPaymentById
  }
}

