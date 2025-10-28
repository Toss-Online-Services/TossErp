/**
 * CRM API Composable
 * Connects to TOSS backend /api/CRM endpoints
 */
export const useCRMAPI = () => {
  const config = useRuntimeConfig()
  const baseURL = (config.public.apiBase || 'https://localhost:5001') + '/api'

  return {
    /**
     * Get all customers for a shop
     */
    async getCustomers(shopId: number, searchTerm?: string, pageNumber: number = 1, pageSize: number = 100) {
      return await $fetch<Array<{
        id: number
        shopId: number
        firstName: string
        lastName: string
        fullName: string
        phoneNumber: string | null
        email: string | null
        isActive: boolean
        totalPurchaseAmount: number
        totalPurchaseCount: number
        lastPurchaseDate: string | null
      }>>(`${baseURL}/CRM/customers`, {
        method: 'GET',
        params: { shopId, searchTerm, pageNumber, pageSize }
      })
    },

    /**
     * Search customers
     */
    async searchCustomers(shopId: number, searchTerm: string) {
      return await $fetch<Array<{
        id: number
        name: string
        email: string
        phone: string | null
        address: string | null
        shopId: number
      }>>(`${baseURL}/CRM/customers/search`, {
        method: 'GET',
        params: { shopId, searchTerm }
      })
    },

    /**
     * Get customer profile by ID
     */
    async getCustomerById(id: number) {
      return await $fetch<{
        id: number
        firstName: string
        lastName: string
        fullName: string
        phoneNumber: string | null
        email: string | null
        totalPurchases: number
        totalOrders: number
        lastPurchaseDate: string | null
        notes: string | null
      }>(`${baseURL}/CRM/customers/${id}`)
    },

    /**
     * Create a new customer
     */
    async createCustomer(data: {
      shopId: number
      firstName: string
      lastName: string
      phoneNumber?: string
      email?: string
      allowsMarketing?: boolean
      notes?: string
    }) {
      return await $fetch<{ id: number }>(`${baseURL}/CRM/customers`, {
        method: 'POST',
        body: data
      })
    }
  }
}

