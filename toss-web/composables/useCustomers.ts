/**
 * CRM/Customers API Composable
 * Connects to TOSS backend /api/crm endpoints
 */
export const useCustomers = () => {
  const { get, post } = useApi()

  /**
   * Get all customers with pagination and search
   */
  const getCustomers = async (params?: {
    shopId?: number
    searchTerm?: string
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/crm/customers', params)
  }

  /**
   * Get customer profile by ID
   */
  const getCustomerProfile = async (id: number) => {
    return await get<{
      id: number
      shopId: number
      name: string
      email?: string
      phone?: string
      address?: string
      totalPurchases: number
      lastPurchaseDate?: string
      loyaltyPoints?: number
      purchaseHistory: any[]
      interactions: any[]
    }>(`/api/crm/customers/${id}`)
  }

  /**
   * Create a new customer
   */
  const createCustomer = async (customerData: {
    shopId: number
    name: string
    email?: string
    phone?: string
    address?: string
  }) => {
    return await post<{ id: number }>('/api/crm/customers', customerData)
  }

  /**
   * Search customers by name, phone, or email
   */
  const searchCustomers = async (searchTerm: string, shopId?: number) => {
    return await get<any>('/api/crm/customers/search', { searchTerm, shopId })
  }

  return {
    getCustomers,
    getCustomerProfile,
    createCustomer,
    searchCustomers
  }
}

