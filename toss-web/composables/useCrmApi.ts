import { useApi, useAuthApi } from './useApi'
import type { Customer } from '~/stores/crm'

export function useCrmApi() {
  const { getHeaders } = useAuthApi()
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase || 'http://localhost:5000/api'

  async function getCustomers(storeId?: number, pageNumber = 1, pageSize = 50) {
    const params = new URLSearchParams()
    if (storeId) params.append('storeId', storeId.toString())
    params.append('pageNumber', pageNumber.toString())
    params.append('pageSize', pageSize.toString())
    
    const url = `/crm/customers?${params.toString()}`
    const { data, error, execute } = useApi<{ items: Customer[], totalCount: number, pageNumber: number, totalPages: number }>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getCustomerById(id: number) {
    const { data, error, execute } = useApi<Customer>(`/crm/customers/${id}`, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function searchCustomers(searchTerm: string, storeId?: number) {
    const params = new URLSearchParams()
    params.append('searchTerm', searchTerm)
    if (storeId) params.append('storeId', storeId.toString())
    
    const url = `/crm/customers/search?${params.toString()}`
    const { data, error, execute } = useApi<Customer[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createCustomer(customer: Partial<Customer>) {
    const { data, error, execute } = useApi<{ id: number }>('/crm/customers', {
      method: 'POST',
      body: customer,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function updateCustomer(id: number, updates: Partial<Customer>) {
    const { data, error, execute } = useApi<Customer>(`/crm/customers/${id}`, {
      method: 'PUT',
      body: updates,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function deleteCustomer(id: number) {
    const { data, error, execute } = useApi(`/crm/customers/${id}`, {
      method: 'DELETE',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createInteraction(customerId: number, interaction: { type: string, subject: string, content: string }) {
    const { data, error, execute } = useApi<{ id: number }>(`/crm/customers/${customerId}/interactions`, {
      method: 'POST',
      body: interaction,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getInteractions(customerId: number, pageNumber = 1, pageSize = 50) {
    const params = new URLSearchParams()
    params.append('pageNumber', pageNumber.toString())
    params.append('pageSize', pageSize.toString())
    
    const url = `/crm/customers/${customerId}/interactions?${params.toString()}`
    const { data, error, execute } = useApi<{ items: any[], totalCount: number }>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  return {
    getCustomers,
    getCustomerById,
    searchCustomers,
    createCustomer,
    updateCustomer,
    deleteCustomer,
    createInteraction,
    getInteractions
  }
}



