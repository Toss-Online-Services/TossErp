/**
 * CRM API Composable
 * Connects to TOSS backend /api/CRM endpoints
 */
export const useCRMAPI = () => {
  const config = useRuntimeConfig()
  const devLocal = (typeof window !== 'undefined' && window.location.hostname === 'localhost')
    ? 'http://localhost:5000'
    : ''
  const apiBase = devLocal || config.public.apiBase || 'http://localhost:5000'
  const baseURL = apiBase + '/api'

  return {
    /**
     * Get all customers for a shop
     */
    async getCustomers(shopId: number, searchTerm?: string, pageNumber: number = 1, pageSize: number = 100) {
      const resp: any = await $fetch(`${baseURL}/CRM/customers`, {
        method: 'GET',
        params: { shopId, searchTerm, pageNumber, pageSize }
      })
      const raw = resp?.items || resp?.Items || resp?.value || []
      const arr = Array.isArray(raw) ? raw : []
      // Normalize keys to camelCase for the UI
      return arr.map((c: any) => ({
        id: c.id ?? c.Id,
        shopId: c.shopId ?? c.ShopId,
        firstName: c.firstName ?? c.FirstName ?? '',
        lastName: c.lastName ?? c.LastName ?? '',
        fullName: c.fullName ?? c.FullName ?? `${(c.firstName ?? c.FirstName ?? '')} ${(c.lastName ?? c.LastName ?? '')}`.trim(),
        phoneNumber: c.phoneNumber ?? c.PhoneNumber ?? null,
        email: c.email ?? c.Email ?? null,
        isActive: c.isActive ?? c.IsActive ?? true,
        totalPurchaseAmount: c.totalPurchaseAmount ?? c.TotalPurchases ?? 0,
        totalPurchaseCount: c.totalPurchaseCount ?? c.TotalPurchaseCount ?? undefined,
        lastPurchaseDate: c.lastPurchaseDate ?? c.LastPurchaseDate ?? null
      }))
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

